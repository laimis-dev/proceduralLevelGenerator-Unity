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
    private Vector2Int roomRange;
    private Vector2Int corridorRange;

    public Map(int width, int height, Vector2Int roomRange, Vector2Int corridorRange)
    {
      this.width = width;
      this.height = height;
      this.roomRange = roomRange;
      this.corridorRange = corridorRange;
      this.map = new int[width, height];
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

    public Vector2Int getRoomRange()
    {
      return roomRange;
    }

    public Vector2Int getCorridorRange()
    {
      return corridorRange;
    }
  }
}