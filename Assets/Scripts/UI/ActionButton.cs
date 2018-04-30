using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ActionButton : MonoBehaviour {

    enum sceneTypes { mainMenu, playGame};

    public string actionStr = "";

    void LoadGame() {
        SceneManager.LoadScene((int)sceneTypes.playGame);
    }

    // Get The action needed
    void GetAction() {
        switch (actionStr) {
            case "play":
                LoadGame();
                break;
            default:
                Debug.LogWarning("Invalid Action by " + gameObject.name);
                break;
        }
    }

    // Perform Action on Mouse Up
    private void OnMouseUp() {
        GetAction();
    }
}
