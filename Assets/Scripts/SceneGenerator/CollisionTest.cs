using System.Collections;
using System.Collections.Generic;
using Utils;
using UnityEngine;

public class CollisionTest : MonoBehaviour
{
    [SerializeField] Room room;

    void Start()
    {

    }

    void Update()
    {
        List<BoxCollider> roomColliders = room.GetColliders();
        foreach(BoxCollider roomCollider in roomColliders){
            Bounds bounds = roomCollider.bounds;
            bounds.Expand(-0.1f);

            Collider[] colliders = Physics.OverlapBox(bounds.center, bounds.size / 2, room.transform.rotation, Helpers.sceneLayerMask);
            if(colliders.Length > 0){
                foreach(Collider c in colliders){
                    if(c.transform.parent.gameObject.transform.parent.gameObject.Equals(room.gameObject)){
                        continue;
                    } else {
                        Debug.Log("COLLISION");
                    }
                }
            }
        }
    }
}
