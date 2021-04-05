using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Observer;
using Utils;
using RPG.Core;

public class UIAndCamera : MonoBehaviour, IObserver {
    [SerializeField] SceneGenerator sceneGenerator;
    [SerializeField] GameObject startButton;
    [SerializeField] GameObject topView;
    [SerializeField] TMP_Text stateText;

    [SerializeField] GameObject gameView;
    [SerializeField] TMP_Text healthText;

    float cameraSpeed = 300.0f;
    void Start(){
        startButton.SetActive(false);
        sceneGenerator.Attach(this);
        TurnOnCanvas();
    }

    void Update(){
        Health playerHealth = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<Health>();
        if(playerHealth != null) healthText.text = playerHealth.GetHealth().ToString();

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
        gameView.SetActive(true);
        topView.SetActive(false);
    }

    private void TurnOnCanvas(){
        topView.SetActive(true);
        gameView.SetActive(false);
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