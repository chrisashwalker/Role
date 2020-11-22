using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour, IPointerClickHandler{

    public static GameController Instance;

    public List<Character> Characters;
    public Character Player;

    void Awake(){
        Instance = this;
    }

    public void OnPointerClick(PointerEventData pointerEventData){
        GameObject clickedToggle = pointerEventData.pointerPress;
        if (clickedToggle.tag == "ItemToggle"){
            InventoryManager.Equip(clickedToggle);
        }
    }

    void ItemUseCheck(){
        int itemShift = 0;
        if (Input.GetKeyDown(ScrollLeft)){
            itemShift = -1;
        } else if (Input.GetKeyDown(ScrollRight)){
            itemShift = 1;
        }
        if (itemShift != 0){
            if (Backpack.EquippedItemIndex + itemShift < 0){
                Backpack.EquippedItemIndex = Backpack.StoredItems.Count - 1;
            } else if (Backpack.EquippedItemIndex + itemShift > Backpack.StoredItems.Count - 1){
                Backpack.EquippedItemIndex = 0;
            } else {
                Backpack.EquippedItemIndex += itemShift;
            }
            UpdateToggles();
        }
        if (Input.GetKeyDown(UseItem)){
            if (Backpack.StoredItems[Backpack.EquippedItemIndex].Type.Equals(ItemTypes.PROJECTILE) && ShotProjectile == null){
                ShootProjectile(Player);
            } else if (Backpack.StoredItems[Backpack.EquippedItemIndex].Type.Equals(ItemTypes.TOOL)){
                UseTool(Player);   
            }
            UpdateToggles();     
        }
    }

    void BuildScenes(){
        SceneList.Add(1, "1_home");
        SceneList.Add(2, "2_field");
    }

    void FindCharacters(){
        Player = new Character();
        Player.Object = GameObject.FindWithTag("Player");
        Player.Rigidbody = Player.Object.GetComponent<Rigidbody>();
        Player.Collider = Player.Object.GetComponent<Collider>();
        Player.Type.Add(CharacterTypes.Player);

        if (GameObject.FindGameObjectsWithTag("Character").Length > 0){
            CharacterArray = GameObject.FindGameObjectsWithTag("Character");
            foreach (GameObject charObject in CharacterArray){
                Character newChar = new Character();
                newChar.Object = charObject;
                newChar.Rigidbody = newChar.Object.GetComponent<Rigidbody>();
                newChar.Collider = newChar.Object.GetComponent<Collider>();
                newChar.Type.Add(CharacterTypes.CHARACTER);
            }
        }
    }

     void FindObjects(){
        Rocks = new List<GameObject>();
        Trees = new List<GameObject>();

        if (GameObject.FindGameObjectsWithTag("Rock").Length > 0){
            GameObject[] rockArray = GameObject.FindGameObjectsWithTag("Rock");
            foreach (GameObject rock in rockArray){
                Rocks.Add(rock);
            }
        }

        if (GameObject.FindGameObjectsWithTag("Tree").Length > 0){
            GameObject[] treeArray = GameObject.FindGameObjectsWithTag("Tree");
            foreach (GameObject tree in treeArray){
                Trees.Add(tree);
            }
        }
    }

    void MoveCharacter(Character character){
        character.Rigidbody.velocity = new Vector3(Input.GetAxis("Horizontal") * 10,0,Input.GetAxis("Vertical") * 20);
        if (character.Rigidbody.velocity.z > 0){
            character.Direction = 'U';
        } else if (character.Rigidbody.velocity.z < 0){
            character.Direction = 'D';
        } else if (character.Rigidbody.velocity.x > 0){
            character.Direction = 'R';
        } else if (character.Rigidbody.velocity.x < 0){
            character.Direction = 'L';
        }
    }

    void MapToggle(){ // Effectively zooms out to grant an overview of the surrounding area
        if (cameraIsStandardSized){
            MainCamera.orthographicSize = standardCameraSize * 10;
            cameraIsStandardSized = false;
            } else {
            MainCamera.orthographicSize = standardCameraSize;
            cameraIsStandardSized = true;
            }
    }

    void FastTravel(int sceneNumber){
        //if (farthestScene >= sceneNumber && currentScene != sceneNumber){ TODO: Reactivate after testing
            currentScene = sceneNumber;
            SceneManager.LoadScene(SceneList[sceneNumber], LoadSceneMode.Single);
            string savedItems = "";
            foreach (Item i in Backpack.StoredItems){
                savedItems += i.Name + ";";
            }
            gameData = new Saves.SaveData(gameDayNumber, gameTime, farthestScene, currentScene, savedItems, placedObjects);
            Saves.SaveGame(gameData);
        //}
    }

    void ClockTick(){
        gameTime += Time.deltaTime;
        daylightTime = gameTime - sunrise;
        if (gameTime > sunrise && gameTime < sunset){
            // Handles light intensity relative to time of day; the closer to midday, the stronger the light
            Daylight.intensity = 0.2f + maxLightIntensity - System.Math.Abs((sunrise - daylightTime) * lightDensity);
        } else {
            Daylight.intensity = 0.2f;
        }
        if (gameTime >= lengthOfDay){
            gameDayNumber += 1;
            gameTime = 0.0f;
            Debug.Log("It's Day " + gameDayNumber);
        }
    }

    void FindGates(){
        GateList = new List<Gate>();
        foreach (GameObject gate in GameObject.FindGameObjectsWithTag("Gate")){
            Collider gateCollider = gate.GetComponent<Collider>();
            Gate newGate = new Gate();
            newGate.Destination = int.Parse(gate.name.Substring(gate.name.Length - 1));
            newGate.Object = gate;
            newGate.Collider = gateCollider;
            GateList.Add(newGate);
        }
    }

    void FindMapItems(){
        MapItemList = new List<Item>();
        foreach (GameObject mapItem in GameObject.FindGameObjectsWithTag("MapItem")){
            Collider mapItemCollider = mapItem.GetComponent<Collider>();
            Item foundItem = GameItemList.Find(x => x.Name == mapItem.name);
            foundItem.Object = mapItem;
            foundItem.Collider = mapItemCollider;
            MapItemList.Add(foundItem);
        }
    }

    public void CollisionCheck(Collision collision){
        foreach (Gate gate in GateList){
            if (gate.Collider == collision.collider && Input.GetKey(Interact)){
                Debug.Log("Interaction triggered with Gate");
                currentScene = gate.Destination;
                if (farthestScene < gate.Destination){
                    farthestScene = gate.Destination;
                }
                SceneManager.LoadScene(SceneList[gate.Destination], LoadSceneMode.Single);
                string savedItems = "";
                foreach (Item i in Backpack.StoredItems){
                    savedItems += i.Name + ";";
                }
                gameData = new Saves.SaveData(gameDayNumber, gameTime, farthestScene, currentScene, savedItems, placedObjects);
                Saves.SaveGame(gameData);
                result = gate.Object;
            }
        }
        foreach (Item mapItem in MapItemList){ // TODO: Store in save data
            if (mapItem.Collider == collision.collider && Input.GetKey(Interact)){
                Debug.Log("Item picked up");
                Destroy(mapItem.Object);
                Backpack.StoredItems.Add(mapItem);
                UpdateToggles();
                result = mapItem.Object;
            }
        }
        if (GameObject.FindGameObjectsWithTag("Rock").Length > 0){
            GameObject[] rockArray = GameObject.FindGameObjectsWithTag("Rock");
            foreach (GameObject rock in rockArray){
                if (rock.GetComponent<Collider>() == collision.collider && Input.GetKey(UseItem)){
                    result = rock;
                }
            }
        }
        if (GameObject.FindGameObjectsWithTag("Tree").Length > 0){
            GameObject[] treeArray = GameObject.FindGameObjectsWithTag("Tree");
            foreach (GameObject tree in treeArray){
                if (tree.GetComponent<Collider>() == collision.collider && Input.GetKey(UseItem)){
                    result = tree;
                }
            }
        }
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
