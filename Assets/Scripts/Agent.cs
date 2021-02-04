using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GeneratorClasses;

namespace GeneratorClasses
{
  public class Agent
  {
    private Vector2Int position;
    private Vector2Int movementDirection;
    private Map map;

    private bool isRoomGenerated = false;
    private bool isCorridorGenerated = false;
    private bool running = true;

    static Vector2Int[] directions = new Vector2Int[] {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right
    };

    //Without start position

    public Agent(Map map)
    {
      Vector2Int position = new Vector2Int(
        LevelGenerator.pseudoRandom.Next(0, map.getWidth()),
        LevelGenerator.pseudoRandom.Next(0, map.getHeight()));
      this.position = position;
      this.map = map;
    }

    //With start position
    public Agent(Vector2Int startPosition, Map map)
    {
      this.position = startPosition;
      this.map = map;
    }

    public void Process()
    {
      if(!running) return;

      PreProcess();
      PlaceRoom();
      PlaceCorridor();

      if(!isRoomGenerated && !isCorridorGenerated) running = false;
    }

    private void PreProcess(){
      isRoomGenerated = false;
      isCorridorGenerated = false;
    }

    private void PlaceRoom()
    {
      List<Room> rooms = map.GetRooms();
      int roomStartPosX = 0;
      int roomStartPosY = 0;
      int roomEndPosX = 0;
      int roomEndPosY = 0;
      
      while(rooms.Count > 0){
        bool isRoomPlaceable = true;
        Room room = rooms[LevelGenerator.pseudoRandom.Next(0, rooms.Count)];
        Vector2Int roomSizeRange = room.getRoomRange();

        int roomWidth = LevelGenerator.pseudoRandom.Next(roomSizeRange.x, roomSizeRange.y);
        int roomHeight = LevelGenerator.pseudoRandom.Next(roomSizeRange.x, roomSizeRange.y);

        roomStartPosX = this.position.x + roomWidth / 2;
        roomStartPosY = this.position.y + roomHeight / 2;
        roomEndPosX = this.position.x + roomWidth / 2;
        roomEndPosY = this.position.y + roomHeight / 2;

        //Check room placement
        for (int x = roomStartPosX; x < roomEndPosX; x++)
        {
          for (int y = roomStartPosY; y < roomEndPosY; y++)
          {
            if (x == 0 || x == map.getWidth() - 1
            || y == 0 || y == map.getHeight() - 1
            || map.getMapNode(x,y) == 1)
            {
              Debug.Log("room NOT placeable");
              isRoomPlaceable = false;
            }
          }
        }

        if(!isRoomPlaceable){
          rooms.Remove(room);
        } else {
          break;
        }
      }

      if(rooms.Count <= 0) return;

      for (int x = roomStartPosX; x < roomEndPosX; x++)
      {
        for (int y = roomStartPosY; y < roomEndPosY; y++)
        {
          map.setMapNode(x, y, 1);
        }
      }
      isRoomGenerated = true;
      Debug.Log("ROOM PLACED");
    }

    private void PlaceCorridor()
    {
      RandomDirection();
      Vector2Int corridorSizeRange = map.getCorridorRange();
      int corridorLength = LevelGenerator.pseudoRandom.Next(corridorSizeRange.x, corridorSizeRange.y);

      Vector2Int newPosition = this.position + this.movementDirection * corridorLength;
      if (newPosition.x == 0 || newPosition.x == map.getWidth() - 1
        || newPosition.y == 0 || newPosition.y == map.getHeight() - 1)
      {
        for (int i = 0; i < corridorLength; i++)
        {
          Move();
        }
        isCorridorGenerated = true;
        Debug.Log("CORRIDOR PLACED");
      }
      
    }

    private void Move()
    {
      Vector2Int newPosition = this.position + this.movementDirection;
      this.position = newPosition;
      map.setMapNode(this.position.x, this.position.y, 1);
      
    }

    public void RandomDirection()
    {
      Debug.Log("direction changed");
      this.movementDirection = directions[LevelGenerator.pseudoRandom.Next(0, 3)];
    }

    public Vector2 getPosition()
    {
      return this.position;
    }

    public Vector2 getDirection()
    {
      return movementDirection;
    }

  }
}



