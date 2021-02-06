using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connector : MonoBehaviour
{
    [SerializeField] bool drawGizmo = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDrawGizmos(){
        if(drawGizmo){
            Ray ray = new Ray(transform.position, transform.rotation * Vector3.forward);
            Gizmos.color = Color.red;
            Gizmos.DrawRay(ray);
        }
    }
}
