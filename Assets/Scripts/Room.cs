using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
  [SerializeField] Vector2Int roomSizeRange = new Vector2Int(4, 10);
  // Start is called before the first frame update
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {

  }

  public Vector2Int getRoomRange()
  {
    return roomSizeRange;
  }
}
