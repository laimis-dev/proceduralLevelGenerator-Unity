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
    private int roomPlacementChanceModifier;
    private int corridorPlacementChanceModifier;
    private int roomPlacementChance;
    private int corridorPlacementChance;
    private Vector2Int movementDirection;
    private Map map;

    static Vector2Int[] directions = new Vector2Int[] {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right
    };

    //Without start position

    public Agent(int roomChanceMod, int corChanceMod, Map map)
    {
      Vector2Int position = new Vector2Int(
        LevelGenerator.pseudoRandom.Next(0, map.getWidth()),
        LevelGenerator.pseudoRandom.Next(0, map.getHeight()));
      this.position = position;
      this.roomPlacementChanceModifier = roomChanceMod;
      this.corridorPlacementChanceModifier = corChanceMod;
      this.roomPlacementChance = 0;
      this.corridorPlacementChance = 0;
      this.map = map;
      RandomDirection();
    }

    //With start position
    public Agent(Vector2Int startPosition, int roomChanceMod, int corChanceMod, Map map)
    {
      this.position = startPosition;
      this.roomPlacementChanceModifier = roomChanceMod;
      this.corridorPlacementChanceModifier = corChanceMod;
      this.roomPlacementChance = 0;
      this.corridorPlacementChance = 0;
      this.map = map;
      RandomDirection();
    }

    public void Process()
    {
      Move();

      map.setMapNode(this.position.x, this.position.y, 1);

      if (LevelGenerator.pseudoRandom.Next(0, 100) < roomPlacementChance)
      {
        roomPlacementChance = 0;
        PlaceRoom();
      }
      else
      {
        roomPlacementChance += roomPlacementChanceModifier;
      }

      if (LevelGenerator.pseudoRandom.Next(0, 100) < corridorPlacementChance)
      {
        corridorPlacementChance = 0;
        RandomDirection();
        // PlaceCorridor();
      }
      else
      {
        corridorPlacementChance += corridorPlacementChanceModifier;
      }

    }

    private void PlaceRoom()
    {
      Vector2Int roomSizeRange = map.getRoomRange();

      int roomWidth = LevelGenerator.pseudoRandom.Next(roomSizeRange.x, roomSizeRange.y);
      int roomHeight = LevelGenerator.pseudoRandom.Next(roomSizeRange.x, roomSizeRange.y);

      int roomStartPosX = this.position.x - roomWidth / 2;
      int roomStartPosY = this.position.y - roomHeight / 2;
      int roomEndPosX = this.position.x + roomWidth / 2;
      int roomEndPosY = this.position.y + roomHeight / 2;

      for (int x = roomStartPosX; x < roomEndPosX; x++)
      {
        for (int y = roomStartPosY; y < roomEndPosY; y++)
        {
          map.setMapNode(x, y, 1);
        }
      }
      Debug.Log("ROOM PLACED");
    }

    private void PlaceCorridor()
    {
      Vector2Int corridorSizeRange = map.getCorridorRange();
      int corridorLength = LevelGenerator.pseudoRandom.Next(corridorSizeRange.x, corridorSizeRange.y);

      for (int i = 0; i < corridorLength; i++)
      {
        Move();
      }
      Debug.Log("CORRIDOR PLACED");
    }

    private void Move()
    {
      Vector2Int newPosition = this.position + this.movementDirection;

      //TODO refactor this garbage
      if (newPosition.x == 0 || newPosition.x == map.getWidth() - 1
        || newPosition.y == 0 || newPosition.y == map.getHeight() - 1)
      {
        RandomDirection();
        newPosition = this.position + this.movementDirection;
        if (newPosition.x == 0 || newPosition.x == map.getWidth() - 1
        || newPosition.y == 0 || newPosition.y == map.getHeight() - 1)
        {
          this.position = newPosition;
        }
        else
        {
          RandomDirection();
          newPosition = this.position + this.movementDirection;
          this.position = newPosition;
        }
      }
      else
      {
        this.position = newPosition;
      }
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



