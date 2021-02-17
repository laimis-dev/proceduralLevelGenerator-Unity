using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorridorConnector : MonoBehaviour
{
    [SerializeField] Connector start;
    [SerializeField] Connector end;
    [SerializeField] SceneObject cyclicConnectionPrefab;
    [SerializeField] GameObject wallPrefab;

    LayerMask sceneLayerMask;

    Vector2Int[] directions = new Vector2Int[] {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right
    };
        
    List<SceneObject> nodes = new List<SceneObject>();
    List<SceneObject> allNodes = new List<SceneObject>();
    List<SceneObject> connectorPath = new List<SceneObject>();
    Queue<SceneObject> queue = new Queue<SceneObject>();
        WaitForSeconds startup =  new WaitForSeconds(1);
        WaitForFixedUpdate fixedUpdateInterval = new WaitForFixedUpdate();

    
    void Start() {

        print(start.transform.position);
        print(end.transform.position);
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
        startBlock.transform.position = 
            corridorConnector.transform.position + start.transform.rotation * Vector3.forward;
        startBlock.transform.parent = this.transform;
        // queue.Enqueue(pathBlock);
        startBlock.fScore = DistanceToEnd(startBlock.transform);
        nodes.Add(startBlock);
        allNodes.Add(startBlock);
        // while(queue.Count > 0){
        while(nodes.Count > 0){
            // var searchCenter = queue.Dequeue();
            var current = FindLowestFScoreNode();

            if(IfEndFound(current.transform)){
                PlaceEndPath();
                nodes.Add(startBlock);
                nodes.Add(current);
                allNodes.Add(current);
                GetFinalPath(current, startBlock);
                
                DeleteUnneededPaths();
                yield return AddWallsToPath();
                break;
            }
            nodes.Remove(current);
            yield return StartCoroutine("ExploreNeighbours", current);
            
        }

        StopCoroutine("PathFinder");
    }

    void GetFinalPath(SceneObject current, SceneObject startBlock){
        SceneObject currentPath = current;
        connectorPath.Add(currentPath);
        while(currentPath != startBlock){
            currentPath = currentPath.instantiatedFrom;
            connectorPath.Add(currentPath);
        }
        connectorPath.Add(startBlock);
    }

    void DeleteUnneededPaths(){
        foreach (Transform child in this.transform) {
            bool isPath = false;
            foreach (SceneObject path in connectorPath) {
                if(child.position == path.transform.position){
                    isPath = true;
                }  
                
            }
            if(!isPath) GameObject.Destroy(child.gameObject);
        }
    }

    IEnumerator AddWallsToPath(){
        bool xSign = Mathf.Abs(start.transform.position.x - end.transform.position.x) % 2 == 1;
        bool zSign = Mathf.Abs(start.transform.position.z - end.transform.position.z) % 2 == 1;

        for(int i = 0; i < connectorPath.Count; i++){
            Vector3 current = connectorPath[i].transform.position;

            foreach(Vector2Int direction in directions){   
                Vector3 checkPos = new Vector3(
                    current.x + direction.x * 2f,
                    current.y,
                    current.z + direction.y * 2f);
                
                if(i == 0){
                    

                    if(!xSign && zSign){
                        Vector3 b = end.transform.position
                                + end.transform.rotation * Vector3.forward * -2f;
                        if(checkPos == b){
                            continue;
                        }
                    } else {
                        Vector3 b = end.transform.position + end.transform.rotation * Vector3.forward 
                                    + end.transform.rotation * Vector3.forward * -2f;
                        if(checkPos == b){
                            continue;
                        }
                    }
                    

                    // b = end.transform.position + end.transform.rotation * Vector3.forward 
                    //             + end.transform.rotation * Vector3.forward * 2f;
                    // if(checkPos == b){
                    //     continue;
                    // }
                } 

                
                 
                if(i == connectorPath.Count - 2 ){

                    Vector3 b = start.transform.position + start.transform.rotation * Vector3.forward 
                                + start.transform.rotation * Vector3.forward * -2f;
                    if(checkPos == b){
                        continue;
                    }

                } 

                if(i == connectorPath.Count - 1 ){
                    Vector3 b = start.transform.position + start.transform.rotation * Vector3.forward 
                                + start.transform.rotation * Vector3.forward * -2f;
                    if(checkPos == b){
                        continue;
                    }
                } 

                bool isWallPlaceable = true;
                foreach(SceneObject path in connectorPath){
                    if(checkPos == path.transform.position){
                        isWallPlaceable = false;
                    }
                }

                if(!isWallPlaceable) continue;

                GameObject wall = Instantiate(wallPrefab);
                wall.transform.parent = this.transform;
                yield return fixedUpdateInterval;
                checkPos = new Vector3(
                    current.x + direction.x * 1.5f ,
                    current.y,
                    current.z + direction.y * 1.5f);

                
                if(direction == Vector2Int.up || direction == Vector2Int.down){
                    wall.transform.rotation = Quaternion.Euler(0, 90f, 0);
                }

                wall.transform.position = checkPos;
            }
        }
        StopCoroutine("AddWallsToPath");
    }

    // void AddWallsToPath(){
    //     for(int i = 0; i < connectorPath.Count; i++){
    //         Vector3 current = connectorPath[i].transform.position;

    //         foreach(Vector2Int direction in directions){   
    //             Vector3 checkPos = new Vector3(
    //                 current.x + direction.x * 2f,
    //                 current.y,
    //                 current.z + direction.y * 2f);
                
    //             if(i == 0){
    //                 Vector3 b = end.transform.position + end.transform.rotation * Vector3.forward 
    //                             + end.transform.rotation * Vector3.back * 2f;
    //                 if(checkPos == b){
    //                     continue;
    //                 }
    //             } 

    //             if(i == connectorPath.Count - 1){
    //                 Vector3 b = start.transform.position + start.transform.rotation * Vector3.forward 
    //                             + start.transform.rotation * Vector3.back * 2f;
    //                 if(checkPos == b){
    //                     continue;
    //                 }
    //             } 

    //             bool isWallPlaceable = true;
    //             foreach(SceneObject path in connectorPath){
    //                 if(checkPos == path.transform.position){
    //                     isWallPlaceable = false;
    //                 }
    //             }

    //             if(!isWallPlaceable) continue;

    //             GameObject wall = Instantiate(wallPrefab);
    //             wall.transform.parent = this.transform;

    //             checkPos = new Vector3(
    //                 current.x + direction.x * 1.5f ,
    //                 current.y,
    //                 current.z + direction.y * 1.5f);

                
    //             if(direction == Vector2Int.up || direction == Vector2Int.down){
    //                 wall.transform.rotation = Quaternion.Euler(0, 90f, 0);
    //             }

    //             wall.transform.position = checkPos;
    //         }
    //     }
    // }

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
            float currentScore = from.gScore + connectionWeight;
            SceneObject pathBlock = Instantiate(cyclicConnectionPrefab);
            pathBlock.transform.parent = this.transform;
            pathBlock.transform.position = new Vector3(
                from.transform.position.x + direction.x * from.transform.localScale.x,
                from.transform.position.y,
                from.transform.position.z + direction.y * from.transform.localScale.z);
            
            
            foreach(SceneObject node in allNodes){
                if(pathBlock.transform.position == node.transform.position){
                    if(currentScore < node.gScore){
                        Destroy(pathBlock.gameObject);
                        pathBlock = null;

                        node.instantiatedFrom = from;
                        node.gScore = currentScore;
                        node.fScore = currentScore + DistanceToEnd(node.transform);
                        if(nodes.Contains(node)){
                            nodes.Add(node);
                        }
                        break;
                    }
                    
                }
            }

            if(pathBlock == null) continue;

            yield return fixedUpdateInterval;
            if(CheckOverlap(pathBlock)){
                Destroy(pathBlock.gameObject);
            } else {
                pathBlock.instantiatedFrom = from;
                pathBlock.gScore = currentScore;
                pathBlock.fScore = currentScore + DistanceToEnd(pathBlock.transform);
                nodes.Add(pathBlock);
                allNodes.Add(pathBlock);
            }
        }
        StopCoroutine("ExploreNeighbours");
    }

    void PlaceEndPath() {
        bool xSign = Mathf.Abs(start.transform.position.x - end.transform.position.x) % 2 == 1;
        bool zSign = Mathf.Abs(start.transform.position.z - end.transform.position.z) % 2 == 1;
        print(xSign);
        print(zSign);
        
        if(!xSign && zSign){
            SceneObject pathBlock1 = Instantiate(cyclicConnectionPrefab);
            pathBlock1.transform.position = end.transform.position;
            pathBlock1.transform.parent = this.transform;
            connectorPath.Add(pathBlock1);

            // SceneObject pathBlock2 = Instantiate(cyclicConnectionPrefab);
            // pathBlock2.transform.position = pathBlock1.transform.position + end.transform.rotation * Vector3.forward * 2f;
            // pathBlock2.transform.parent = this.transform;
            // connectorPath.Add(pathBlock2);
        }


        if(!xSign && !zSign){
            
            SceneObject pathBlock1 = Instantiate(cyclicConnectionPrefab);
            pathBlock1.transform.position = end.transform.position + end.transform.rotation * Vector3.forward;
            pathBlock1.transform.parent = this.transform;
            connectorPath.Add(pathBlock1);

            // SceneObject pathBlock2 = Instantiate(cyclicConnectionPrefab);
            // pathBlock2.transform.position = pathBlock1.transform.position + end.transform.rotation * Vector3.forward;
            // pathBlock2.transform.parent = this.transform;
            // connectorPath.Add(pathBlock2);


        }
        
        
        
    }

    float DistanceToEnd(Transform from){
         return Vector3.Distance(
            from.position, 
            end.transform.position);
    }

    bool IfEndFound(Transform current){
        // print(DistanceToEnd(current));
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



     Collider CheckOverlap(SceneObject sceneObject){
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
                        return c;
                    }
                }
            }
        }

        return null;
    }

}
