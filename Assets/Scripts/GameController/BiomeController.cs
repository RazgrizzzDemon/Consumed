﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BiomeController : MonoBehaviour {

    enum skyTypes { day, dawnDusk, night}
    enum timeMultiplierTypes { normal, dayBlast, yearsBlast}

    // Biome Health
    [Header("Biome Helath")]
    public Material biomeMat;
    static Color[] biomeHealthCol = new Color[2] { new Color(1f,1f,1f,1f), new Color(0.1f, 0.04f, 0f, 1f) };
    static float biomeHealth = 100;
    static int totalBiomeSpecies = 0;
    static bool biomeHealthUpdate = true;


    // World
    [Header("World")]
    public GameObject world;
    Vector3 worldBounds;
    public static float worldRadius;
    static int[] timeMultiplier = new int[3] { 1, 10, 10}; // normal, days, years
    static int currentTimeMultiplier = 0;
    static int regrowStage = 0;
    static bool regrowStageUpdate = false;
    static float worldClock = 0f;
    static float oneDayTime = 1f;
    static int worldDays = 1;
    static int worldYears = 0;
    public static bool isPaused = true; // Only world clock is stoped, animations continue
    public static float[] zDepthLayers;
    public static int maxZlayers;

    [Space]
    // Creatures
    [Header("Creatures")]
    public CreatureSpecs[] creatureSpecs;
    [Space]
    [Header("Vegitation")]
    public VegitationSpecs[] vegitationSpecs;
    [Space]
    [Header("Flying Types")]
    public FlyingTypes[] flyingTypes;
    public static GameObject[] LifeContainers; // Contains all life groups (ex: cats, pigs, grass, trees.. etc)
    public static int currentLifeContainer = 0;
    static int[] deathRate;

    [Space]
    // Day and Night
    [Header("Day and Night")]
    public GameObject starParticleSystem;
    public GameObject starMaskRevolveObj;
    SpriteRenderer starMaskSpr;
    public Color[] skyColors;
    public float dawnDuskAngle = 30f;
    Vector2 dawnAngles = new Vector2();// Direction is towards vector angle, Clock wise is dawn.
    Vector2 duskAngles = new Vector2(); // Direction is towards vector angle, Anti-clockwise is duck.
    float dawnDuskRange; // the range between angles
    public Light sunLight;
    public float sunMaxLight;
    public Light dirrectLight;
    public float dirrectionalMaxLight;
    public Light moonLight;

    [Space]
    // Planetary Entry
    [Header("Planetary Entry")]
    public GameObject entryParticles;
    float entryRange;
    float entryBuffer = 2f;
    bool isEntry = true;



    // MONOBEHAVIOUR --------------------------------------------------------------------
    private void Awake() {
        // Reset Biome
        BiomeReset();
        // Get Bounds
        worldBounds = world.GetComponent<Renderer>().bounds.size;
        // Get World Radius
        worldRadius = (worldBounds[0] / 2f);
        // Z Depth Layer Setup
        ZlayerLayout();
        // Count
        int _numberOfSpecies = creatureSpecs.Length + vegitationSpecs.Length;
        // Set container Array
        LifeContainers = new GameObject[_numberOfSpecies];
        // Set Date Rate Counter
        deathRate = new int[_numberOfSpecies];
        // Create
        // Create Species
        for (int i = 0; i < creatureSpecs.Length; i++) {
            creatureSpecs[i].Initialize(worldRadius, ref world);
            // Posiion
            NestDistributor(i);
        }
        // Vegitation
        for (int i = 0; i < vegitationSpecs.Length; i++) {
            vegitationSpecs[i].Initialize(worldRadius, ref world);
            // Position
            PlantationDistributor(i);
        }
        // Flying Objects Awake
        for (int i = 0; i < flyingTypes.Length; i++) {
            flyingTypes[i].Initialize();
        }
        // Dawn and dusk angles
        duskAngles[0] = 180f - (dawnDuskAngle / 2f);
        duskAngles[1] = 180f + (dawnDuskAngle / 2f);
        dawnAngles[0] = 360f - (dawnDuskAngle / 2f);
        dawnAngles[1] = 0f + (dawnDuskAngle / 2f);
        dawnDuskRange = duskAngles[1] - duskAngles[0];
        // Lights
        moonLight.gameObject.SetActive(false);
        // Get Star Mask Sprite
        starMaskSpr = starMaskRevolveObj.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
    }

    // Use this for initialization
    void Start () {
        // Flying Objects Awake
        for (int i = 0; i < flyingTypes.Length; i++) {
            flyingTypes[i].Start();
        }
    }
	
	// Update is called once per frame
	void Update () {
        WorldTimeUpdate();
        // Auto MOve
        for (int i = 0; i < creatureSpecs.Length; i++) {
            creatureSpecs[i].AutoMove();
        }
    }

    // Late update so that player movement will be already avaluated
    private void LateUpdate() {
        PlanetaryEntry();
        SkyColor();
        BiomeHealthUpdate();
        Regrow();
    }

    private void FixedUpdate() {
        // Update Rigid body
        for (int i = 0; i < creatureSpecs.Length; i++) {
            creatureSpecs[i].MoveUpdate();
        }
    }

    // METHODS ----------------------------------------------------------------------
    // ** Wolrd Time Update **
    void WorldTimeUpdate() {
        // Pause
        if (isPaused) {
            return;
        }
        // Time
        worldClock = (worldClock + Time.deltaTime);

        // One Day
        if(worldClock >= oneDayTime) {
            // Time Speed Up
            // Days Blast
            if (currentTimeMultiplier == (int)timeMultiplierTypes.dayBlast) {
                TimeLock(true);
                StartCoroutine(DayBlast());
            }
            // Year Blast
            else if (currentTimeMultiplier == (int)timeMultiplierTypes.yearsBlast) {
                TimeLock(true);
                StartCoroutine(YearBlast());
            }
            // Normal Speed
            else {
                DailyUpdate();
                // + One Year
                if (worldDays > 365) {
                    YearlyUpdate();
                }
            }
            
            // Reset Clock
            worldClock = 0f;
            // String Update
            UIController.YearUpdate("Days: " + worldDays + ", Years: " + worldYears);
        }
    }

    // Daily Update
    void DailyUpdate() {
        // Update Alien
        PlayerStats.DailyUpdate();
        // Creatures Daily Updates
        for (int i = 0; i < creatureSpecs.Length; i++) {
            creatureSpecs[i].DailyUpdate();
        }
        // Vegitation Daily Updates
        for (int i = 0; i < vegitationSpecs.Length; i++) {
            vegitationSpecs[i].DailyUpdate();
        }
        // Reproductioon
        Reproduce();
        // Incrioment Days
        worldDays++;
    }

    // Yearly Update
    void YearlyUpdate() {
        worldYears++;
        worldDays = 1;
        // Creatures Age
        for (int i = 0; i < creatureSpecs.Length; i++) {
            creatureSpecs[i].AgeUpdate();
        }
        // Vegitation Age
        for (int i = 0; i < vegitationSpecs.Length; i++) {
            vegitationSpecs[i].AgeUpdate();
        }
    }

    // Sky Colour
    void SkyColor() {
        float _playerAngle = CameraControl.currentAngle[2] + 90;
        // Limit from 0 - 360
        if(_playerAngle > 360) {
            _playerAngle -= 360f;
        }
        else if(_playerAngle < 0) {
            _playerAngle = 360f + _playerAngle;
        }
        bool _isSkyUpdate = false;
        bool _isDayToNight = true;
        float _normalized = 0f;
        
        // Shift To Night
        if (_playerAngle > duskAngles[0] && _playerAngle < duskAngles[1]) {
            _isSkyUpdate = true;
            _normalized = (_playerAngle - duskAngles[0]) / dawnDuskRange;
        }
        // Shift to day
        else if((_playerAngle < 360 && _playerAngle > dawnAngles[0]) || (_playerAngle > 0 && _playerAngle < dawnAngles[1])) {
            _isSkyUpdate = true;
            _isDayToNight = false;
            if(_playerAngle > dawnAngles[0]) {
                _normalized = (_playerAngle - dawnAngles[0]) / (dawnDuskRange / 2f);
            }
            else {
                _normalized = _playerAngle / (dawnDuskRange / 2f);
            }
        }

        // Enable Disable Lights
        if(_playerAngle >= 0 && _playerAngle <= 180) {
            // Day Lights
            SwitchBiomeLights(true);
        }
        else {
            // Night Lights
            SwitchBiomeLights(false);
        }

        // Year Text Colour Switch
        if(_playerAngle > duskAngles[1] && _playerAngle < dawnAngles[0]) {
            // Night Clours
            if (UIController.isDay) {
                UIController.isDay = false;
                UIController.isColorSwitch = true;
            }
        }
        else {
            if (!UIController.isDay) {
                UIController.isDay = true;
                UIController.isColorSwitch = true;
            }
        }

        if (_isSkyUpdate) {
            Color _skyCol = new Color();
            // Day to Night
            if (_isDayToNight) {
                if (_normalized < 0.5f) {
                    _normalized /= 0.5f;
                    _skyCol = Color.Lerp(skyColors[(int)skyTypes.day], skyColors[(int)skyTypes.dawnDusk], _normalized);
                    sunLight.intensity = Mathf.Lerp(sunMaxLight, 0.0f, _normalized);
                    dirrectLight.intensity = Mathf.Lerp(dirrectionalMaxLight, 0.0f, _normalized);
                    starParticleSystem.SetActive(false);
                }
                else {
                    _normalized -= 0.5f;
                    _normalized /= 0.5f;
                    _skyCol = Color.Lerp(skyColors[(int)skyTypes.dawnDusk], skyColors[(int)skyTypes.night], _normalized);
                    starParticleSystem.SetActive(true);
                }
            }
            // Night to Day
            else {
                if(_playerAngle > dawnAngles[0]) {
                    _skyCol = Color.Lerp(skyColors[(int)skyTypes.night], skyColors[(int)skyTypes.dawnDusk], _normalized);
                    starParticleSystem.SetActive(true);
                }
                else {
                    _skyCol = Color.Lerp(skyColors[(int)skyTypes.dawnDusk], skyColors[(int)skyTypes.day], _normalized);
                    sunLight.intensity = Mathf.Lerp(0f, sunMaxLight, _normalized);
                    dirrectLight.intensity = Mathf.Lerp(0f, dirrectionalMaxLight, _normalized);
                    starParticleSystem.SetActive(false);
                }
            }
            // Colour Camera Background
            Camera.main.GetComponent<Camera>().backgroundColor = _skyCol;
            // Colour Star MAsk
            starMaskSpr.color = _skyCol;
            // Rotate counter player
            starMaskRevolveObj.transform.localEulerAngles = new Vector3(0f, 0f, -_playerAngle + 90f);
        }
    }

    // Enable Disable Lights
    void SwitchBiomeLights(bool _isDay) {
        if(_isDay && moonLight.gameObject.activeSelf) {
            sunLight.gameObject.SetActive(true);
            dirrectLight.gameObject.SetActive(true);
            moonLight.gameObject.SetActive(false);
        }
        else if(!_isDay && sunLight.gameObject.activeSelf) {
            sunLight.gameObject.SetActive(false);
            dirrectLight.gameObject.SetActive(false);
            moonLight.gameObject.SetActive(true);
        }
    }

    // Alien Planetary Entry
    void PlanetaryEntry() {
        if (!isEntry || PlayerControlls.playerPos[1] == 0) {
            return;
        }
        // Set Entry Range
        if(entryBuffer == 2f) {
            entryBuffer += worldRadius;
            entryRange = PlayerControlls.playerPos[1] - entryBuffer;
            entryParticles.SetActive(true);
        }
        float _normalized = ((PlayerControlls.playerPos[1] - entryBuffer) / entryRange);
        Camera.main.GetComponent<Camera>().backgroundColor = Color.Lerp(skyColors[(int)skyTypes.day], skyColors[(int)skyTypes.night], _normalized);
        sunLight.intensity = Mathf.Lerp(sunMaxLight, 0.0f, _normalized);
        dirrectLight.intensity = Mathf.Lerp(dirrectionalMaxLight, 0.0f, _normalized);
        // Stop Entry
        if (_normalized <= 0) {
            isEntry = false;
            entryParticles.SetActive(false);
            PlayerControlls.controlLock = false; // unlock controls
            UIController.speechBubbleObjStat.SetActive(true); // Show Bubble
            isPaused = false; // Start world Timer
        }
    }


    // Z Layer Layout - setts the position of each layer based on the number of creatures and vegitation.
    void ZlayerLayout() {
        // Gatter all biome species and devide by worlds  Z depth
        maxZlayers = (creatureSpecs.Length + vegitationSpecs.Length);
        float _layersSpacing = worldBounds[2] / maxZlayers;
        float _halfLayerSpaceing = _layersSpacing / 2f;
        // Set Z Layers Cneter position
        zDepthLayers = new float[maxZlayers];
        for (int i = 0; i < zDepthLayers.Length; i++) {
            if (i == 0) {
                zDepthLayers[i] = (worldBounds[2] / 2f) - _halfLayerSpaceing;
            }
            else {
                zDepthLayers[i] = zDepthLayers[i - 1] - _layersSpacing;
            }
        }
        // Assign to Species according to their layer preferance
        // Creatures
        for (int i = 0; i < creatureSpecs.Length; i++) {
            creatureSpecs[i].worldZdepth = zDepthLayers[creatureSpecs[i].zLayerNumber];
        }
        // Vegitation
        for (int i = 0; i < vegitationSpecs.Length; i++) {
            vegitationSpecs[i].worldZdepth = zDepthLayers[vegitationSpecs[i].zLayerNumber];
        }
    }

    // Biome Health Update
    public static void HealthUpdate(bool _isPositive = true, bool _onAwake = false) {
        if (_onAwake && _isPositive) {
            totalBiomeSpecies++;
            return;
        }
        // Creatures birth
        else if (_isPositive) {
            biomeHealth += ((1f / totalBiomeSpecies) * 100f);
        }
        // Creatures Death
        else {
            biomeHealth -= ((1f / totalBiomeSpecies) * 100f);
        }
        biomeHealthUpdate = true;
    }

    // Death Rate - The amount of creatures died from a type
    public static void DeathRate(string _name, bool _isPregnant) {
        for (int i = 0; i < LifeContainers.Length; i++) {
            if (LifeContainers[i].name.Equals(_name)) {
                deathRate[i]++;
                if (_isPregnant) {
                    deathRate[i]++;
                }
            }
        }
    }

    // Check for creatures to Reproduce
    static void Reproduce() {
        bool _isDone = false;
        bool _haveMale = false;
        bool _haveFemale = false;
        int _femaleIndex = -1;
        for (int i = 0; i < LifeContainers.Length; i++) {
            if (deathRate[i] > 0) {
                for (int j = 0; j < LifeContainers[i].gameObject.transform.childCount; j++) {
                    // Exclusive Serch
                    bool _exclusive = false;
                    bool _forMale = false;
                    if (_haveMale || _haveMale) {
                        _exclusive = true;
                        if (_haveFemale) {
                            _forMale = true;
                        }
                    }
                    // Search
                    int _result = LifeContainers[i].gameObject.transform.GetChild(j).gameObject.GetComponent<CreaturesBase>().Reproduce(_exclusive, _forMale);
                    // Not needed result
                    if (_result == -1) {
                        continue;
                    }
                    // Male
                    else if (_result == (int)genderType.male) {
                        _haveMale = true;
                    }
                    // Female
                    else if (_result == (int)genderType.female) {
                        _haveFemale = true;
                        _femaleIndex = j;
                    }
                    // Got Male and Female
                    if (_haveFemale && _haveMale) {
                        LifeContainers[i].gameObject.transform.GetChild(_femaleIndex).gameObject.GetComponent<CreaturesBase>().Pregnant(true);
                        deathRate[i]--;
                        _isDone = true;
                        break;
                    }
                }
                if (_isDone) {
                    break;
                }
            }
        }
    }

    // Birth
    public static void Birth(string _name) {
        bool _isDone = false;
        // Search for a dead one and revive
        for (int i = 0; i < LifeContainers.Length; i++) {
            if (LifeContainers[i].name.Equals(_name)) {
                for (int j = 0; j < LifeContainers[i].gameObject.transform.childCount; j++) {
                    // If it is Dead, Recycle
                    if (!LifeContainers[i].gameObject.transform.GetChild(j).gameObject.GetComponent<CreaturesBase>().isAlive) {
                        LifeContainers[i].gameObject.transform.GetChild(j).gameObject.GetComponent<CreaturesBase>().InitializeSpecies("", "");
                        HealthUpdate(true);
                        _isDone = true;
                        break;
                    }
                }
            }
            if (_isDone) {
                break;
            }
        }
    }

    // Material Update
    void BiomeHealthUpdate() {
        if (!biomeHealthUpdate) {
            return;
        }
        biomeMat.color = Color.Lerp(biomeHealthCol[1], biomeHealthCol[0], (biomeHealth / 100));
        biomeHealthUpdate = false;
        if(currentTimeMultiplier == (int)timeMultiplierTypes.normal && biomeHealth <= 0.9) {
            currentTimeMultiplier = (int)timeMultiplierTypes.dayBlast;
        }
    }

    public static void StartRegrowt() {
        if(biomeHealth > 0.9) {
            return;
        }
        Debug.Log("Regrowt Started");
        currentTimeMultiplier = (int)timeMultiplierTypes.yearsBlast;
        currentLifeContainer = 0;
        regrowStage = 1;
        regrowStageUpdate = true;
    }

    // Regrowt
    void Regrow() {
        if(regrowStage == 0 || !regrowStageUpdate) {
            return;
        }
        Debug.Log("Stage: " + regrowStage);
        switch (regrowStage) {
            // Stage one grow grass
            case 1:
                for (int i = 0; i < vegitationSpecs.Length; i++) {
                    vegitationSpecs[i].Regenerate("grass");
                }
                break;
            // Stage 2 grow trees
            case 2:
                for (int i = 0; i < vegitationSpecs.Length; i++) {
                    vegitationSpecs[i].Regenerate("trees");
                }
                break;
            // Stage 3 grow animals
            case 3:
                for (int i = 0; i < creatureSpecs.Length; i++) {
                    creatureSpecs[i].Regenerate();
                }
                break;
        }
        regrowStageUpdate = false;
        
        
    }

    // Reset Biome
    static void BiomeReset() {
        biomeHealth = 100;
        currentTimeMultiplier = 0;
        regrowStage = 0;
        worldClock = 0f;
        worldDays = 1;
        worldYears = 0;
        currentLifeContainer = 0;
    }

    // TIME SPEED -------------------------------------------------------------------------------------------
    IEnumerator YearBlast() {
        int _times = 0;
        int _devider = 4;
        int _days = 366;
        for (int i = 0; i < timeMultiplier[currentTimeMultiplier]; i++) {
            for (int j = 0; j < _days; j++) {
                DailyUpdate();
                if(j > (_days / _devider) * _times) {
                    yield return _times++;
                }
            }
            yield return null;
            YearlyUpdate();
            yield return null;
        }
        // Regrowt
        if (regrowStage > 0) {
            regrowStage++;
            if (regrowStage > 3) {
                regrowStage = 0;
            }
            else {
                regrowStageUpdate = true;
            }
        }
        TimeLock(false);
    }

    IEnumerator DayBlast() {
        for (int i = 0; i < timeMultiplier[currentTimeMultiplier]; i++) {
            DailyUpdate();
            yield return null;
        }
        TimeLock(false);
    }

    void TimeLock(bool _ans) {
        isPaused = _ans;
    }

    // LIFE FORMS -------------------------------------------------------------------------------------------
    // Nest Distributor
    void NestDistributor(int _type) {
        float _speciesSize = creatureSpecs[_type].species[0].GetComponent<CreaturesBase>().fullOccupationAngle;
        float _NestSize =  _speciesSize * creatureSpecs[_type].startHerdLimit;
        float[] _NestsPos = new float[creatureSpecs[_type].nestsNum];
        float _NestAngle = 360f / creatureSpecs[_type].nestsNum;
        float _nextPos = 0;
        int j = 0;
        for (int i = 0; i < _NestsPos.Length; i++) {
            // Set nest Angle
            _NestsPos[i] = _NestAngle * (i + 1);
            // Instantiate and position
            for (; j < creatureSpecs[_type].startHerdLimit * (i + 1); j++) {
                // Check if first on Nest
                if(j + 1 == ((i + 1) * creatureSpecs[_type].startHerdLimit)) {
                    // if yes its possition is on angle limit
                    _nextPos = (_NestsPos[i] + (_NestSize / 2f)) - _speciesSize;
                }
                else {
                    _nextPos -= (_speciesSize + (_speciesSize / 2f));
                }
                creatureSpecs[_type].InitSpawn(j, _nextPos, _NestSize);
                HealthUpdate(true, true);
            }
        }
    }

    // Plantation Distributor
    void PlantationDistributor(int _type) {
        float _speciesSize = vegitationSpecs[_type].species[0].GetComponent<CreaturesBase>().fullOccupationAngle;
        float _NestSize;
        // Fill Planet
        if (vegitationSpecs[_type].startPlantationLimit == 0) {
            vegitationSpecs[_type].startPlantationLimit = vegitationSpecs[_type].totalPlants;
            vegitationSpecs[_type].plantationsNum = 1;
            _NestSize = 360f;
        }
        // Plantation
        else {
            _NestSize = _speciesSize * vegitationSpecs[_type].startPlantationLimit;
        }
        float[] _NestsPos = new float[vegitationSpecs[_type].plantationsNum];
        float _NestAngle = 360f / vegitationSpecs[_type].plantationsNum;
        float _nextPos = 0;
        int j = 0;
        for (int i = 0; i < _NestsPos.Length; i++) {
            // Set nest Angle
            _NestsPos[i] = (_NestAngle * (i + 1)) + vegitationSpecs[_type].angleOffset;
            // Instantiate and position
            for (; j < vegitationSpecs[_type].startPlantationLimit * (i + 1); j++) {
                // Check if first on Nest
                if (j + 1 == ((i + 1) * vegitationSpecs[_type].startPlantationLimit)) {
                    // if yes its possition is on angle limit
                    _nextPos = (_NestsPos[i] + (_NestSize / 2f)) - _speciesSize;
                }
                else {
                    _nextPos -= _speciesSize;
                }
                vegitationSpecs[_type].Active(j, true);
                vegitationSpecs[_type].InitSpawn(j, _nextPos);
                HealthUpdate(true, true);
            }
        }
    }
}
