using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room1 : MonoBehaviour
{
  // Start is called before the first frame update
  [SerializeField] GameObject baseFloor;

  Vector2Int roomSize;
  Room room;
  void Start()
  {
    room = GetComponent<Room>();
    GenerateRoom();
  }

  // Update is called once per frame
  void Update()
  {

  }

  void GenerateRoom()
  {
    roomSize = room.getRoomSize();
    Debug.Log(roomSize);
    for (int x = 0; x < roomSize.x; x++)
    {
      for (int y = 0; y < roomSize.y; y++)
      {
        GameObject createdFloor = Instantiate(baseFloor);
        createdFloor.transform.parent = room.transform;

        Vector3 parentPos = room.transform.position;
        createdFloor.transform.position = new Vector3(parentPos.x + x + Room.offset, 0f, parentPos.z + y + Room.offset);
      }
    }
  }
}
