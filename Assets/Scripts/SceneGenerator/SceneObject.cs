using System.Collections;
using System.Collections.Generic;
using System;

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
}
