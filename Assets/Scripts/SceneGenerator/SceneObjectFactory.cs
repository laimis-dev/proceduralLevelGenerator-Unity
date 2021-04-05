using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class SceneObjectFactory : MonoBehaviour {
    [SerializeField] Room startRoomPrefab;
    [SerializeField] Room endRoomPrefab;
    [SerializeField] List<Room> roomPrefabs = new List<Room>();
    [SerializeField] List<Corridor> corridorPrefabs = new List<Corridor>();
    [SerializeField] List<SpecialRoom> specialRoomPrefabs = new List<SpecialRoom>();

    public SceneObject Create(string type) {
        switch (type) {
            case "startRoom": 
                return Instantiate(startRoomPrefab);
            case "endRoom": 
                return Instantiate(endRoomPrefab);
            case "specialRoom": 
                SpecialRoom specRoomPrefab = specialRoomPrefabs[SceneGenerator.pseudoRandom.Next(0, specialRoomPrefabs.Count)];
                return Instantiate(specRoomPrefab);
            case "regularRoom": 
                Room roomInstance = roomPrefabs[SceneGenerator.pseudoRandom.Next(0, roomPrefabs.Count)];
                return Instantiate(roomInstance);
            case "corridor": 
                Corridor corridor = corridorPrefabs[SceneGenerator.pseudoRandom.Next(0, corridorPrefabs.Count)];
                return Instantiate(corridor);
            default:
                Debug.LogError("Wrong sceneObject type passed.");
                return null;
        }
    }

    public List<SceneObject> GetList(string type) {
        switch (type) {
            case "specialRoom": 
                return new List<SceneObject>(specialRoomPrefabs);
                
            case "regularRoom": 
                return new List<SceneObject>(roomPrefabs);
            case "corridor": 
                return new List<SceneObject>(corridorPrefabs);
            default:
                Debug.LogError("Wrong sceneObject type passed.");
                return null;
        }
    }
}