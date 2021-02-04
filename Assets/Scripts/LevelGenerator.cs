using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] bool drawGizmo = false;
    // [SerializeField] bool processMap = true;
    [SerializeField] Vector2Int sceneSize = new Vector2Int(100,100);
    [SerializeField] bool useRandomSeed = true;
    [SerializeField] string seed;
    [SerializeField] int roomPlacementChanceModifier = 5;
    [SerializeField] int corridorPlacementChanceModifier = 5;

    [SerializeField] Vector2Int corridorSizeRange;
    [SerializeField] Vector2Int roomSizeRange;

    Agent[] agents;
    Map map;
    static System.Random pseudoRandom;
    static Vector2Int[] directions = new Vector2Int[] {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right
    };
    // Start is called before the first frame update
    void Start() {
        GenerateLevel();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) {
            GenerateLevel();
        }   
    }

    private void GenerateLevel(){
        map = new Map(sceneSize.x, sceneSize.y);
        if (useRandomSeed) {
            seed = Time.time.ToString();
        }

        pseudoRandom = new System.Random(seed.GetHashCode());
        // Debug.Log(pseudoRandom.Next(0, (int) sceneSize.x));
        Vector2Int position = new Vector2Int(
            pseudoRandom.Next(0, sceneSize.x), 
            pseudoRandom.Next(0, sceneSize.y));


        Agent agent = new Agent(
            position, 
            roomPlacementChanceModifier, 
            corridorPlacementChanceModifier,
            map);


        for(int i = 0; i < 5; i++){
            print(agent.getPosition());
            print(agent.getDirection());

            agent.Process();
        }


        
        
        // for (int x = 0; x < width; x ++) {
        //     for (int y = 0; y < height; y ++) {
        //         if (x == 0 || x == width-1 || y == 0 || y == height -1) {
        //             map[x,y] = 1;
        //         } else {
        //             map[x,y] = ( < randomFillPercent)? 1: 0;
        //         }
        //     }
        // }
    }

    class Map {
        private int[,] map;
        private int width;
        private int height;

        public Map(int width, int height){
            this.width = width;
            this.height = height;
            this.map = new int[width, height];
        }

        public int[,] getMap(){
            return map;
        }

        public int getWidth(){
            return width;
        }

        public int getHeight(){
            return height;
        }

        public void setMapNode(int x, int y, int val){
            this.map[x,y] = val;
        }

        public int getMapNode(int x, int y){
            return this.map[x,y];
        }
    }

    class Agent {
        private Vector2Int position;
        private int roomPlacementChanceModifier;
        private int corridorPlacementChanceModifier;
        private int roomPlacementChance;
        private int corridorPlacementChance;
        private Vector2Int movementDirection;
        private Map map;

        public Agent(Vector2Int startPosition, int roomChanceMod, int corChanceMod, Map map){
            this.position = startPosition;
            this.roomPlacementChanceModifier = roomChanceMod;
            this.corridorPlacementChanceModifier = corChanceMod;
            this.roomPlacementChance = 0;
            this.corridorPlacementChance = 0;
            this.map = map;
            RandomDirection();
        }

        public Agent(Vector2Int startPosition, int roomChanceMod, int corChanceMod, Map map, Vector2Int direction){
            this.position = startPosition;
            this.roomPlacementChanceModifier = roomChanceMod;
            this.corridorPlacementChanceModifier = corChanceMod;
            this.roomPlacementChance = 0;
            this.corridorPlacementChance = 0;
            this.map = map;
            this.movementDirection = direction;
        }

        public void Process(){
            Vector2Int newPosition = this.position + this.movementDirection; 
            // print("----- new position");
            // print(newPosition);
            // print(map.getHeight());
            // print(map.getWidth());
            // print("------");
            // print(newPosition.x == 0);
            // print(newPosition.x == map.getWidth()-1);
            // print(newPosition.y == 0);
            // print(newPosition.y == map.getHeight()-1);

            //TODO refactor this garbage
            if (newPosition.x == 0 || newPosition.x == map.getWidth()-1 
             || newPosition.y == 0 || newPosition.y == map.getHeight()-1) {
                RandomDirection();
                newPosition = this.position + this.movementDirection; 
                if (newPosition.x == 0 || newPosition.x == map.getWidth()-1 
                || newPosition.y == 0 || newPosition.y == map.getHeight()-1) {
                    print("passed 2");
                    this.position = newPosition;
                } else {
                    print("passed 3");
                    RandomDirection();
                    newPosition = this.position + this.movementDirection; 
                    this.position = newPosition;
                }
            } else {
                print("passed 1");
                this.position = newPosition;                
            }
            
            map.setMapNode(this.position.x, this.position.y, 1);
            // map[this.position.x, this.position.y] = 1;

            if(pseudoRandom.Next(0,100) < roomPlacementChance){
                roomPlacementChance = 0;
                PlaceRoom();
            } else {
                roomPlacementChance += roomPlacementChanceModifier;
            }

            if(pseudoRandom.Next(0,100) < corridorPlacementChance){
                corridorPlacementChance = 0;
                RandomDirection();
                PlaceCorridor();
            } else {
                corridorPlacementChance += corridorPlacementChanceModifier;
            }

        }

        private void PlaceRoom(){
            print("ROOM PLACED");
        }

        private void PlaceCorridor(){
            print("CORRIDOR PLACED");
        }

        public void RandomDirection(){
            print("direction changed");
            this.movementDirection = directions[pseudoRandom.Next(0,3)];
        }

        public Vector2 getPosition(){
            return this.position;
        }

        public Vector2 getDirection(){
            return movementDirection;
        }

    }


    void OnDrawGizmos() {
        if(!drawGizmo) return;
        int width = sceneSize.x;
        int height = sceneSize.y;

        if (map != null) {
            for (int x = 0; x < width; x ++) {
                for (int y = 0; y < height; y ++) {
                    Gizmos.color = (map.getMapNode(x,y) == 1)?Color.black:Color.white;
                    Vector3 pos = new Vector3(-width/2 + x + .5f,0, -height/2 + y+.5f);
                    Gizmos.DrawCube(pos,Vector3.one);
                }
            }
        }
    }
}
