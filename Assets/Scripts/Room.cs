using UnityEngine;

namespace GeneratorClasses
{
  [CreateAssetMenu(fileName = "Room", menuName = "Room/ Create Room", order = 0)]
  public class Room : ScriptableObject
  {
    [SerializeField] Vector2Int roomRange;

    public Room(Vector2Int range)
    {
      this.roomRange = range;
    }

    public Vector2Int getRoomRange()
    {
      return roomRange;
    }
  }
}