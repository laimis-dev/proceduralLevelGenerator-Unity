using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
  [SerializeField] Vector2Int roomSizeRange = new Vector2Int(4, 10);
  [SerializeField] GameObject baseFloor;


  Vector2Int roomSize;
  float offset = 0.5f;
  // Start is called before the first frame update
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {

  }

  public void generateRoom(Vector2Int startPosition){
    Room room = Instantiate(this);
    room.transform.position = new Vector3(startPosition.x, 0f, startPosition.y);

    for(int x = 0; x<roomSize.x; x++){
      for(int y = 0; y<roomSize.y; y++){
        GameObject createdFloor = Instantiate(baseFloor);
        createdFloor.transform.parent = room.transform;

        Vector3 parentPos = room.transform.position;
        createdFloor.transform.position = new Vector3(parentPos.x + x + offset, 0f, parentPos.z + y + offset);
      }
    }
  }

  public void setRoomSize(Vector2Int roomSize){
    this.roomSize = roomSize;
  }

  public Vector2Int getRoomRange()
  {
    return roomSizeRange;
  }
}
