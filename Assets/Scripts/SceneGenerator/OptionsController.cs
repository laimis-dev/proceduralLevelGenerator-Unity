using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Utils;

public class OptionsController : MonoBehaviour
{

    [SerializeField] TMP_InputField minRooms;
    [SerializeField] TMP_InputField maxRooms;
    [SerializeField] TMP_InputField sceneX;
    [SerializeField] TMP_InputField sceneY;
    [SerializeField] TMP_InputField roomSizeMin;
    [SerializeField] TMP_InputField roomSizeMax;
    [SerializeField] TMP_InputField corridorSizeMin;
    [SerializeField] TMP_InputField corridorSizeMax;
    [SerializeField] TMP_InputField seed;
    [SerializeField] TMP_Text saved;

    void Start()
    {
        int hasPlayed = PlayerPrefs.GetInt("HasPlayed");

        if (hasPlayed == 0)
        {
            PlayerPrefs.SetInt("HasPlayed", 1);
            SetDefaults();
        }

        saved.gameObject.SetActive(false);
        SetText();
    }

    public void SaveSettings()
    {
        PlayerPrefsController.SetMinRooms(Int32.Parse(minRooms.text));
        PlayerPrefsController.SetMaxRooms(Int32.Parse(maxRooms.text));
        PlayerPrefsController.SetSceneSizeX(Int32.Parse(sceneX.text));
        PlayerPrefsController.SetSceneSizeY(Int32.Parse(sceneY.text));
        PlayerPrefsController.SetMinRoomSize(Int32.Parse(roomSizeMin.text));
        PlayerPrefsController.SetMaxRoomSize(Int32.Parse(roomSizeMax.text));
        PlayerPrefsController.SetMinCorridorSize(Int32.Parse(corridorSizeMin.text));
        PlayerPrefsController.SetMaxCorridorSize(Int32.Parse(corridorSizeMax.text));
        PlayerPrefsController.SetSeed(seed.text);
        SetText();
        StartCoroutine("ShowSavedMessage");
    }

    IEnumerator ShowSavedMessage()
    {
        saved.gameObject.SetActive(true);
        yield return Helpers.startup;
        saved.gameObject.SetActive(false);
    }

    public void SetDefaults()
    {
        PlayerPrefsController.SetMinRooms(10);
        PlayerPrefsController.SetMaxRooms(20);
        PlayerPrefsController.SetSceneSizeX(1000);
        PlayerPrefsController.SetSceneSizeY(1000);
        PlayerPrefsController.SetMinRoomSize(0);
        PlayerPrefsController.SetMaxRoomSize(800);
        PlayerPrefsController.SetMinCorridorSize(0);
        PlayerPrefsController.SetMaxCorridorSize(800);
        PlayerPrefsController.SetSeed("");
        SetText();
        StartCoroutine("ShowSavedMessage");
    }

    private void SetText()
    {
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
}
