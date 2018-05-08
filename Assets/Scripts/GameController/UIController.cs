using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    enum powerButtonsType { jump, speed, omnivore}

    // Info
    [Header ("Info")]
    public Text yearText;
    static string yearStr = "";
    public GameObject speechBubbleObj;
    public static GameObject speechBubbleObjStat;

    [Space]
    // Panels
    [Header("Panels")]
    public GameObject powerUpPanel;
    public GameObject helpMenuPanel;
    static GameObject helpMenuPanelStat;

    [Space]
    // Buttons
    [Header("Buttons")]
    public Button[] powerButtons;
    public static int avaialbleSkills = 0;

    [Space]
    // Player Stats
    [Header("Player Stats")]
    public GameObject heartObj;
    static Vector3 heartScale = new Vector3(1f, 1f, 1f);
    public GameObject cubeRollMeatObj;
    static float hungerMinScaleRange = 0.3f;
    static float hungerRange;
    static Vector3 hungerScale = new Vector3(1f, 1f, 1f);
    public static bool statsUpdate = false;

    [Space]
    // Evolution Bar
    [Header("Evolution Bar")]
    public GameObject evolutionBar;
    Vector2 barMaxSizeDelta = new Vector2();
    float currentBarWidth = 0f;
    float alienMaxScale;
    float alienScaleRange;

    [Space]
    // Other
    [Header ("Other")]
    // Power Panel
    public static bool powerPanel_isSwitchPos = false;
    public static bool arePowersAvailable = false;
    public float powerPanel_Speed;
    public float powerPanel_HiddenPos;
    public float powerPanel_AvailablePos;
    Vector3 powerPanel_CurrentPos;
    // Help Menu
    public static bool isHelpMenu = false;

    // MONOBEHAVIOUR --------------------------------------------------------------
    private void Awake() {
        hungerRange = 1f - hungerMinScaleRange;
        barMaxSizeDelta = evolutionBar.GetComponent<RectTransform>().sizeDelta;
        alienMaxScale = PlayerStats.maxGrowSize;
        alienScaleRange = alienMaxScale - PlayerStats.growSize;
        speechBubbleObj.SetActive(false);
        helpMenuPanelStat = helpMenuPanel;
        helpMenuPanelStat.SetActive(false);
        speechBubbleObjStat = speechBubbleObj;
        speechBubbleObjStat.SetActive(false);
    }
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
        // Health and Hunger
        StatsRefresh();
    }

    // draw Evolution Bar
    private void LateUpdate() {
        EvolutionBarUpdate();
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
                    Cursor.visible = false;
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
                    Cursor.visible = true;
                }
            }
            // Update Pos
            powerUpPanel.GetComponent<RectTransform>().anchoredPosition = powerPanel_CurrentPos;
        }
    }

    // Updates UI Player stats 
    public static void StatsUpdate(float _health, float _hunger) {
        statsUpdate = true;
        // Health
        heartScale = new Vector3(_health, _health, _health) / 100f;
        // hunger
        _hunger /= 100f;
        _hunger = hungerMinScaleRange + (_hunger * hungerRange);
        hungerScale = new Vector3(_hunger, 1f, _hunger);
    }

    // Stats Refresh
    void StatsRefresh() {
        if (statsUpdate) {
            heartObj.transform.localScale = heartScale;
            cubeRollMeatObj.transform.localScale = hungerScale;
        }
    }

    // Update Evolution Bar
    void EvolutionBarUpdate() {
        currentBarWidth = ((PlayerStats.growSize - 1f) / alienScaleRange) * barMaxSizeDelta[0];
        evolutionBar.GetComponent<RectTransform>().sizeDelta = new Vector2(currentBarWidth, barMaxSizeDelta[1]);
    }

    // Toggles on and off help manu
    public static void ToggleHelpMenu() {
        isHelpMenu = !isHelpMenu;
        PlayerControlls.controlLock = isHelpMenu;
        BiomeController.isPaused = isHelpMenu;
        // Enable Disable Menu
        if (!isHelpMenu) {
            if (helpMenuPanelStat.activeSelf) {
                helpMenuPanelStat.SetActive(false);
            }
            return;
        }
        else if (!helpMenuPanelStat.activeSelf) {
            helpMenuPanelStat.SetActive(true);
        }
        // Speach bubble
        if (speechBubbleObjStat.activeSelf) {
            speechBubbleObjStat.SetActive(false);
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
