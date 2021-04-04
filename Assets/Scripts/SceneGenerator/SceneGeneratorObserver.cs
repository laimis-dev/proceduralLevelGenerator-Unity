using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Observer;
using Utils;

public class SceneGeneratorObserver : MonoBehaviour, IObserver {
    [SerializeField] SceneGenerator sceneGenerator;
    [SerializeField] GameObject startButton;
    [SerializeField] GameObject UI;
    [SerializeField] GameObject sun;
    [SerializeField] TMP_Text stateText;

    float cameraSpeed = 300.0f;
    void Start(){
        startButton.SetActive(false);
        sceneGenerator.Attach(this);
        sun.transform.rotation = Quaternion.Euler(50,-30,0);
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
        sun.SetActive(false);
        UI.SetActive(false);
        
    }

    private void TurnOnCanvas(){
        sun.SetActive(true);
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