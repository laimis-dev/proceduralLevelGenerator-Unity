using UnityEngine;

public class PlayerPrefsController : MonoBehaviour {
    const string MIN_ROOMS = "MIN_ROOMS";
    const string MAX_ROOMS = "MAX_ROOMS";
    const string SCENE_SIZE_X = "SCENE_SIZE_X";
    const string SCENE_SIZE_Y = "SCENE_SIZE_Y";
    const string MIN_ROOM_SIZE = "MIN_ROOM_SIZE";
    const string MAX_ROOM_SIZE = "MAX_ROOM_SIZE";
    const string MIN_CORRIDOR_SIZE = "MIN_CORRIDOR_SIZE";
    const string MAX_CORRIDOR_SIZE = "MAX_CORRIDOR_SIZE";
    const string SEED = "SEED";

    const int MIN_SIZE = 0;
    const int MAX_SIZE = 100;

    public static void SetMinRooms(int min){
        if(min > MAX_SIZE) PlayerPrefs.SetInt(MIN_ROOMS, MAX_SIZE);
        else if(min < MIN_SIZE) PlayerPrefs.SetInt(MIN_ROOMS, MIN_SIZE);
        else PlayerPrefs.SetInt(MIN_ROOMS, min);
    }

    public static void SetMaxRooms(int max){
        if(max > MAX_SIZE) PlayerPrefs.SetInt(MAX_ROOMS, MAX_SIZE);
        else if(max < MIN_SIZE) PlayerPrefs.SetInt(MAX_ROOMS, MIN_SIZE);
        else PlayerPrefs.SetInt(MAX_ROOMS, max);
    }

    public static void SetSceneSizeX(int x){
        if(x > MAX_SIZE) PlayerPrefs.SetInt(SCENE_SIZE_X, MAX_SIZE);
        else if(x < MIN_SIZE) PlayerPrefs.SetInt(SCENE_SIZE_X, MIN_SIZE);
        else PlayerPrefs.SetInt(SCENE_SIZE_X, x);
    }

    public static void SetSceneSizeY(int y){
        if(y > MAX_SIZE) PlayerPrefs.SetInt(SCENE_SIZE_Y, MAX_SIZE);
        else if(y < MIN_SIZE) PlayerPrefs.SetInt(SCENE_SIZE_Y, MIN_SIZE);
        else PlayerPrefs.SetInt(SCENE_SIZE_Y, y);
    }

    public static void SetMinRoomSize(int min){
        if(min > MAX_SIZE) PlayerPrefs.SetInt(MIN_ROOM_SIZE, MAX_SIZE);
        else if(min < MIN_SIZE) PlayerPrefs.SetInt(MIN_ROOM_SIZE, MIN_SIZE);
        else PlayerPrefs.SetInt(MIN_ROOM_SIZE, min);
    }

    public static void SetMaxRoomSize(int max){
        if(max > MAX_SIZE) PlayerPrefs.SetInt(MAX_ROOM_SIZE, MAX_SIZE);
        else if(max < MIN_SIZE) PlayerPrefs.SetInt(MAX_ROOM_SIZE, MIN_SIZE);
        else PlayerPrefs.SetInt(MAX_ROOM_SIZE, max);
    }

    public static void SetMinCorridorSize(int min){
        if(min > MAX_SIZE) PlayerPrefs.SetInt(MIN_CORRIDOR_SIZE , MAX_SIZE);
        else if(min < MIN_SIZE) PlayerPrefs.SetInt(MIN_CORRIDOR_SIZE, MIN_SIZE);
        else PlayerPrefs.SetInt(MIN_CORRIDOR_SIZE, min);
    }

    public static void SetMaxCorridorSize(int max){
        if(max > MAX_SIZE) PlayerPrefs.SetInt(MAX_CORRIDOR_SIZE, MAX_SIZE);
        else if(max < MIN_SIZE) PlayerPrefs.SetInt(MAX_CORRIDOR_SIZE, MIN_SIZE);
        else PlayerPrefs.SetInt(MAX_CORRIDOR_SIZE, max);
    }

    public static void SetSeed(string seed){
        PlayerPrefs.SetString(SEED, seed);
    }

    

    public static int GetMinRooms(){
        return PlayerPrefs.GetInt(MIN_ROOMS);
    }

    public static int GetMaxRooms(){
        return PlayerPrefs.GetInt(MAX_ROOMS);
    }

    public static int GetSceneSizeX(){
        return PlayerPrefs.GetInt(SCENE_SIZE_X);
    }

    public static int GetSceneSizeY(){
        return PlayerPrefs.GetInt(SCENE_SIZE_Y);
    }

    public static int GetMinRoomSize(){
        return PlayerPrefs.GetInt(MIN_ROOM_SIZE);
    }

    public static int GetMaxRoomSize(){
        return PlayerPrefs.GetInt(MAX_ROOM_SIZE);
    }

    public static int GetMinCorridorSize(){
        return PlayerPrefs.GetInt(MIN_CORRIDOR_SIZE);
    }

    public static int GetMaxCorridorSize(){
        return PlayerPrefs.GetInt(MAX_CORRIDOR_SIZE);
    }

    public static string GetSeed(){
        return PlayerPrefs.GetString(SEED);
    }
}