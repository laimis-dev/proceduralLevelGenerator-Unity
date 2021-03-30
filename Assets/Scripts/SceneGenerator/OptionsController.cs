using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionsController : MonoBehaviour {
    
    [SerializeField] TMP_InputField minRooms;
    [SerializeField] TMP_InputField maxRooms;
    [SerializeField] TMP_InputField sceneX;
    [SerializeField] TMP_InputField sceneY;
    [SerializeField] TMP_InputField roomSizeMin;
    [SerializeField] TMP_InputField roomSizeMax;
    [SerializeField] TMP_InputField corridorSizeMin;
    [SerializeField] TMP_InputField corridorSizeMax;
    [SerializeField] TMP_InputField seed;

    void Start(){
        minRooms.text = PlayerPrefsController.GetMinRooms().ToString();
        maxRooms.text = PlayerPrefsController.GetMaxRooms().ToString();
        sceneX.text = PlayerPrefsController.GetSceneSizeX().ToString();
        sceneY.text = PlayerPrefsController.GetSceneSizeY().ToString();
        roomSizeMin.text = PlayerPrefsController.GetMinRoomSize().ToString();
        roomSizeMax.text = PlayerPrefsController.GetMaxRoomSize().ToString();
        corridorSizeMin.text = PlayerPrefsController.GetMinCorridorSize().ToString();
        corridorSizeMax.text = PlayerPrefsController.GetMaxCorridorSize().ToString();
        seed.text = PlayerPrefsController.GetSeed();
        
    }

    public void SaveSettings(){
        PlayerPrefsController.SetMinRooms(Int32.Parse(minRooms.text));
        PlayerPrefsController.SetMaxRooms(Int32.Parse(maxRooms.text));
        PlayerPrefsController.SetSceneSizeX(Int32.Parse(sceneX.text));
        PlayerPrefsController.SetSceneSizeY(Int32.Parse(sceneY.text));
        PlayerPrefsController.SetMinRoomSize(Int32.Parse(roomSizeMin.text));
        PlayerPrefsController.SetMaxRoomSize(Int32.Parse(roomSizeMax.text));
        PlayerPrefsController.SetMinCorridorSize(Int32.Parse(corridorSizeMin.text));
        PlayerPrefsController.SetMaxCorridorSize(Int32.Parse(corridorSizeMax.text));
        PlayerPrefsController.SetSeed(seed.text);
    }

    public void SetDefaults(){

    }
}
