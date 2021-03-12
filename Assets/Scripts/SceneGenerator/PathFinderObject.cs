using System.Collections;
using System.Collections.Generic;
using System;

using UnityEngine;

public class PathFinderObject: MonoBehaviour
{
    [SerializeField] BoxCollider collider;
    LayerMask sceneLayerMask;
    void Start() {
        sceneLayerMask = LayerMask.GetMask("SceneColliders");
    }

    public BoxCollider GetCollider(){
        return collider;
    }

    public bool CheckOverlap(){
        BoxCollider boxCollider = this.GetCollider();

        // print(boxCollider);
        Bounds bounds = boxCollider.bounds;
        bounds.Expand(-0.1f);

        Collider[] colliders = Physics.OverlapBox(boxCollider.transform.position, bounds.size / 2, boxCollider.transform.rotation, sceneLayerMask);
        if(colliders.Length > 0){
            foreach(Collider c in colliders){
                if(c.transform.parent.gameObject.transform.parent.gameObject.Equals(gameObject)){
                    continue;
                } else {
                    return true;
                }
            }
        }
        

        return false;
    }
}
