using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BiomeController : MonoBehaviour {

    enum skyTypes { day, dawnDusk, night}

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
    float worldRadius;
    static float worldClock = 0f;
    static float oneDayTime = 1f;
    static int worldDays = 1;
    static int worldYears = 0;
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

    [Space]
    // Planetary Entry
    [Header("Planetary Entry")]
    public GameObject entryParticles;
    float entryRange;
    float entryBuffer = 2f;
    bool isEntry = true;



    // MONOBEHAVIOUR --------------------------------------------------------------------
    private void Awake() {
        // Get Bounds
        worldBounds = world.GetComponent<Renderer>().bounds.size;
        // Get World Radius
        worldRadius = (worldBounds[0] / 2f);
        // Z Depth Layer Setup
        ZlayerLayout();
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
        // Dawn and dusk angles
        duskAngles[0] = 180f - (dawnDuskAngle / 2f);
        duskAngles[1] = 180f + (dawnDuskAngle / 2f);
        dawnAngles[0] = 360f - (dawnDuskAngle / 2f);
        dawnAngles[1] = 0f + (dawnDuskAngle / 2f);
        dawnDuskRange = duskAngles[1] - duskAngles[0];
        // Get Star Mask Sprite
        starMaskSpr = starMaskRevolveObj.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
    }

    // Use this for initialization
    void Start () {
        
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
        GroundMatUpdate();
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
        // Time
        worldClock += Time.deltaTime;
        // One Day
        if(worldClock >= oneDayTime) {
            // Update Alien
            PlayerStats.DailyUpdate();
            // Incrioment Days
            worldDays++;
            // + One Year
            if(worldDays > 365) {
                worldYears++;
                worldDays = 1;
                // Creatures Age
                for (int i = 0; i < creatureSpecs.Length; i++) {
                    creatureSpecs[i].AgeUpdate();
                }
                // Vegitation Age
                for (int i = 0; i < creatureSpecs.Length; i++) {
                    vegitationSpecs[i].AgeUpdate();
                }
            }
            // Reset Clock
            worldClock = 0f;
            // String Update
            UIController.YearUpdate("Years: " + worldYears + " /  Days: " + worldDays);
        }
    }

    // Sky Colour
    void SkyColor() {
        float _playerAngle = PlayerControlls.worldAngleDeg;
        bool _isSkyUpdate = false;
        bool _isDayToNight = true;
        float _normalized = 0f;
        
        // Shift To Night
        if (_playerAngle > duskAngles[0] && _playerAngle < duskAngles[1]) {
            _isSkyUpdate = true;
            _normalized = (_playerAngle - duskAngles[0]) / dawnDuskRange;
        }
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

        if (_isSkyUpdate) {
            Color _skyCol = new Color();
            // Day to Night
            if (_isDayToNight) {
                if (_normalized < 0.5f) {
                    _normalized /= 0.5f;
                    _skyCol = Color.Lerp(skyColors[(int)skyTypes.day], skyColors[(int)skyTypes.dawnDusk], _normalized);
                    starParticleSystem.SetActive(false);
                }
                else {
                    _normalized -= 0.5f;
                    _normalized /= 0.5f;
                    _skyCol = Color.Lerp(skyColors[(int)skyTypes.dawnDusk], skyColors[(int)skyTypes.night], _normalized);
                    starParticleSystem.SetActive(true);
                }
            }
            else {
                if(_playerAngle > dawnAngles[0]) {
                    _skyCol = Color.Lerp(skyColors[(int)skyTypes.night], skyColors[(int)skyTypes.dawnDusk], _normalized);
                    starParticleSystem.SetActive(true);
                }
                else {
                    _skyCol = Color.Lerp(skyColors[(int)skyTypes.dawnDusk], skyColors[(int)skyTypes.day], _normalized);
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

    // Alien Planetary Entry
    void PlanetaryEntry() {
        if (!isEntry) {
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
        if(_normalized <= 0) {
            isEntry = false;
            entryParticles.SetActive(false);
            PlayerControlls.controlLock = false; // unlock controls
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

    // Material Update
    void GroundMatUpdate() {
        if (!biomeHealthUpdate) {
            return;
        }
        biomeMat.color = Color.Lerp(biomeHealthCol[1], biomeHealthCol[0], (biomeHealth / 100));
        biomeHealthUpdate = false;
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
            vegitationSpecs[_type].startPlantationLimit = vegitationSpecs[_type].maxPlantationLimit;
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
            _NestsPos[i] = _NestAngle * (i + 1);
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
