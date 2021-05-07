using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class SceneObjectFactory : MonoBehaviour
{
    [SerializeField] Room startRoomPrefab;
    [SerializeField] Room endRoomPrefab;
    [SerializeField] List<Room> roomPrefabs = new List<Room>();
    [SerializeField] List<Corridor> corridorPrefabs = new List<Corridor>();
    [SerializeField] List<SpecialRoom> specialRoomPrefabs = new List<SpecialRoom>();

    public SceneObject Create(string type)
    {
        switch (type)
        {
            case "startRoom":
                return Instantiate(startRoomPrefab);
            case "endRoom":
                return Instantiate(endRoomPrefab);
            case "specialRoom":
                int random = SceneGenerator.pseudoRandom.Next(0, specialRoomPrefabs.Count);
                // print(random);
                SpecialRoom specRoomPrefab = specialRoomPrefabs[random];
                return Instantiate(specRoomPrefab);
            case "regularRoom":
                random = SceneGenerator.pseudoRandom.Next(0, roomPrefabs.Count);
                // print(random);
                Room roomInstance = roomPrefabs[random];
                return Instantiate(roomInstance);
            case "corridor":
                random = SceneGenerator.pseudoRandom.Next(0, corridorPrefabs.Count);
                // print(random);
                Corridor corridor = corridorPrefabs[random];
                return Instantiate(corridor);
            default:
                Debug.LogError("Wrong sceneObject type passed.");
                return null;
        }
    }

    public List<SceneObject> GetList(string type)
    {
        switch (type)
        {
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