using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneGenerator : MonoBehaviour
{
    [SerializeField] Room startRoomPrefab;
    [SerializeField] List<Room> roomPrefabs = new List<Room>();
    [SerializeField] List<Corridor> corridorPrefabs = new List<Corridor>();

    [SerializeField] Vector2Int roomNumberRange = new Vector2Int(1, 5);

    List<Connector> availableConnectors = new List<Connector>();

    List<Room> generatedRooms = new List<Room>();
    List<Corridor> generatedCorridors = new List<Corridor>();
    // Start is called before the first frame update
    LayerMask sceneLayerMask;
    
    void Start() {
        sceneLayerMask = LayerMask.GetMask("Scene");
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
        WaitForSeconds startup =  new WaitForSeconds(2);
        WaitForFixedUpdate fixedUpdateInterval = new WaitForFixedUpdate();
    
        yield return startup;
        PlaceStartRoom();

        int numberOfIterations = Random.Range(roomNumberRange.x, roomNumberRange.y);
        for(int i = 0; i < numberOfIterations; i++){
            PlaceRoom();
            yield return fixedUpdateInterval;

            PlaceCorridor();
            yield return fixedUpdateInterval;
        }


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

    void PlaceRoom(){
        // Debug.Log("place random room");
        Room currentRoom = Instantiate(roomPrefabs[Random.Range(0, roomPrefabs.Count)]) as Room;
        currentRoom.transform.parent = this.transform;
        List<Connector> currentRoomConnectors = currentRoom.getConnectors();

        foreach(Connector currentSceneConnector in availableConnectors){
            foreach(Connector currentRoomConnector in currentRoomConnectors){
                PositionRoomAtConnector(currentRoom, currentRoomConnector, currentSceneConnector);

                if(CheckRoomOverlap(currentRoom)){
                    Destroy(currentRoom);
                    continue;
                }

                AddRoomConnectorsToList(currentRoom);
                generatedRooms.Add(currentRoom);

                availableConnectors.Remove(currentSceneConnector);
                availableConnectors.Remove(currentRoomConnector);
                return;
            }
        }
    }

    void PlaceCorridor(){
        // Debug.Log("place corridor");
    }
    
    void AddRoomConnectorsToList(Room room){
        foreach(Connector connector in room.getConnectors()){
            int randomExitPoint = Random.Range(0, availableConnectors.Count);
            availableConnectors.Insert(randomExitPoint, connector);
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
        List<BoxCollider> roomColliders = room.getColliders();
        foreach(BoxCollider roomCollider in roomColliders){
            Bounds bounds = roomCollider.bounds;
            bounds.Expand(-0.1f);

            Collider[] colliders = Physics.OverlapBox(bounds.center, bounds.size / 2, room.transform.rotation, sceneLayerMask);
            if(colliders.Length > 0){
                foreach(Collider c in colliders){
                    if(c.transform.parent.gameObject.transform.parent.gameObject.Equals(room.gameObject)){
                        continue;
                    } else {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    void CleanUp(){
        generatedCorridors.Clear();
        generatedRooms.Clear();
        availableConnectors.Clear();
        foreach (Transform child in this.transform) {
            GameObject.Destroy(child.gameObject);
        }
    }


}
