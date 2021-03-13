using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SceneGenerator : MonoBehaviour
{
    [SerializeField] Room startRoomPrefab;
    [SerializeField] Room endRoomPrefab;
    [SerializeField] List<Room> roomPrefabs = new List<Room>();
    [SerializeField] List<Corridor> corridorPrefabs = new List<Corridor>();
    [SerializeField] List<SpecialRoom> specialRoomPrefabs = new List<SpecialRoom>();

    [SerializeField] Vector2Int roomNumberRange = new Vector2Int(1, 5);

    [SerializeField] GameObject cyclicConnectionPrefab;
    [SerializeField] float cyclicConnectionRange = 15f;
    [SerializeField] float maxGScore = 15f;
    [SerializeField] PathFinder pathFinderBuilder;

    [SerializeField] bool useRandomSeed = true;
    [SerializeField] string seed;

    List<Connector> availableRoomConnectors = new List<Connector>();
    List<Connector> availablePathFinders = new List<Connector>();

    List<Room> generatedRooms = new List<Room>();
    List<Corridor> generatedCorridors = new List<Corridor>();
    List<SpecialRoom> generatedSpecialRooms = new List<SpecialRoom>();
    List<PathFinder> pathFinders = new List<PathFinder>();

    LayerMask sceneLayerMask;

    bool wasRoomPlaced = false;
    bool wasCorridorPlaced = false;

    Vector2Int[] directions = new Vector2Int[] {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right
    };
        
    System.Random pseudoRandom;
    
    void Start() {
        sceneLayerMask = LayerMask.GetMask("SceneColliders");
        StartCoroutine("GenerateScene");
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            CleanUp();
            StartCoroutine("GenerateScene");
        }   
    }

    IEnumerator GenerateScene(){
        if (useRandomSeed) {
            seed = Time.time.ToString();
        }

        pseudoRandom = new System.Random(seed.GetHashCode());

        WaitForSeconds startup =  new WaitForSeconds(1);
        WaitForFixedUpdate fixedUpdateInterval = new WaitForFixedUpdate();
    
        yield return startup;
        PlaceStartRoom();

        int numberOfIterations = pseudoRandom.Next(roomNumberRange.x, roomNumberRange.y);
        for(int i = 0; i < numberOfIterations - 1; i++){
            wasRoomPlaced = false;
            wasCorridorPlaced = false;
            List<Room> possibleRooms = new List<Room>(roomPrefabs);
            List<Corridor> possibleCorridors = new List<Corridor>(corridorPrefabs);

            for(int j = 0; j < specialRoomPrefabs.Count; j++){
                SpecialRoom specRoomPrefab = specialRoomPrefabs[j];

                if(specRoomPrefab.GetSpawnChance() < pseudoRandom.Next(0, 100)) continue;
                if(specRoomPrefab.GetMaxAmountPerScene() <= CountGeneratedSpecialRooms(specRoomPrefab)) continue;
                    
                SpecialRoom specRoom = Instantiate(specRoomPrefab);
                PlaceSpecialRoom(specRoom);
                break;
            }

            while(!wasRoomPlaced && possibleRooms.Count > 0){
                Room currentRoom = possibleRooms[pseudoRandom.Next(0, possibleRooms.Count)];
                possibleRooms.Remove(currentRoom);
                PlaceRoom(Instantiate(currentRoom));
                yield return fixedUpdateInterval;
            }
            
            
            // yield return startup;
            
            while(!wasCorridorPlaced && possibleCorridors.Count > 0){
                Corridor currentCorridor = corridorPrefabs[pseudoRandom.Next(0, corridorPrefabs.Count)];
                possibleCorridors.Remove(currentCorridor);
                PlaceCorridor(Instantiate(currentCorridor));
                yield return fixedUpdateInterval;
            }
        }

        PlaceEndRoom();

        yield return StartCoroutine(ConnectEmptyConnectors());

        DeleteUnconnectedCorridors();
        ProcessDoors();

        AddWallsToPathFinders();
        // Debug.Log("finished");
        StopCoroutine("GenerateScene");
    }

    int CountGeneratedSpecialRooms(SpecialRoom specRoom){
        int count = 0;
        for(int i = 0; i < generatedSpecialRooms.Count; i++){
            SpecialRoom room = generatedSpecialRooms[i];
            if(room.GetName() == specRoom.GetName()) count++;
        }
        // print(count);
        return count;
    }

    void PlaceStartRoom(){
        // Debug.Log("placed start room");
        Room startRoom = Instantiate(startRoomPrefab);
        startRoom.transform.parent = this.transform;
        generatedRooms.Add(startRoom);

        AddRoomConnectorsToList(startRoom);

        startRoom.transform.position = Vector3.zero;
        startRoom.transform.rotation = Quaternion.identity;
    }

    void PlaceEndRoom(){
        Room endRoom = Instantiate(endRoomPrefab) as Room;
        endRoom.transform.parent = this.transform;
        List<Connector> currentRoomConnectors = endRoom.GetConnectors();

        List<Connector> sortedAvailablePathFinders = new List<Connector>(availablePathFinders);
        //sort connectors by distance
        for(int i = 1; i < sortedAvailablePathFinders.Count; i++){
            Connector current = sortedAvailablePathFinders[i];
            int j = i - 1;

            while(j >= 0 && 
                sortedAvailablePathFinders[j].distanceFromStart < current.distanceFromStart){

                    sortedAvailablePathFinders[j+1] = sortedAvailablePathFinders[j];
                    j--;
            }

            sortedAvailablePathFinders[j+1] = current;
        }

        foreach(Connector currentScenePathFinder in sortedAvailablePathFinders){
            foreach(Connector currentRoomConnector in currentRoomConnectors){
                PositionRoomAtConnector(endRoom, currentRoomConnector, currentScenePathFinder);

                if(CheckRoomOverlap(endRoom)){
                    continue;
                }

                AddRoomConnectorsToList(endRoom);
                generatedRooms.Add(endRoom);

                availablePathFinders.Remove(currentScenePathFinder);
                availableRoomConnectors.Remove(currentRoomConnector);

                currentScenePathFinder.SetConnectedTo(currentRoomConnector);
                currentRoomConnector.SetConnectedTo(currentScenePathFinder);

                SetRoomConnectorDistance(endRoom, currentScenePathFinder.distanceFromStart + 1);

                return;
            }
        }
        Destroy(endRoom.gameObject);
    }

    void PlaceRoom(Room currentRoom){
        // Debug.Log("place random room");
        currentRoom.transform.parent = this.transform;
        List<Connector> currentRoomConnectors = currentRoom.GetConnectors();

        foreach(Connector currentScenePathFinder in availablePathFinders){
            foreach(Connector currentRoomConnector in currentRoomConnectors){
                PositionRoomAtConnector(currentRoom, currentRoomConnector, currentScenePathFinder);

                if(CheckRoomOverlap(currentRoom)){
                    continue;
                }

                AddRoomConnectorsToList(currentRoom);
                generatedRooms.Add(currentRoom);

                availablePathFinders.Remove(currentScenePathFinder);
                availableRoomConnectors.Remove(currentRoomConnector);

                currentScenePathFinder.SetConnectedTo(currentRoomConnector);
                currentRoomConnector.SetConnectedTo(currentScenePathFinder);

                SetRoomConnectorDistance(currentRoom, currentScenePathFinder.distanceFromStart + 1);
                wasRoomPlaced = true;
                return;
            }
        }
        Destroy(currentRoom.gameObject);
    }

    void PlaceSpecialRoom(SpecialRoom currentRoom){
        // Debug.Log("place random room");
        currentRoom.transform.parent = this.transform;
        List<Connector> currentRoomConnectors = currentRoom.GetConnectors();

        foreach(Connector currentScenePathFinder in availablePathFinders){
            if(currentScenePathFinder.distanceFromStart <= currentRoom.GetMinSpawnDistance()) continue;
            foreach(Connector currentRoomConnector in currentRoomConnectors){
                PositionRoomAtConnector(currentRoom, currentRoomConnector, currentScenePathFinder);

                if(CheckRoomOverlap(currentRoom)){
                    continue;
                }

                AddRoomConnectorsToList(currentRoom);
                generatedRooms.Add(currentRoom);
                generatedSpecialRooms.Add(currentRoom);

                availablePathFinders.Remove(currentScenePathFinder);
                availableRoomConnectors.Remove(currentRoomConnector);

                currentScenePathFinder.SetConnectedTo(currentRoomConnector);
                currentRoomConnector.SetConnectedTo(currentScenePathFinder);

                SetRoomConnectorDistance(currentRoom, currentScenePathFinder.distanceFromStart + 1);
                wasRoomPlaced = true;
                return;
            }
        }
        Destroy(currentRoom.gameObject);
    }
  
    void AddRoomConnectorsToList(Room room){
        foreach(Connector connector in room.GetConnectors()){
            int randomExitPoint = pseudoRandom.Next(0, availableRoomConnectors.Count);
            availableRoomConnectors.Insert(randomExitPoint, connector);
        }
    }

    void SetRoomConnectorDistance(Room room, int distance){
        foreach(Connector connector in room.GetConnectors()){
            connector.distanceFromStart = distance;
        }
    }

    void PositionRoomAtConnector(Room room, Connector currentRoomConnector, Connector targetConnector){
        room.transform.position = Vector3.zero;
        room.transform.rotation = Quaternion.identity;

        Vector3 targetConnectorEuler = targetConnector.transform.eulerAngles;
        Vector3 currentRoomConnectorEuler = currentRoomConnector.transform.eulerAngles;
        float deltaAngle = Mathf.DeltaAngle(currentRoomConnectorEuler.y, targetConnectorEuler.y);
        Quaternion currentRoomTargetRotation = Quaternion.AngleAxis(deltaAngle, Vector3.up);
        room.transform.rotation = currentRoomTargetRotation * Quaternion.Euler(0, 180f, 0);

        Vector3 roomPositionOffset = currentRoomConnector.transform.position - room.transform.position;
        room.transform.position = targetConnector.transform.position - roomPositionOffset;
    }

    bool CheckRoomOverlap(Room room){
        List<BoxCollider> roomColliders = room.GetColliders();
        foreach(BoxCollider roomCollider in roomColliders){
            Bounds bounds = roomCollider.bounds;
            bounds.Expand(-0.1f);

            Collider[] colliders = Physics.OverlapBox(roomCollider.transform.position, bounds.size / 2, roomCollider.transform.rotation, sceneLayerMask);
            if(colliders.Length > 0){
                foreach(Collider c in colliders){
                    if(GetRootGameObject(c.transform).Equals(room.gameObject)){
                        continue;
                    } else {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    void PlaceCorridor(Corridor currentCorridor){
        
        currentCorridor.transform.parent = this.transform;
        List<Connector> currentPathFinders = currentCorridor.GetConnectors();

        foreach(Connector currentSceneRoomConnector in availableRoomConnectors){
            foreach(Connector currentPathFinder in currentPathFinders){
                PositionCorridorAtConnector(currentCorridor, currentPathFinder, currentSceneRoomConnector);

                if(CheckCorridorOverlap(currentCorridor)){
                    continue;
                }

                AddPathFindersToList(currentCorridor);
                generatedCorridors.Add(currentCorridor);

                availableRoomConnectors.Remove(currentSceneRoomConnector);
                availablePathFinders.Remove(currentPathFinder);

                currentSceneRoomConnector.SetConnectedTo(currentPathFinder);
                currentPathFinder.SetConnectedTo(currentSceneRoomConnector);

                SetPathFinderDistance(currentCorridor, currentSceneRoomConnector.distanceFromStart + 1);

                return;
            }
        }
        Destroy(currentCorridor.gameObject);
    }


    void AddPathFindersToList(Corridor corridor){
        foreach(Connector connector in corridor.GetConnectors()){
            int randomExitPoint = pseudoRandom.Next(0, availablePathFinders.Count);
            availablePathFinders.Insert(randomExitPoint, connector);
        }
    }

    void SetPathFinderDistance(Corridor corridor, int distance){
        foreach(Connector connector in corridor.GetConnectors()){
            connector.distanceFromStart = distance;
        }
    }


    void PositionCorridorAtConnector(Corridor corridor, Connector currentPathFinder, Connector targetConnector){
        corridor.transform.position = Vector3.zero;
        corridor.transform.rotation = Quaternion.identity;

        Vector3 targetConnectorEuler = targetConnector.transform.eulerAngles;
        Vector3 currentPathFinderEuler = currentPathFinder.transform.eulerAngles;
        float deltaAngle = Mathf.DeltaAngle(currentPathFinderEuler.y, targetConnectorEuler.y);
        Quaternion currentCorridorTargetRotation = Quaternion.AngleAxis(deltaAngle, Vector3.up);
        corridor.transform.rotation = currentCorridorTargetRotation * Quaternion.Euler(0, 180f, 0);

        Vector3 corridorPositionOffset = currentPathFinder.transform.position - corridor.transform.position;
        corridor.transform.position = targetConnector.transform.position - corridorPositionOffset;
    }

    bool CheckCorridorOverlap(Corridor corridor){
        List<BoxCollider> corridorColliders = corridor.GetColliders();
        foreach(BoxCollider corridorCollider in corridorColliders){
            Bounds bounds = corridorCollider.bounds;
            bounds.Expand(-0.1f);

            Collider[] colliders = Physics.OverlapBox(corridorCollider.transform.position, bounds.size / 2, corridorCollider.transform.rotation, sceneLayerMask);
            if(colliders.Length > 0){
                foreach(Collider c in colliders){
                    if(GetRootGameObject(c.transform).Equals(corridor.gameObject)){
                        continue;
                    } else {
                        return true;
                    }
                }
            }
        }
        return false;
    }






    IEnumerator ConnectEmptyConnectors(){
        foreach(Connector pathFinder in availablePathFinders){
            if(pathFinder.GetConnectedTo() != null) continue;
            List<Connector> foundConnectors = FindClosestConnectors(pathFinder);
            foreach(Connector foundConnector in foundConnectors){
                PathFinder newBuilder = Instantiate(pathFinderBuilder);
                newBuilder.transform.parent = this.transform;
                newBuilder.SetMaxGScore(maxGScore);
                newBuilder.SetConnectionPoints(pathFinder, foundConnector);
                yield return StartCoroutine(newBuilder.StartConnecting());

                if(newBuilder.isEndFound){
                    pathFinder.SetConnectedTo(foundConnector);
                    foundConnector.SetConnectedTo(pathFinder);
                    pathFinders.Add(newBuilder);
                    break;                   
                } else {
                    Destroy(newBuilder.gameObject);
                }
            }
        }
    }

    void AddWallsToPathFinders(){
        foreach(PathFinder pathFinder in pathFinders){
            StartCoroutine(pathFinder.AddWallsToPath());
        }
    }

    List<Connector> FindClosestConnectors(Connector pathFinder){
        List<Connector> foundConnectors = new List<Connector>();
        foreach(Connector roomConnector in availableRoomConnectors){
            if(roomConnector.GetConnectedTo() != null) continue;

            float distanceBetweenConnectors = Vector3.Distance(
                pathFinder.transform.position, 
                roomConnector.transform.position);

            if(distanceBetweenConnectors <= cyclicConnectionRange){
                if(!CheckIfSameRoom(pathFinder, roomConnector)){
                    foundConnectors.Add(roomConnector);
                }
                
            }
        }

        //insertion sort
        for(int i = 1; i < foundConnectors.Count; i++){
            Connector current = foundConnectors[i];
            float currentDist = Vector3.Distance(
                pathFinder.transform.position, 
                current.transform.position);
            int j = i - 1;

            while(j >= 0 && 
                Vector3.Distance(pathFinder.transform.position, 
                foundConnectors[j].transform.position) > currentDist){

                    foundConnectors[j+1] = foundConnectors[j];
                    j--;
            }

            foundConnectors[j+1] = current;
        }

        return foundConnectors;
    }

    bool CheckIfSameRoom(Connector startPathFinder, Connector endRoomConnector){
        GameObject corridorGameObject = GetRootGameObject(startPathFinder.transform);
        Corridor currentCorridor = corridorGameObject.GetComponent<Corridor>();
        List<Connector> pathFinders = currentCorridor.GetConnectors();

        foreach(Connector pathFinder in pathFinders){
            
            if(pathFinder == startPathFinder) continue;
            if(pathFinder.GetConnectedTo() == null) continue;
            GameObject roomGameObject = GetRootGameObject(pathFinder.GetConnectedTo().transform);
            Room currentRoom = roomGameObject.GetComponent<Room>();
            List<Connector> roomConnectors = currentRoom.GetConnectors();
            
            
            foreach(Connector roomConnector in roomConnectors){
                if(roomConnector.GetConnectedTo() != null) continue;
                if(roomConnector.transform.position == endRoomConnector.transform.position){
                    return true;
                }
            }
        }
        return false;
    }


    void DeleteUnconnectedCorridors(){
        foreach(Corridor corridor in generatedCorridors){
            List<Connector> connectors = corridor.GetConnectors();
            int connections = 0;
            foreach(Connector connector in connectors){
                if(connector.GetConnectedTo() != null) connections++;
            }

            if(connections < 2) {
                foreach(Connector connector in connectors){
                    if(connector.GetConnectedTo() != null) {
                        connector.GetConnectedTo().SetConnectedTo(null);
                    }
                }
                Destroy(corridor.gameObject);
            }
        }
    }

    void ProcessDoors(){

    }

    GameObject GetRootGameObject(Transform transform){
        return transform.parent.gameObject.transform.parent.gameObject;
    }


    void CleanUp(){
        generatedCorridors.Clear();
        generatedRooms.Clear();
        generatedSpecialRooms.Clear();
        availableRoomConnectors.Clear();
        availablePathFinders.Clear();
        pathFinders.Clear();
        foreach (Transform child in this.transform) {
            GameObject.Destroy(child.gameObject);
        }
    }


}
