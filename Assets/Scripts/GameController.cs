using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour{

    public static GameController Instance;

    public List<Character> Characters;
    public Character Player;

    void Awake(){
        Instance = this;
    }

    // Triggered when the script is enabled and first run
    void Start(){
        SceneList = new Dictionary<int, string>();
        BuildScenes();
        LoadGameItems();
        gameData = Saves.LoadGame();
        
        if (gameData != null){
            currentScene = gameData.location;
            if (Saves.loaded == false){ 
                SceneManager.LoadScene(SceneList[currentScene]);
                Saves.loaded = true;
            }
            gameDayNumber = gameData.day;
            gameTime = gameData.time;
            farthestScene = gameData.progress;
            string savedItems = gameData.items;
            List<string> SavedItemsList = new List<string>(savedItems.Split(';'));
            if (savedItems == ""){
                LoadStandardItems();
            } else if (Backpack.StoredItems.Count == 0) {
                foreach (string itemName in SavedItemsList){
                    if (GameItemList.Find(x => x.Name == itemName) != null){
                        Backpack.StoredItems.Add(GameItemList.Find(x => x.Name == itemName));
                    }
                }
            }
            placedObjects = gameData.placedObjects;
        } else {
            gameDayNumber = 1;
            gameTime = 0.0f;
            farthestScene = 1;
            currentScene = 1;
            LoadStandardItems();
        }

        Backpack.EquippedItemIndex = 0;
        UpdateToggles();
        FindCharacters();
        FindGates();
        FindMapItems();
        FindObjects();
        MainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        standardCameraSize = MainCamera.orthographicSize;
        cameraIsStandardSized = true;
        Daylight = GameObject.FindWithTag("Daylight").GetComponent<Light>();
        lengthOfDay = 600.0f;
        maxLightIntensity = 4.0f;
        sunrise = lengthOfDay / 4;
        sunset = lengthOfDay / 4 * 3;
        lightDensity = maxLightIntensity / sunrise;

        terrain = Terrain.activeTerrain;

        if (gameData != null){
            placedObjects = gameData.placedObjects;
        }
        foreach (AlteredObject po in placedObjects){
            if (po.Scene == SceneManager.GetActiveScene().name){
                GameObject loadedPo = Instantiate(Resources.Load(po.Prefab, typeof(GameObject))) as GameObject;
                loadedPo.transform.position = new Vector3(po.PositionX, po.PositionY, po.PositionZ);
            }
        }
    }

    void FixedUpdate(){
        MoveCharacter(Player);
        findTarget();
    }

    void Update(){
        if (Input.GetKeyDown(MapZoom)){
            MapToggle();
        } else if (Input.GetKeyDown(KeyCode.Alpha1)){
            FastTravel(1);
        } else if (Input.GetKeyDown(KeyCode.Alpha2)){
            FastTravel(2);
        }

        ClockTick();
        ItemUseCheck();

        if (ShotProjectile != null && (ShotProjectileObject.transform.position - ShotProjectileStart).magnitude >= ShotProjectile.Distance){
                GameObject.Destroy(ShotProjectileObject);
                ShotProjectile = null;
        }
    }
}
