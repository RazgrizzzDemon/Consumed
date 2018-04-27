﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    enum powerButtonsType { jump, speed, omnivore}

    // Info
    [Header ("Info")]
    public Text yearText;
    static string yearStr = "";

    [Space]
    // Panels
    [Header("Panels")]
    public GameObject powerUpPanel;

    [Space]
    // Buttons
    [Header("Buttons")]
    public Button[] powerButtons;
    public static int avaialbleSkills = 0;

    // Other
    public static bool powerPanel_isSwitchPos = false;
    public static bool arePowersAvailable = false;
    public float powerPanel_Speed;
    public float powerPanel_HiddenPos;
    public float powerPanel_AvailablePos;
    Vector3 powerPanel_CurrentPos;

    // MONOBEHAVIOUR --------------------------------------------------------------
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        // Update Text
        if (!yearStr.Equals("")) {
            yearText.text = yearStr;
        }
        // Hide show Powers
        HideShowPowerPanel();
	}

    // METHODS --------------------------------------------------------------------
    // Date text Update
    public static void YearUpdate(string _str) {
        yearStr = _str;
    }

    // Show Hide panel
    void HideShowPowerPanel() {
        if (powerPanel_isSwitchPos) {
            powerPanel_CurrentPos = powerUpPanel.GetComponent<RectTransform>().anchoredPosition;
            // Hide Powers
            if (arePowersAvailable) {
                powerPanel_CurrentPos[1] -= (powerPanel_Speed * Time.deltaTime);
                // Check if on target
                if(powerPanel_CurrentPos[1] <= powerPanel_HiddenPos) {
                    powerPanel_CurrentPos[1] = powerPanel_HiddenPos;
                    arePowersAvailable = false;
                    powerPanel_isSwitchPos = false;
                }
            }
            // Show Powers
            else if (!arePowersAvailable) {
                powerPanel_CurrentPos[1] += (powerPanel_Speed * Time.deltaTime);
                // Check if on target
                if (powerPanel_CurrentPos[1] >= powerPanel_AvailablePos) {
                    powerPanel_CurrentPos[1] = powerPanel_AvailablePos;
                    arePowersAvailable = true;
                    powerPanel_isSwitchPos = false;
                }
            }
            // Update Pos
            powerUpPanel.GetComponent<RectTransform>().anchoredPosition = powerPanel_CurrentPos;
        }
    }

    // BUTTONS ------------------------------------------------------------------------
    // Jump
    public void ActivateJump() {
        PlayerControlls.jumpLock = false;
        NonInterectableButtons((int)powerButtonsType.jump);
    }
    // Speed
    public void DoubleSpeed() {
        PlayerControlls.doubleSpeed = true;
        NonInterectableButtons((int)powerButtonsType.speed);
    }
    // Omnivore
    public void Omnivore() {
        PlayerControlls.isOmnivore = true;
        NonInterectableButtons((int)powerButtonsType.omnivore);
    }
    // NonInterectable Buttons + Hide Panel
    void NonInterectableButtons(int _buttNum) {
        powerButtons[_buttNum].interactable = false;
        --avaialbleSkills;
        if (avaialbleSkills == 0) {
            powerPanel_isSwitchPos = true;
        }
    }
}
