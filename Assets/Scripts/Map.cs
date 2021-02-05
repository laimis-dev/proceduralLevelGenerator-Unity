using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GeneratorClasses
{
  public class Map
  {
    private int[,] map;
    private int width;
    private int height;
    private Vector2Int corridorRange;
    private List<Room> rooms = new List<Room>();

    public Map(int width, int height, Vector2Int corridorRange, List<Room> rooms)
    {
      this.width = width;
      this.height = height;
      this.corridorRange = corridorRange;
      this.map = new int[width, height];
      this.rooms = rooms;
    }

    public int[,] getMap()
    {
      return map;
    }

    public int getWidth()
    {
      return width;
    }

    public int getHeight()
    {
      return height;
    }

    public void setMapNode(int x, int y, int val)
    {
      this.map[x, y] = val;
    }

    public int getMapNode(int x, int y)
    {
      return this.map[x, y];
    }

    public Vector2Int getCorridorRange()
    {
      return corridorRange;
    }

    public List<Room> GetRooms()
    {
      return rooms;
    }
    
  }
}