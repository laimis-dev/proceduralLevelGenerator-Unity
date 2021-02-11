using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corridor : MonoBehaviour
{
    [SerializeField] List<Connector> connectors = new List<Connector>();
    [SerializeField] List<BoxCollider> colliders = new List<BoxCollider>();

    [SerializeField] bool testCollision = false;
    // [SerializeField] MeshCollider meshCollider;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(testCollision){
            LayerMask sceneLayerMask;
            sceneLayerMask = LayerMask.GetMask("SceneColliders");

            foreach(BoxCollider roomCollider in colliders){
                Bounds bounds = roomCollider.bounds;
                bounds.Expand(-0.1f);

                Collider[] colliders = Physics.OverlapBox(bounds.center, bounds.size / 2, this.transform.rotation, sceneLayerMask);
                if(colliders.Length > 0){
                    foreach(Collider c in colliders){
                        if(c.transform.parent.gameObject.transform.parent.gameObject.Equals(this.gameObject)){
                            continue;
                        } else {
                            Debug.Log("COLLISION");
                        }
                    }
                }
            }
        }
        
    }

    // public Bounds roomBounds(){
    //     return meshCollider.bounds;
    // }

    public List<Connector> getConnectors(){
        return connectors;
    }

    public List<BoxCollider> getColliders(){
        return colliders;
    }
}
