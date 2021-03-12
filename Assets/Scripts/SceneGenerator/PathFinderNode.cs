using System.Collections;
using System.Collections.Generic;
using System;

using UnityEngine;

public class PathFinderNode: PathFinderObject
{
    public float gScore = 0;
    public float fScore = 0;
    public PathFinderNode instantiatedFrom;
}
