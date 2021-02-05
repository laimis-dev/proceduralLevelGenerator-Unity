using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GeneratorClasses;

namespace GeneratorClasses
{
  public class Agent : MonoBehaviour 
  {
    private Vector2Int position;
    private Vector2Int movementDirection;
    private Map map;
    private GameObject corridorFloor;

    private bool isRoomGenerated = false;
    private bool isCorridorGenerated = false;
    private bool running = true;

    float offset = 0.5f;

    static Vector2Int[] directions = new Vector2Int[] {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right
    };

    //Without start position

    public Agent(Map map, GameObject floor) {
      Vector2Int position = new Vector2Int(
        LevelGenerator.pseudoRandom.Next(0, map.getWidth()),
        LevelGenerator.pseudoRandom.Next(0, map.getHeight()));
      this.position = position;
      this.map = map;
      this.corridorFloor = floor;
    }

    //With start position
    public Agent(Vector2Int startPosition, Map map, GameObject floor) {
      this.position = startPosition;
      this.map = map;
      this.corridorFloor = floor;
    }

    public void Process() {
      // Debug.Log("running " + running);
      // Debug.Log("is room " + isRoomGenerated);
      // Debug.Log("isCorridor " + isCorridorGenerated);
      if(!running) return;

      PreProcess();
      PlaceRoom();
      PlaceCorridor();

      if(!isRoomGenerated || !isCorridorGenerated) running = false;
    }

    private void PreProcess() {
      isRoomGenerated = false;
      isCorridorGenerated = false;
      RandomDirection();
    }

    private void PlaceRoom() {
      List<Room> rooms = new List<Room>(map.GetRooms());
      int roomStartPosX = 0;
      int roomStartPosY = 0;
      int roomEndPosX = 0;
      int roomEndPosY = 0;
      int roomWidth = 0;
      int roomHeight = 0;
      Room room = null;

      Debug.Log("Room count " + rooms.Count);
      
      while(rooms.Count > 0){
        Debug.Log("Trying to place ROOM");
        bool isRoomPlaceable = true;
        room = rooms[LevelGenerator.pseudoRandom.Next(0, rooms.Count)];
        Vector2Int roomSizeRange = room.getRoomRange();

        roomWidth = LevelGenerator.pseudoRandom.Next(roomSizeRange.x, roomSizeRange.y);
        roomHeight = LevelGenerator.pseudoRandom.Next(roomSizeRange.x, roomSizeRange.y);

        if(this.movementDirection.Equals(Vector2Int.up)){
          //UP is DOWN in an array
          roomStartPosX = this.position.x - roomWidth / 2;
          roomEndPosX = this.position.x + roomWidth / 2;

          roomStartPosY = this.position.y + roomHeight;
          roomEndPosY = this.position.y ;
        } else if(this.movementDirection.Equals(Vector2Int.down)){
          roomStartPosX = this.position.x - roomWidth / 2;
          roomEndPosX = this.position.x + roomWidth / 2;

          roomStartPosY = this.position.y - roomHeight;
          roomEndPosY = this.position.y ;
        } else if(this.movementDirection.Equals(Vector2Int.left)){
          roomStartPosY = this.position.y - roomHeight/2;
          roomEndPosY = this.position.y + roomHeight/2;

          roomStartPosX = this.position.x - roomWidth;
          roomEndPosX = this.position.x ;
        } else if(this.movementDirection.Equals(Vector2Int.right)){
          roomStartPosY = this.position.y - roomHeight/2;
          roomEndPosY = this.position.y + roomHeight/2;

          roomStartPosX = this.position.x + roomWidth;
          roomEndPosX = this.position.x;  
        }
        

        //Check room placement
        for (int x = roomStartPosX; x < roomEndPosX; x++) {
          for (int y = roomStartPosY; y < roomEndPosY; y++) {
            if (x < 0 || x > map.getWidth() - 1
             || y < 0 || y > map.getHeight() - 1
             || map.getMapNode(x,y) == 1 || map.getMapNode(x,y) == 2) {
              
              // Debug.Log(x + " " + y);
              // Debug.Log("room NOT placeable");
              isRoomPlaceable = false;
              break;
            }
          }

          if(!isRoomPlaceable) {
            break;
          }

        }

        if(!isRoomPlaceable) {
          rooms.Remove(room);
          // Debug.Log("room removed");
          // Debug.Log("rooms size " + rooms.Count);
        } else {
          break;
        }
      }

      if(rooms.Count <= 0) return;
      // Debug.Log(roomStartPosX);
      // Debug.Log(roomStartPosY);
      // Debug.Log(roomEndPosX);
      // Debug.Log(roomEndPosY);
      for (int x = roomStartPosX; x < roomEndPosX; x++) {
        for (int y = roomStartPosY; y < roomEndPosY; y++) {
          map.setMapNode(x, y, 1);
          // Debug.Log(map.getMapNode(x,y));

        }
      }

      room.setRoomSize(new Vector2Int(roomWidth, roomHeight));
      room.generateRoom(new Vector2Int(roomStartPosX, roomStartPosY));
      isRoomGenerated = true;
      Debug.Log("ROOM PLACED");
    }

    private void PlaceCorridor() {
      // Debug.Log("Trying to place Corridor");
      // RandomDirection();
      List<Vector2Int> newDirections = new List<Vector2Int>(directions);
      
      for(int i = 0; i < newDirections.Count; i++){
        // ChangeDirection(i);
        this.movementDirection = newDirections[LevelGenerator.pseudoRandom.Next(0, newDirections.Count)];

        Vector2Int corridorSizeRange = map.getCorridorRange();
        int corridorLength = LevelGenerator.pseudoRandom.Next(corridorSizeRange.x, corridorSizeRange.y);

        Vector2Int newPosition = this.position + this.movementDirection * corridorLength;
        // Debug.Log("Checking Corridor");
        // Debug.Log(newPosition);
        if (newPosition.x < 0 || newPosition.x > map.getWidth() - 1
        || newPosition.y < 0 || newPosition.y > map.getHeight() - 1){

          Debug.Log("Corridor not placed");
          newDirections.Remove(this.movementDirection);
        } else {
          // Debug.Log("Moving for Corridor");
          for (int j = 0; j < corridorLength; j++){
            Move();
          }
          
          isCorridorGenerated = true;
          Debug.Log("CORRIDOR PLACED");
          break;
        }
      }
     
      
    }

    private void Move() {
      Vector2Int newPosition = this.position + this.movementDirection;
      this.position = newPosition;

      if(map.getMapNode(this.position.x,this.position.y) == 1 
       || map.getMapNode(this.position.x,this.position.y) == 2){
         return;
       }
       
      GameObject createdFloor = Instantiate(corridorFloor);
      createdFloor.transform.SetParent(GameObject.Find("LevelGenerator").transform);
      createdFloor.transform.position = new Vector3(newPosition.x + offset,  0f, newPosition.y + offset);
      // Debug.Log(this.position);
      map.setMapNode(this.position.x, this.position.y, 2);
      
    }

    public void RandomDirection(){
      Debug.Log("direction changed");
      this.movementDirection = directions[LevelGenerator.pseudoRandom.Next(0, 3)];
    }

    public void ChangeDirection(int index){
      Debug.Log("direction changed " + index);
      this.movementDirection = directions[index];
    }

    public Vector2 getPosition(){
      return this.position;
    }

    public Vector2 getDirection(){
      return movementDirection;
    }

  }
}



