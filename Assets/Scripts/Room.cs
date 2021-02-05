using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
  [SerializeField] Vector2Int roomSizeRange = new Vector2Int(4, 10);

  Vector2Int roomSize;
  public static float offset = 0.5f;

  // Start is called before the first frame update
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {

  }

  public void instantiateRoom(Vector2Int startPosition, Vector2Int roomSize){
    Room room = Instantiate(this);
    room.setRoomSize(roomSize);
    room.transform.SetParent(GameObject.Find("LevelGenerator").transform);
    room.transform.position = new Vector3(startPosition.x, 0f, startPosition.y);
  }

  public void setRoomSize(Vector2Int roomSize){
    this.roomSize = roomSize;
  }

  public Vector2Int getRoomSize(){
    return this.roomSize;
  }

  public Vector2Int getRoomRange()
  {
    return roomSizeRange;
  }
}
