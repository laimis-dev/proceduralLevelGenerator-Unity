using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GeneratorClasses;

namespace GeneratorClasses
{
    public class LevelGenerator : MonoBehaviour
    {
        [SerializeField] bool drawGizmo = false;
        // [SerializeField] bool processMap = true;
        [SerializeField] Vector2Int sceneSize = new Vector2Int(100,100);
        [SerializeField] bool useRandomSeed = true;
        [SerializeField] string seed;
        [SerializeField] Vector2Int corridorSizeRange = new Vector2Int(3,10);
        [SerializeField] List<Room> rooms;

        Map map;
        public static System.Random pseudoRandom;
        // Start is called before the first frame update
        void Start() {
            GenerateLevel();
        }

        // Update is called once per frame
        void Update() {
            if (Input.GetMouseButtonDown(0)) {
                GenerateLevel();
            }   
        }

        private void GenerateLevel() {
            map = new Map(sceneSize.x, sceneSize.y, corridorSizeRange, rooms);
            if (useRandomSeed) {
                seed = Time.time.ToString();
            }

            pseudoRandom = new System.Random(seed.GetHashCode());
            // Debug.Log(pseudoRandom.Next(0, (int) sceneSize.x));
            Agent agent = new Agent(map);


            for(int i = 0; i < 200; i++){
                // print(agent.getPosition());
                // print(agent.getDirection());

                agent.Process();
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
}
