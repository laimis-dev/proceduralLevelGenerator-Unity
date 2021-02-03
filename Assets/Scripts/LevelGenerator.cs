using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] bool drawGizmo = false;
    // [SerializeField] bool processMap = true;
    [SerializeField] Vector2 sceneSize;
    [SerializeField] bool useRandomSeed = true;
    [SerializeField] string seed;
    [SerializeField] int roomPlacementChanceModifier = 5;
    [SerializeField] int corridorPlacementChanceModifier = 5;

    int[,] map;
    Agent[] agents;
    System.Random pseudoRandom;

    Vector2[] directions = new Vector2[] {
        Vector2.up,
        Vector2.down,
        Vector2.left,
        Vector2.right
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
        if (useRandomSeed) {
            seed = Time.time.ToString();
        }

        pseudoRandom = new System.Random(seed.GetHashCode());
        // Debug.Log(pseudoRandom.Next(0, (int) sceneSize.x));
        Vector2 position = new Vector2(
            pseudoRandom.Next(0, (int) sceneSize.x), 
            pseudoRandom.Next(0, (int) sceneSize.y));

        //if you need the vector to have a specific length:
        Vector2 direction = directions[pseudoRandom.Next(0,3)];

        Agent agent = new Agent(
            position, 
            roomPlacementChanceModifier, 
            corridorPlacementChanceModifier, 
            direction);

        print(agent.getPosition());
        print(agent.getDirection());
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

    class Agent {
        private Vector2 position;
        private int roomPlacementChanceModifier;
        private int corridorPlacementChanceModifier;
        private int roomPlacementChance;
        private int corridorPlacementChance;
        private Vector2 movementDirection;

        public Agent(Vector2 startPosition, int roomChanceMod, int corChanceMod, Vector2 direction){
            this.position = startPosition;
            this.roomPlacementChanceModifier = roomChanceMod;
            this.corridorPlacementChanceModifier = corChanceMod;
            this.roomPlacementChance = 0;
            this.corridorPlacementChance = 0;
            this.movementDirection = direction;
        }

        public void Process(){
            
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

        // if (map != null) {
        //     for (int x = 0; x < width; x ++) {
        //         for (int y = 0; y < height; y ++) {
        //             Gizmos.color = (map[x,y] == 1)?Color.black:Color.white;
        //             Vector3 pos = new Vector3(-width/2 + x + .5f,0, -height/2 + y+.5f);
        //             Gizmos.DrawCube(pos,Vector3.one);
        //         }
        //     }
        // }
    }
}
