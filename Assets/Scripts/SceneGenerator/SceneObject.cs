using System.Collections;
using System.Collections.Generic;
using System;
using Utils;

using UnityEngine;

public class SceneObject : MonoBehaviour
{
    [SerializeField] string roomName;

    [SerializeField] List<Connector> connectors = new List<Connector>();
    [SerializeField] List<BoxCollider> colliders = new List<BoxCollider>();

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public List<Connector> GetConnectors(){
        return connectors;
    }

    public List<BoxCollider> GetColliders(){
        return colliders;
    }

    public string GetName(){
        return roomName;
    }

    public bool CheckOverlap(){
        List<BoxCollider> objectColliders = this.GetColliders();
        foreach(BoxCollider objectCollider in objectColliders){
            Bounds bounds = objectCollider.bounds;
            bounds.Expand(-0.1f);

            Collider[] colliders = Physics.OverlapBox(objectCollider.transform.position, bounds.size / 2, objectCollider.transform.rotation, Helpers.sceneLayerMask);
            if(colliders.Length > 0){
                foreach(Collider c in colliders){
                    if(Helpers.GetRootGameObject(c.transform).Equals(gameObject)){
                        continue;
                    } else {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public float GetSize(){
        List<BoxCollider> objectColliders = this.GetColliders();
        float size = 0;
        foreach(BoxCollider objectCollider in objectColliders){
            Bounds bounds = objectCollider.bounds;
            size += bounds.size.x * bounds.size.z;
        }
        
        return size;
    }
}
