using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils {
    public class Helpers
    {
        public static Vector2Int[] directions = new Vector2Int[] {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        public static LayerMask sceneLayerMask = LayerMask.GetMask("SceneColliders");




    }
}