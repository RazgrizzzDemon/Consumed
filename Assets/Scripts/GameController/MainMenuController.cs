using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour {

    enum sceneTypes { egg, hatch, portal, travel}

    // Book
    [Header("Scenes")]
    public GameObject[] scenes;
    int previopusScene = 0;
    int currentScene = 0;
    bool isUpdate = true;

    [Header("BlackOut")]
    public Image blackoutImage;
    public Color[] blackoutColors = new Color[2];
    public float blackoutDuration = 1;
    float currentBlackoutTime = 0f;
    bool isBlackOut = false;

    // MONOBEHAVIOR --------------------------------------------------
    private void Awake() {
        scenes[0].SetActive(true);
        for (int i = 1; i < scenes.Length; i++) {
            scenes[i].SetActive(false);
        }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (isBlackOut) {
            BlackOut();
            return;
        }
        if (Input.GetMouseButtonDown(0)) {
            SceneLog();
            currentScene++;
        }
        else if (Input.GetMouseButtonDown(1)) {
            SceneLog();
            currentScene--;
        }

        if(currentScene < 0) {
            currentScene = 0;
        }
        else if(currentScene > scenes.Length - 1) {
            SceneManager.LoadScene(1);
        }
	}

    // Takes note of the previous scene and prompts update.
    void SceneLog() {
        previopusScene = currentScene;
        isBlackOut = true;
    }

    void SceneUpdate() {
        scenes[previopusScene].SetActive(false);
        scenes[currentScene].SetActive(true);
        isUpdate = false;
    }

    void BlackOut() {
        if (isBlackOut) {
            currentBlackoutTime += Time.deltaTime;
            float _normalized = currentBlackoutTime / blackoutDuration;
            // Switch Scene
            if (isUpdate && _normalized >= 0.5f) {
                SceneUpdate();
            }
            // Stop Blackout
            else if(_normalized >= 1) {
                currentBlackoutTime = 0f;
                isBlackOut = false;
                isUpdate = true;
            }
            // Lerp Forward
            if(_normalized < 0.5f) {
                _normalized /= 0.5f;
            }
            // Lerp Back
            else {
                _normalized = 1 - ((_normalized - 0.5f) / 0.5f);
            }
            //Debug.Log("Norm After: " + _normalized);
            //Debug.Log("---------------------------------------");
            // Colour Image
            blackoutImage.color = Color.Lerp(blackoutColors[0], blackoutColors[1], _normalized);
        }
    }
}
