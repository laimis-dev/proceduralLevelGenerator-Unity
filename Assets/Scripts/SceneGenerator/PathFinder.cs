using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
    [SerializeField] Connector start;
    [SerializeField] Connector end;
    [SerializeField] PathFinderNode cyclicConnectionPrefab;
    [SerializeField] PathFinderObject wallPrefab;

    [SerializeField] bool startOnAwake = false;
    [SerializeField] bool addWallsOnFinish = false;
    [SerializeField] float maxGScore = 15f;
    

    

    Vector2Int[] directions = new Vector2Int[] {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right
    };
        
    List<PathFinderNode> openNodes = new List<PathFinderNode>();
    List<PathFinderNode> allNodes = new List<PathFinderNode>();
    List<PathFinderNode> connectorPath = new List<PathFinderNode>();

    WaitForSeconds startup =  new WaitForSeconds(1);
    WaitForFixedUpdate fixedUpdateInterval = new WaitForFixedUpdate();
    public bool isEndFound = false;

    float minEndDistance;
    Bounds cyclicBlockBounds;
    Bounds wallBounds;
    
    
    void Start() {
        PathFinderNode pathBlock = Instantiate(cyclicConnectionPrefab);
        cyclicBlockBounds = pathBlock.GetCollider().bounds;
        minEndDistance = cyclicBlockBounds.size.x;
        Destroy(pathBlock.gameObject);

        PathFinderObject wallBlock = Instantiate(wallPrefab);
        wallBounds = wallBlock.GetCollider().bounds;
        Destroy(wallBlock.gameObject);

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
        openNodes = new List<PathFinderNode>();
        allNodes = new List<PathFinderNode>();
        connectorPath = new List<PathFinderNode>();
        yield return StartCoroutine("FindPath");
        if(!isEndFound){
            DeleteAll();
        }
        
       
    }

    IEnumerator FindPath(){
        Connector corridorConnector = start;
        Connector roomConnector = end;
        PathFinderNode startBlock = Instantiate(cyclicConnectionPrefab);
        startBlock.transform.parent = this.transform;
        startBlock.transform.position = 
            corridorConnector.transform.position + 
            start.transform.rotation * Vector3.forward;

        startBlock.transform.position = new Vector3(
                                        startBlock.transform.position.x,
                                        startBlock.transform.position.y - 0.01f,
                                        startBlock.transform.position.z);

        
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

                if(addWallsOnFinish){
                    yield return AddWallsToPath();
                }
                
                break;
            }
            openNodes.Remove(current);
            yield return StartCoroutine("ExploreNeighbours", current);
        }

        StopCoroutine("PathFinder");
    }

    void GetFinalPath(PathFinderNode current, PathFinderNode startBlock){
        PathFinderNode currentPath = current;
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
            foreach (PathFinderNode path in connectorPath) {
                if(child.position == path.transform.position){
                    isPath = true;
                }
                
            }
            if(!isPath) GameObject.Destroy(child.gameObject);
        }
    }
    
    bool AreWallsPlacableInDirection(Vector3 current, Vector2 direction){
        Vector3 checkPos = new Vector3(
                        current.x + direction.x,
                        current.y,
                        current.z + direction.y);

        foreach(PathFinderNode path in connectorPath){
            if(checkPos == path.transform.position){
                return false;
            }
        }

        return true;
    }

    List<PathFinderObject> PlaceWallsInDirection(Vector3 current, float edgeDist, string direction){
        List<PathFinderObject> walls = new List<PathFinderObject>();
        for(float j = -1 * edgeDist; j <= edgeDist; j += wallBounds.size.x){
            PathFinderObject wall = Instantiate(wallPrefab);
            wall.transform.parent = this.transform;
            walls.Add(wall);
            switch(direction){
                case "up":
                    wall.transform.position = 
                    new Vector3(
                        current.x + edgeDist,
                        current.y + wallBounds.size.y/8,
                        current.z + j);
                    break;

                case "down":
                    wall.transform.position = 
                    new Vector3(
                        current.x - edgeDist,
                        current.y + wallBounds.size.y/8,
                        current.z + j);
                    break;
                    

                case "left":
                    wall.transform.position = 
                    new Vector3(
                        current.x + j,
                        current.y + wallBounds.size.y/8,
                        current.z - edgeDist);
                    break;

                case "right":
                    wall.transform.position = 
                    new Vector3(
                        current.x + j,
                        current.y + wallBounds.size.y/8,
                        current.z + edgeDist);      
                    break;            

                default:
                    break;
            }

        }
        return walls;
    }

    public IEnumerator AddWallsToPath(){

        List<PathFinderObject> walls = new List<PathFinderObject>();
        for(int i = 0; i < connectorPath.Count; i++){
            Vector3 current = connectorPath[i].transform.position;
            //optimisation check some placement without colliders
            float numberOfWalls = (cyclicBlockBounds.size.x * 2) / wallBounds.size.x;
            float edgeDist = numberOfWalls/4 + wallBounds.size.x/2;

            

            if(AreWallsPlacableInDirection(current, new Vector2(0f, cyclicBlockBounds.size.x))){
                walls.AddRange(PlaceWallsInDirection(current, edgeDist, "right"));
            }

            if(AreWallsPlacableInDirection(current, new Vector2(0f, -cyclicBlockBounds.size.x))){
                walls.AddRange(PlaceWallsInDirection(current, edgeDist, "left"));
            }

            if(AreWallsPlacableInDirection(current, new Vector2(cyclicBlockBounds.size.x, 0f))){
                walls.AddRange(PlaceWallsInDirection(current, edgeDist, "up"));
            }

            if(AreWallsPlacableInDirection(current, new Vector2(-cyclicBlockBounds.size.x, 0f))){
                walls.AddRange(PlaceWallsInDirection(current, edgeDist, "down"));
            }
        }
            
        
        
        foreach(PathFinderObject wall in walls){
            yield return fixedUpdateInterval;
            if(wall.CheckOverlap()){
                Destroy(wall.gameObject);
            }
        }
        StopCoroutine("AddWallsToPath");
        
    }

    PathFinderNode FindLowestFScoreNode(){

        PathFinderNode minPathObject = openNodes[0];
        float minFScore = minPathObject.fScore;
        foreach(PathFinderNode path in openNodes){
            if(path.fScore < minFScore){
                minPathObject = path;
                minFScore = minPathObject.fScore;
            }
        }
        return minPathObject;
    }

    IEnumerator ExploreNeighbours(PathFinderNode from){
        int connectionWeight = 1;
        foreach(Vector2Int direction in directions){        
            float currentScore = from.gScore + connectionWeight;
            PathFinderNode pathBlock = Instantiate(cyclicConnectionPrefab);

            Bounds bounds = pathBlock.GetCollider().bounds;

            pathBlock.transform.parent = this.transform;
            pathBlock.transform.position = new Vector3(
                from.transform.position.x + direction.x * bounds.size.x,
                from.transform.position.y,
                from.transform.position.z + direction.y * bounds.size.z);
            
            
            foreach(PathFinderNode node in allNodes){
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
            if(pathBlock.CheckOverlap()){
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
        PathFinderNode pathBlock = Instantiate(cyclicConnectionPrefab);
        pathBlock.transform.parent = this.transform;
        pathBlock.transform.position = end.transform.position + end.transform.rotation * Vector3.forward;
        pathBlock.transform.position = new Vector3(
                                        pathBlock.transform.position.x,
                                        pathBlock.transform.position.y - 0.01f,
                                        pathBlock.transform.position.z);

        connectorPath.Add(pathBlock);
    }

    float DistanceToEnd(Transform from){
         return Vector3.Distance(
            from.position, 
            end.transform.position);
    }

    bool IfEndFound(Transform current){
        if(DistanceToEnd(current) <= minEndDistance){
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

}
