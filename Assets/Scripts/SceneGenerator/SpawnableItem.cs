using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class SpawnableItem : MonoBehaviour
{
    [Range(0,100)]
    [SerializeField] int spawnChance = 10;
    bool spawned = false;
    // Start is called before the first frame update
    void Start()
    {
        Spawn();

    }

    void Update(){
        if(!spawned){
            Spawn();
        }   
    }

    void Spawn(){
        if(SceneGenerator.pseudoRandom == null) return;
        int rand = SceneGenerator.pseudoRandom.Next(0, 100);

        if(spawnChance <= rand) {
            // print("destroying");
            Destroy(gameObject);
        } else {
            spawned = true;
        }
    }
}
