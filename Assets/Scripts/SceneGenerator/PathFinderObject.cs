using System.Collections;
using System.Collections.Generic;
using System;

using UnityEngine;

public class PathFinderObject: MonoBehaviour
{
    [SerializeField] BoxCollider collider;

    public BoxCollider GetCollider(){
        return collider;
    }
}
