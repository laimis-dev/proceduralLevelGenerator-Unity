using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Observer;
using Utils;

public class SceneGeneratorObserver : MonoBehaviour, IObserver {
    [SerializeField] SceneGenerator sceneGenerator;
    [SerializeField] GameObject startButton;
    [SerializeField] GameObject UI;
    [SerializeField] TMP_Text stateText;

    float cameraSpeed = 500.0f;
    void Start(){
        startButton.SetActive(false);
        sceneGenerator.Attach(this);
    }

    void Update(){
        if (Input.GetKeyDown("escape"))
        {
            TurnOnCanvas();
        } else if(Input.GetMouseButton(1)){
            MoveCamera();
        }
    }
    public void UpdateObserver(string state){
        stateText.text = state;
        if(state == "Finished"){
            startButton.SetActive(true);
        }
    }

    public void TurnOffCanvas(){
        UI.SetActive(false);
    }

    private void TurnOnCanvas(){
        UI.SetActive(true);
    }

    private void MoveCamera(){
        if (Input.GetAxis ("Mouse X") < 0) {
			transform.position += new Vector3 (-Input.GetAxisRaw ("Mouse X") * Time.deltaTime * cameraSpeed, 
	                                   0.0f, -Input.GetAxisRaw ("Mouse Y") * Time.deltaTime * cameraSpeed);
		}
 
		else if (Input.GetAxis ("Mouse X") > 0) {
			transform.position += new Vector3 (-Input.GetAxisRaw ("Mouse X") * Time.deltaTime * cameraSpeed, 
	                                   0.0f, -Input.GetAxisRaw ("Mouse Y") * Time.deltaTime * cameraSpeed);
		}
    }


}