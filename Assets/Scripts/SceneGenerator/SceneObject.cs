using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneObject : MonoBehaviour
{
    [SerializeField] List<BoxCollider> colliders = new List<BoxCollider>();
    public float gScore = 0;
    public float fScore = 0;

    void Start()
    {
        
    }


    public List<BoxCollider> getColliders(){
        return colliders;
    }

    // public int getGScore(){
    //     return gScore;
    // }

    // public int getFScore(){
    //     return fScore;
    // }
}
