using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connector : MonoBehaviour
{
    [SerializeField] bool drawGizmo = true;
    [SerializeField] GameObject door;
    public bool isConnected = false;
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

    public void ProcessDoor(){
        print("---------");
        print(this.transform.parent.gameObject.transform.parent.gameObject);
        print(isConnected);

        if(!isConnected){
            print("door closed");
            door.SetActive(true);
        } else {
            print("door open");
            door.SetActive(false);
        }
    }
}
