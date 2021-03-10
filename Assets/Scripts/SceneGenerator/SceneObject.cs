using System.Collections;
using System.Collections.Generic;
using System;

using UnityEngine;

public class SceneObject : MonoBehaviour
{
    [SerializeField] BoxCollider collider;
    public float gScore = 0;
    public float fScore = 0;
    public SceneObject instantiatedFrom;


    void Start()
    {
        
    }


    public BoxCollider GetCollider(){
        return collider;
    }

    // public int getGScore(){
    //     return gScore;
    // }

    // public int getFScore(){
    //     return fScore;
    // }

}
