using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connector : MonoBehaviour
{
    [SerializeField] bool drawGizmo = true;
    [SerializeField] GameObject door;
    [SerializeField] Connector connectedTo;
    public int distanceFromStart = 0;

    // Start is called before the first frame update
    void Start()
    {
        ProcessDoor();
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

    public void ProcessDoor(){
        if(connectedTo == null){
            door.SetActive(true);
        } else {
            door.SetActive(false);
        }
    }

    public void SetConnectedTo(Connector connectedTo){
        this.connectedTo = connectedTo;
        ProcessDoor();
    }

    public Connector GetConnectedTo(){
        return connectedTo;
    }

}
