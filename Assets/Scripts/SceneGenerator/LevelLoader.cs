using UnityEngine;
using UnityEngine.SceneManagement;
public class LevelLoader : MonoBehaviour {
    public void LoadSceneGenerator(){
        SceneManager.LoadScene("SceneGenerator");
    }

    public void LoadStartMenu(){
        SceneManager.LoadScene("StartMenu");
    }

    public void Quit(){
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}