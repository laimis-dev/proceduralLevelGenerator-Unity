using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneGenerator : MonoBehaviour
{
    [SerializeField] List<Room> roomPrefabs = new List<Room>();
    [SerializeField] List<Corridor> corridorPrefabs = new List<Corridor>();

    [SerializeField] Vector2Int roomNumberRange = new Vector2Int(1, 5);

    List<Room> generatedRooms = new List<Room>();
    List<Corridor> generatedCorridors = new List<Corridor>();
    // Start is called before the first frame update
    LayerMask sceneLayerMask;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
