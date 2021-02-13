using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneObject : MonoBehaviour
{
    [SerializeField] List<BoxCollider> colliders = new List<BoxCollider>();


    void Start()
    {
        
    }


    public List<BoxCollider> getColliders(){
        return colliders;
    }
}
