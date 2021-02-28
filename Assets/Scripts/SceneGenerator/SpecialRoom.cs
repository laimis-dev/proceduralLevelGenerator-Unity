using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialRoom : Room
{
    [SerializeField] string roomName;

    [SerializeField] int minSpawnDistance = 1;

    [Range(0,100)]
    [SerializeField] int spawnChance = 10;
    [SerializeField] int maxAmountPerScene = 1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float GetMinSpawnDistance(){
        return minSpawnDistance;
    }

    public int GetSpawnChance(){
        return spawnChance;
    }

    public int GetMaxAmountPerScene(){
        return maxAmountPerScene;
    }

    public string GetName(){
        return roomName;
    }
}
