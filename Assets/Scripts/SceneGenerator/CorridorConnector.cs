using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorridorConnector : MonoBehaviour
{
    [SerializeField] Connector start;
    [SerializeField] Connector end;
    [SerializeField] SceneObject cyclicConnectionPrefab;

    LayerMask sceneLayerMask;

    Vector2Int[] directions = new Vector2Int[] {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right
    };
        
    List<SceneObject> nodes = new List<SceneObject>();
    List<SceneObject> connectorPath = new List<SceneObject>();
    Queue<SceneObject> queue = new Queue<SceneObject>();
        WaitForSeconds startup =  new WaitForSeconds(1);
        WaitForFixedUpdate fixedUpdateInterval = new WaitForFixedUpdate();

    
    void Start() {
        sceneLayerMask = LayerMask.GetMask("SceneColliders");
        StartCoroutine("PathFinder");
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetMouseButtonDown(0)) {

            StartCoroutine("PathFinder");
        }   
    }



    // void ConnectEmptyConnectors(){
    //     foreach(Connector corridorConnector in availableCorridorConnectors){
    //         foreach(Connector roomConnector in availableRoomConnectors){
    //             float distanceBetweenConnectors = Vector3.Distance(
    //                 corridorConnector.transform.position, 
    //                 roomConnector.transform.position);

    //             if(distanceBetweenConnectors <= cyclicConnectionRange){
    //                 PathFinder(corridorConnector, roomConnector);
    //             }
    //         }
    //     }
    // }

    IEnumerator PathFinder(){
        Connector corridorConnector = start;
        Connector roomConnector = end;
        SceneObject startBlock = Instantiate(cyclicConnectionPrefab);
        startBlock.transform.position = corridorConnector.transform.position;
        startBlock.transform.parent = this.transform;
        // queue.Enqueue(pathBlock);
        startBlock.fScore = DistanceToEnd(startBlock.transform);
        nodes.Add(startBlock);
        // while(queue.Count > 0){
        while(nodes.Count > 0){
            // var searchCenter = queue.Dequeue();
            var current = FindLowestFScoreNode();

            if(IfEndFound(current.transform)){
                PlaceEndPath();
                nodes.Add(current);
                
                SceneObject currentPath = current;
                connectorPath.Add(currentPath);
                while(currentPath != startBlock){
                    currentPath = currentPath.instantiatedFrom;
                    connectorPath.Add(currentPath);
                }

                foreach (Transform child in this.transform) {
                    bool isPath = false;
                    foreach (SceneObject path in connectorPath) {
                        if(child.position == path.transform.position){
                            isPath = true;
                        }
                        
                    }
                    if(!isPath) GameObject.Destroy(child.gameObject);
                }
                break;
            }
            nodes.Remove(current);
            yield return StartCoroutine("ExploreNeighbours", current);
            
        }

        StopCoroutine("PathFinder");
    }

    SceneObject FindLowestFScoreNode(){

        SceneObject minPathObject = nodes[0];
        float minFScore = minPathObject.fScore;
        foreach(SceneObject path in nodes){
            if(path.fScore < minFScore){
                minPathObject = path;
                minFScore = minPathObject.fScore;
            }
        }
        return minPathObject;
    }

    IEnumerator ExploreNeighbours(SceneObject from){
        int connectionWeight = 1;
        foreach(Vector2Int direction in directions){        
            SceneObject pathBlock = Instantiate(cyclicConnectionPrefab);
            pathBlock.transform.parent = this.transform;
            pathBlock.transform.position = new Vector3(
                from.transform.position.x + direction.x * from.transform.localScale.x,
                from.transform.position.y,
                from.transform.position.z + direction.y * from.transform.localScale.z);
            
            if(nodes.Contains(pathBlock)){
                Destroy(pathBlock.gameObject);
                continue;
            }

            yield return fixedUpdateInterval;
            if(CheckOverlap(pathBlock)){
                Destroy(pathBlock.gameObject);
            } else {
                pathBlock.instantiatedFrom = from;
                pathBlock.gScore = from.gScore + connectionWeight;
                pathBlock.fScore = DistanceToEnd(pathBlock.transform);
                nodes.Add(pathBlock);
            }
        }
        StopCoroutine("ExploreNeighbours");
    }

    void PlaceEndPath() {
        SceneObject pathBlock = Instantiate(cyclicConnectionPrefab);
        pathBlock.transform.position = end.transform.position;

        pathBlock = Instantiate(cyclicConnectionPrefab);
        pathBlock.transform.position = end.transform.position + end.transform.rotation * Vector3.forward * 2f;
        
    }

    float DistanceToEnd(Transform from){
         return Vector3.Distance(
            from.position, 
            end.transform.position);
    }

    bool IfEndFound(Transform current){
        print(DistanceToEnd(current));
        if(DistanceToEnd(current) <= 3f){
            print("stop");
            return true;
            
        } else {
            return false;
        }
    }







    // void CleanUp(){
    //     generatedCorridors.Clear();
    //     generatedRooms.Clear();
    //     availableRoomConnectors.Clear();
    //     availableCorridorConnectors.Clear();
    //     foreach (Transform child in this.transform) {
    //         GameObject.Destroy(child.gameObject);
    //     }
    // }



     bool CheckOverlap(SceneObject sceneObject){
        List<BoxCollider> objectColliders = sceneObject.getColliders();
        foreach(BoxCollider boxCollider in objectColliders){
            Bounds bounds = boxCollider.bounds;
            bounds.Expand(-0.1f);

            Collider[] colliders = Physics.OverlapBox(boxCollider.transform.position, bounds.size / 2, boxCollider.transform.rotation, sceneLayerMask);
            if(colliders.Length > 0){
                foreach(Collider c in colliders){
                    if(c.transform.parent.gameObject.transform.parent.gameObject.Equals(sceneObject.gameObject)){
                        continue;
                    } else {
                        return true;
                    }
                }
            }
        }

        return false;
    }

}
