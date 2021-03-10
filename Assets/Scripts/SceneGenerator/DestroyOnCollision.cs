using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnCollision : MonoBehaviour
{
    [SerializeField] GameObject gameObject;
    void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}
