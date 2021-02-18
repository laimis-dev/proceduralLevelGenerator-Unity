using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorridorConnector : MonoBehaviour
{
    [SerializeField] Connector start;
    [SerializeField] Connector end;
    [SerializeField] SceneObject cyclicConnectionPrefab;
    [SerializeField] SceneObject wallPrefab;

    [SerializeField] bool startOnAwake = false;
    [SerializeField] float maxGScore = 15f;
    

    LayerMask sceneLayerMask;

    Vector2Int[] directions = new Vector2Int[] {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right
    };
        
    List<SceneObject> openNodes = new List<SceneObject>();
    List<SceneObject> allNodes = new List<SceneObject>();
    List<SceneObject> connectorPath = new List<SceneObject>();

    WaitForSeconds startup =  new WaitForSeconds(1);
    WaitForFixedUpdate fixedUpdateInterval = new WaitForFixedUpdate();
    public bool isEndFound = false;
    
    
    void Start() {
        sceneLayerMask = LayerMask.GetMask("SceneColliders");
        if(startOnAwake){
            StartCoroutine(StartConnecting());
        }
        
    }

    void Update() {
        if (Input.GetMouseButtonDown(0) && startOnAwake) {

            StartCoroutine(StartConnecting());
        }   
    }

    public void SetConnectionPoints(Connector start, Connector end){
        this.start = start;
        this.end = end;
    }

    public void SetMaxGScore(float score){
        maxGScore = score;
    }

    public IEnumerator StartConnecting(){
        isEndFound = false;
        openNodes = new List<SceneObject>();
        allNodes = new List<SceneObject>();
        connectorPath = new List<SceneObject>();
        yield return StartCoroutine("PathFinder");
        if(!isEndFound){
            DeleteAll();
        }
        
       
    }

    IEnumerator PathFinder(){
        Connector corridorConnector = start;
        Connector roomConnector = end;
        SceneObject startBlock = Instantiate(cyclicConnectionPrefab);
        startBlock.transform.parent = this.transform;
        startBlock.transform.position = 
            corridorConnector.transform.position + 
            start.transform.rotation * Vector3.forward;

        
        startBlock.fScore = DistanceToEnd(startBlock.transform);
        openNodes.Add(startBlock);
        allNodes.Add(startBlock);
        // Debug.Break();
        while(openNodes.Count > 0){
            var current = FindLowestFScoreNode();
            if(current.gScore > maxGScore) break;

            if(IfEndFound(current.transform)){
                isEndFound = true;
                PlaceEndPath();
                openNodes.Add(startBlock);
                openNodes.Add(current);
                allNodes.Add(current);
                GetFinalPath(current, startBlock);
                
                DeleteUnneededPaths();
                yield return AddWallsToPath();
                break;
            }
            openNodes.Remove(current);
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
        for(int i = 0; i < connectorPath.Count; i++){
            Vector3 current = connectorPath[i].transform.position;

            foreach(Vector2Int direction in directions){   
                //optimisation check some placement without colliders
                Vector3 checkPos = new Vector3(
                        current.x + direction.x * 2f,
                        current.y,
                        current.z + direction.y * 2f);
                bool isWallPlaceable = true;
                foreach(SceneObject path in connectorPath){
                    if(checkPos == path.transform.position){
                        isWallPlaceable = false;
                    }
                }

                if(!isWallPlaceable) continue;

                for(int j = 1; j < 3; j++){
                    SceneObject wall = Instantiate(wallPrefab);
                    wall.transform.parent = this.transform;
                    if(direction == Vector2Int.up || direction == Vector2Int.down){
                        wall.transform.position = 
                            new Vector3(
                                current.x + direction.x * 1.5f + 2f - j - 0.5f,
                                current.y,
                                current.z + direction.y * 1.5f);
                    } else {
                        wall.transform.position = 
                            new Vector3(
                                current.x + direction.x * 1.5f,
                                current.y,
                                current.z + direction.y * 1.5f + 2f - j - 0.5f);
                    }

                    yield return fixedUpdateInterval;
                    if(CheckOverlap(wall)){
                        Destroy(wall.gameObject);
                    }
                }
            }
        }
        StopCoroutine("AddWallsToPath");
    }

    SceneObject FindLowestFScoreNode(){

        SceneObject minPathObject = openNodes[0];
        float minFScore = minPathObject.fScore;
        foreach(SceneObject path in openNodes){
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
                        if(openNodes.Contains(node)){
                            openNodes.Add(node);
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
                openNodes.Add(pathBlock);
                allNodes.Add(pathBlock);
            }
        }
        StopCoroutine("ExploreNeighbours");
    }

    void PlaceEndPath() {
        SceneObject pathBlock = Instantiate(cyclicConnectionPrefab);
        pathBlock.transform.parent = this.transform;
        pathBlock.transform.position = end.transform.position + end.transform.rotation * Vector3.forward;

        connectorPath.Add(pathBlock);
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







    void DeleteAll(){
        foreach (Transform child in this.transform) {
            GameObject.Destroy(child.gameObject);
        }
    }



     Collider CheckOverlap(SceneObject sceneObject){
        List<BoxCollider> objectColliders = sceneObject.getColliders();
        foreach(BoxCollider boxCollider in objectColliders){
            // print(boxCollider);
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
