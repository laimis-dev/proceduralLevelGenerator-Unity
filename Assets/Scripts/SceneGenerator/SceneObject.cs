using System.Collections;
using System.Collections.Generic;
using System;

using UnityEngine;

public class SceneObject : MonoBehaviour
{
    [SerializeField] List<BoxCollider> colliders = new List<BoxCollider>();
    public float gScore = 0;
    public float fScore = 0;
    public SceneObject instantiatedFrom;


    void Start()
    {
        
    }


    public List<BoxCollider> GetColliders(){
        return colliders;
    }

    // public int getGScore(){
    //     return gScore;
    // }

    // public int getFScore(){
    //     return fScore;
    // }

}
