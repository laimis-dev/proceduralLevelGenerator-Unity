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

        public static WaitForSeconds startup =  new WaitForSeconds(1);
        public static WaitForFixedUpdate fixedUpdateInterval = new WaitForFixedUpdate();

        public static LayerMask sceneLayerMask = LayerMask.GetMask("SceneColliders");

        public static GameObject GetRootGameObject(Transform transform){
            return transform.parent.gameObject.transform.parent.gameObject;
        }
        
        public static bool navBaked = false;
        
    }
}