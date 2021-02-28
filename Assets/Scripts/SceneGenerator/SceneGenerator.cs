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

    [SerializeField] Vector2Int roomNumberRange = new Vector2Int(1, 5);

    [SerializeField] GameObject cyclicConnectionPrefab;
    [SerializeField] float cyclicConnectionRange = 15f;
    [SerializeField] float maxGScore = 15f;
    [SerializeField] CorridorConnector corridorConnectorBuilder;

    [SerializeField] bool useRandomSeed = true;
    [SerializeField] string seed;

    List<Connector> availableRoomConnectors = new List<Connector>();
    List<Connector> availableCorridorConnectors = new List<Connector>();

    List<Room> generatedRooms = new List<Room>();
    List<Corridor> generatedCorridors = new List<Corridor>();
    LayerMask sceneLayerMask;

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
            Room currentRoom = Instantiate(roomPrefabs[pseudoRandom.Next(0, roomPrefabs.Count)]) as Room;
            PlaceRoom(currentRoom);
            yield return fixedUpdateInterval;
            // yield return startup;

            Corridor currentCorridor = Instantiate(corridorPrefabs[pseudoRandom.Next(0, corridorPrefabs.Count)]) as Corridor;
            PlaceCorridor(currentCorridor);
            yield return fixedUpdateInterval;
        }

        PlaceEndRoom();

        yield return StartCoroutine(ConnectEmptyConnectors());

        DeleteUnconnectedCorridors();
        ProcessDoors();
        // Debug.Log("finished");
        StopCoroutine("GenerateScene");
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

        List<Connector> sortedAvailableCorridorConnectors = new List<Connector>(availableCorridorConnectors);
        //sort connectors by distance
        for(int i = 1; i < sortedAvailableCorridorConnectors.Count; i++){
            Connector current = sortedAvailableCorridorConnectors[i];
            int j = i - 1;

            while(j >= 0 && 
                sortedAvailableCorridorConnectors[j].distanceFromStart < current.distanceFromStart){

                    sortedAvailableCorridorConnectors[j+1] = sortedAvailableCorridorConnectors[j];
                    j--;
            }

            sortedAvailableCorridorConnectors[j+1] = current;
        }

        foreach(Connector currentSceneCorridorConnector in sortedAvailableCorridorConnectors){
            foreach(Connector currentRoomConnector in currentRoomConnectors){
                PositionRoomAtConnector(endRoom, currentRoomConnector, currentSceneCorridorConnector);

                if(CheckRoomOverlap(endRoom)){
                    continue;
                }

                AddRoomConnectorsToList(endRoom);
                generatedRooms.Add(endRoom);

                availableCorridorConnectors.Remove(currentSceneCorridorConnector);
                availableRoomConnectors.Remove(currentRoomConnector);

                currentSceneCorridorConnector.connectedTo = currentRoomConnector;
                currentRoomConnector.connectedTo = currentSceneCorridorConnector;

                SetRoomConnectorDistance(endRoom, currentSceneCorridorConnector.distanceFromStart + 1);
                return;
            }
        }
        Destroy(endRoom.gameObject);
    }

    void PlaceRoom(Room currentRoom){
        // Debug.Log("place random room");
        currentRoom.transform.parent = this.transform;
        List<Connector> currentRoomConnectors = currentRoom.GetConnectors();

        foreach(Connector currentSceneCorridorConnector in availableCorridorConnectors){
            foreach(Connector currentRoomConnector in currentRoomConnectors){
                PositionRoomAtConnector(currentRoom, currentRoomConnector, currentSceneCorridorConnector);

                if(CheckRoomOverlap(currentRoom)){
                    continue;
                }

                AddRoomConnectorsToList(currentRoom);
                generatedRooms.Add(currentRoom);

                availableCorridorConnectors.Remove(currentSceneCorridorConnector);
                availableRoomConnectors.Remove(currentRoomConnector);

                currentSceneCorridorConnector.connectedTo = currentRoomConnector;
                currentRoomConnector.connectedTo = currentSceneCorridorConnector;

                SetRoomConnectorDistance(currentRoom, currentSceneCorridorConnector.distanceFromStart + 1);
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
        List<Connector> currentCorridorConnectors = currentCorridor.GetConnectors();

        foreach(Connector currentSceneRoomConnector in availableRoomConnectors){
            foreach(Connector currentCorridorConnector in currentCorridorConnectors){
                PositionCorridorAtConnector(currentCorridor, currentCorridorConnector, currentSceneRoomConnector);

                if(CheckCorridorOverlap(currentCorridor)){
                    continue;
                }

                AddCorridorConnectorsToList(currentCorridor);
                generatedCorridors.Add(currentCorridor);

                availableRoomConnectors.Remove(currentSceneRoomConnector);
                availableCorridorConnectors.Remove(currentCorridorConnector);

                currentSceneRoomConnector.connectedTo = currentCorridorConnector;
                currentCorridorConnector.connectedTo = currentSceneRoomConnector;

                SetCorridorConnectorDistance(currentCorridor, currentSceneRoomConnector.distanceFromStart + 1);

                return;
            }
        }
        Destroy(currentCorridor.gameObject);
    }


    void AddCorridorConnectorsToList(Corridor corridor){
        foreach(Connector connector in corridor.GetConnectors()){
            int randomExitPoint = pseudoRandom.Next(0, availableCorridorConnectors.Count);
            availableCorridorConnectors.Insert(randomExitPoint, connector);
        }
    }

    void SetCorridorConnectorDistance(Corridor corridor, int distance){
        foreach(Connector connector in corridor.GetConnectors()){
            connector.distanceFromStart = distance;
        }
    }


    void PositionCorridorAtConnector(Corridor corridor, Connector currentCorridorConnector, Connector targetConnector){
        corridor.transform.position = Vector3.zero;
        corridor.transform.rotation = Quaternion.identity;

        Vector3 targetConnectorEuler = targetConnector.transform.eulerAngles;
        Vector3 currentCorridorConnectorEuler = currentCorridorConnector.transform.eulerAngles;
        float deltaAngle = Mathf.DeltaAngle(currentCorridorConnectorEuler.y, targetConnectorEuler.y);
        Quaternion currentCorridorTargetRotation = Quaternion.AngleAxis(deltaAngle, Vector3.up);
        corridor.transform.rotation = currentCorridorTargetRotation * Quaternion.Euler(0, 180f, 0);

        Vector3 corridorPositionOffset = currentCorridorConnector.transform.position - corridor.transform.position;
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
        foreach(Connector corridorConnector in availableCorridorConnectors){
            if(corridorConnector.connectedTo != null) continue;
            List<Connector> foundConnectors = FindClosestConnectors(corridorConnector);
            foreach(Connector foundConnector in foundConnectors){
                CorridorConnector newBuilder = Instantiate(corridorConnectorBuilder);
                newBuilder.transform.parent = this.transform;
                newBuilder.SetMaxGScore(maxGScore);
                newBuilder.SetConnectionPoints(corridorConnector, foundConnector);
                yield return StartCoroutine(newBuilder.StartConnecting());

                if(newBuilder.isEndFound){
                    corridorConnector.connectedTo = foundConnector;
                    foundConnector.connectedTo = corridorConnector;
                    break;                   
                } else {
                    Destroy(newBuilder.gameObject);
                }
            }
        }
    }

    List<Connector> FindClosestConnectors(Connector corridorConnector){
        List<Connector> foundConnectors = new List<Connector>();
        foreach(Connector roomConnector in availableRoomConnectors){
            if(roomConnector.connectedTo != null) continue;

            float distanceBetweenConnectors = Vector3.Distance(
                corridorConnector.transform.position, 
                roomConnector.transform.position);

            if(distanceBetweenConnectors <= cyclicConnectionRange){
                if(!CheckIfSameRoom(corridorConnector, roomConnector)){
                    foundConnectors.Add(roomConnector);
                }
                
            }
        }

        //insertion sort
        for(int i = 1; i < foundConnectors.Count; i++){
            Connector current = foundConnectors[i];
            float currentDist = Vector3.Distance(
                corridorConnector.transform.position, 
                current.transform.position);
            int j = i - 1;

            while(j >= 0 && 
                Vector3.Distance(corridorConnector.transform.position, 
                foundConnectors[j].transform.position) > currentDist){

                    foundConnectors[j+1] = foundConnectors[j];
                    j--;
            }

            foundConnectors[j+1] = current;
        }

        return foundConnectors;
    }

    bool CheckIfSameRoom(Connector startCorridorConnector, Connector endRoomConnector){
        GameObject corridorGameObject = GetRootGameObject(startCorridorConnector.transform);
        Corridor currentCorridor = corridorGameObject.GetComponent<Corridor>();
        List<Connector> corridorConnectors = currentCorridor.GetConnectors();

        foreach(Connector corridorConnector in corridorConnectors){
            
            if(corridorConnector == startCorridorConnector) continue;
            if(corridorConnector.connectedTo == null) continue;
            GameObject roomGameObject = GetRootGameObject(corridorConnector.connectedTo.transform);
            Room currentRoom = roomGameObject.GetComponent<Room>();
            List<Connector> roomConnectors = currentRoom.GetConnectors();
            
            
            foreach(Connector roomConnector in roomConnectors){
                if(roomConnector.connectedTo != null) continue;
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
                if(connector.connectedTo != null) connections++;
            }

            if(connections < 2) {
                foreach(Connector connector in connectors){
                    if(connector.connectedTo != null) {
                        connector.connectedTo.connectedTo = null;
                    }
                }
                Destroy(corridor.gameObject);
            }
        }
    }

    void ProcessDoors(){
        foreach(Corridor corridor in generatedCorridors){
            List<Connector> connectors = corridor.GetConnectors();
            foreach(Connector connector in connectors){
                connector.ProcessDoor();
            }
        }

        foreach(Room room in generatedRooms){
            List<Connector> connectors = room.GetConnectors();
            foreach(Connector connector in connectors){
                connector.ProcessDoor();
            }
        }
    }

    GameObject GetRootGameObject(Transform transform){
        return transform.parent.gameObject.transform.parent.gameObject;
    }


    void CleanUp(){
        generatedCorridors.Clear();
        generatedRooms.Clear();
        availableRoomConnectors.Clear();
        availableCorridorConnectors.Clear();
        foreach (Transform child in this.transform) {
            GameObject.Destroy(child.gameObject);
        }
    }


}
