using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corridor : MonoBehaviour
{
    [SerializeField] List<Connector> connectors = new List<Connector>();
    [SerializeField] List<BoxCollider> colliders = new List<BoxCollider>();
    // [SerializeField] MeshCollider meshCollider;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
