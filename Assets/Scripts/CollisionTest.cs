using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionTest : MonoBehaviour
{
    [SerializeField] Room room;

    LayerMask sceneLayerMask;
    // Start is called before the first frame update
    void Start()
    {
        sceneLayerMask = LayerMask.GetMask("SceneColliders");

    }

    // Update is called once per frame
    void Update()
    {
        List<BoxCollider> roomColliders = room.getColliders();
        foreach(BoxCollider roomCollider in roomColliders){
            Bounds bounds = roomCollider.bounds;
            bounds.Expand(-0.1f);

            Collider[] colliders = Physics.OverlapBox(bounds.center, bounds.size / 2, room.transform.rotation, sceneLayerMask);
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
