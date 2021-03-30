using UnityEngine;

public class test : MonoBehaviour {
    void Start(){
        PlayerPrefsController.SetMaxRooms(2);
        print(PlayerPrefsController.GetMaxRooms());
    }
}