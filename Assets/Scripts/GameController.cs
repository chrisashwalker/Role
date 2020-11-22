using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour, IPointerClickHandler{

    public static GameController Instance;

    void Awake(){
        Instance = this;
    }

    public static void SaveGame(SaveData data){
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/save.bin";
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, data);
        stream.Close();
    }
    
    public static SaveData LoadGame(){
        string path = Application.persistentDataPath + "/save.bin";
        if (File.Exists(path)){
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            SaveData data = (SaveData) formatter.Deserialize(stream);
            stream.Close();
            return data;
        } else {
            return null;
        }
    }

    private Camera MainCamera;
    private float standardCameraSize;
    private bool cameraIsStandardSized;

    private Light Daylight;
    private float maxLightIntensity;
    private float lengthOfDay;
    private float sunrise;
    private float sunset;
    private float lightDensity; // Represents daylight gained or lost per game hour
    private float gameTime;
    private float daylightTime;
    private int gameDayNumber;
    private Saves.SaveData gameData;

    private List<AlteredObject> placedObjects = new List<AlteredObject>();

    private GameObject[] CharacterArray;
    public Character Player;
    private Vector3 RoundedPosition;

    private Inventory Backpack = Inventory.Backpack;
    private List<Item> GameItemList;
    private List<Item> MapItemList;
    private List<GameObject> Rocks;
    private List<GameObject> Trees;
    private GameObject FullCanvas;
    public GameObject ItemToggle; // Toggle prefab set via Unity object settings
    public GameObject Prepared;
    public GameObject Placed;
    public GameObject Ready;

    private float toggleWidth = 100.0f;
    private float toggleHeight = 25.0f;

    public Texture2D ProjectileTexture; // Texture assigned via Unity object settings
    private Projectile ShotProjectile;
    private GameObject ShotProjectileObject;
    private Vector3 ShotProjectileStart;

    private int farthestScene; // Represents how far the player has travelled
    private int currentScene;
    private Dictionary<int, string> SceneList;
    private List<Gate> GateList;

    // Key controls, excluding movement inputs
    private KeyCode MapZoom = KeyCode.V;
    private KeyCode Interact = KeyCode.E;
    private KeyCode ScrollLeft = KeyCode.Comma;
    private KeyCode ScrollRight = KeyCode.Period;
    private KeyCode UseItem = KeyCode.Slash;

    public GameObject targetPrefab;
    private GameObject target;
    private float cellSize = 1.0f;
    private Terrain terrain;
    private GameObject result;
    public LayerMask interactiveLayer;


    void LoadGameItems(){
        GameItemList = new List<Item>();
        GameItemList.Add(Tool.Sword);
        GameItemList.Add(Tool.Shovel);
        GameItemList.Add(Tool.Pickaxe);
        GameItemList.Add(Tool.Axe);
        GameItemList.Add(Tool.WateringCan);
        GameItemList.Add(Projectile.Bow);
        GameItemList.Add(Tool.Seed);
        GameItemList.Add(Item.Stone);
        GameItemList.Add(Item.Wood);
    }

    void LoadStandardItems(){
        Backpack.StoredItems.Add(Tool.Sword);
        Backpack.StoredItems.Add(Tool.Shovel);
        Backpack.StoredItems.Add(Tool.Pickaxe);
        Backpack.StoredItems.Add(Tool.Axe);
        Backpack.StoredItems.Add(Tool.WateringCan);
        Backpack.StoredItems.Add(Projectile.Bow);
        Backpack.StoredItems.Add(Tool.Seed);
    }

    public void OnPointerClick(PointerEventData pointerEventData){
        GameObject clickedToggle = pointerEventData.pointerPress;
        if (clickedToggle.tag == "ItemToggle"){
            foreach (Item item in Backpack.StoredItems){
                if (clickedToggle.GetComponentInChildren<Text>().text == item.Name){
                    Backpack.EquippedItemIndex = Backpack.StoredItems.IndexOf(item);
                }
            }
            UpdateToggles();
        }
    }

    void UpdateToggles(){
        FullCanvas = GameObject.FindWithTag("FullCanvas");
        GameObject[] allItemToggles = GameObject.FindGameObjectsWithTag("ItemToggle");
        foreach (GameObject toggle in allItemToggles){
            toggle.SetActive(false);
            GameObject.Destroy(toggle);
        }
        foreach (Item item in Backpack.StoredItems){
            GameObject newToggleObject = Instantiate(ItemToggle);
            newToggleObject.tag = "ItemToggle";
            newToggleObject.transform.SetParent(FullCanvas.transform, false);
            string itemLabel;
            int itemDurability;
            if (item.Type == ItemTypes.TOOL){
                Tool thisTool = (Tool) item;
                itemDurability = thisTool.Durability;
                if (itemDurability > 0){
                    itemLabel = thisTool.Name + " (" + itemDurability + ")";
                } else{
                    itemLabel = item.Name;
                }
            } else {
                itemLabel = item.Name;
            }
            newToggleObject.GetComponentInChildren<Text>().text = itemLabel;
        }
        allItemToggles = GameObject.FindGameObjectsWithTag("ItemToggle");
        int toggleCount = allItemToggles.Length;
        foreach (GameObject toggle in allItemToggles){
            int toggleIndex = System.Array.IndexOf(allItemToggles, toggle);
            float positionFromCenter = toggleIndex - ((float) toggleCount / 2) + 0.5f;
            toggle.GetComponent<RectTransform>().anchoredPosition = new Vector2(positionFromCenter * toggleWidth,toggleHeight);
            if (toggleIndex == Backpack.EquippedItemIndex){
                toggle.GetComponentInChildren<Text>().text = toggle.GetComponentInChildren<Text>().text.ToUpper();
            }
        }
    }

    void ShootProjectile(Character shooter){
        ShotProjectile = (Projectile) Backpack.StoredItems[Backpack.EquippedItemIndex];
        ShotProjectileObject = new GameObject("ShotProjectile");
        SpriteRenderer ShotProjectileSpriteRenderer = ShotProjectileObject.AddComponent<SpriteRenderer>();
        Rigidbody ShotProjectileRigidbody = ShotProjectileObject.AddComponent<Rigidbody>();
        ShotProjectileRigidbody.useGravity = false;
        Sprite ShotProjectileSprite = Sprite.Create(ProjectileTexture, new Rect(0, 0, 128.0f, 256.0f), new Vector2(0.5f, 0.5f), 256.0f);
        ShotProjectileSpriteRenderer.sprite = ShotProjectileSprite;
        ShotProjectileSpriteRenderer.sortingLayerName = "Player";
        int projectileShiftX = 0, projectileShiftY = 0, projectileShiftZ = 0;
        float projectileVelocityX = 0.0f, projectileVelocityY = 0.0f, projectileVelocityZ = 0.0f;
        if (Player.Direction == 'D'){
            projectileVelocityZ = -24.0f;
        } else if (Player.Direction == 'U'){
            projectileVelocityZ = 24.0f;
        } else if (Player.Direction == 'L'){
            projectileVelocityX = -24.0f;
        } else if (Player.Direction == 'R'){
            projectileVelocityX = 24.0f;
        }
        ShotProjectileObject.transform.position = new Vector3(shooter.Rigidbody.position.x + projectileShiftX, shooter.Rigidbody.position.y + projectileShiftY, shooter.Rigidbody.position.z + projectileShiftZ);
        ShotProjectileRigidbody.velocity = new Vector3(projectileVelocityX, projectileVelocityY, projectileVelocityZ);
        ShotProjectileStart = ShotProjectileObject.transform.position;            
    }

    void findTarget(){
        if (target == null){
            target = Instantiate(targetPrefab);
        }
        float targetX, targetY, targetZ;
        targetY = 0.04f;
        if (Player.Direction == 'D'){
            targetX = (float) System.Math.Floor(Player.Rigidbody.position.x / cellSize) * cellSize + cellSize;
            targetZ = (float) System.Math.Floor(Player.Rigidbody.position.z / cellSize) * cellSize - cellSize;
        } else if (Player.Direction == 'U'){
            targetX = (float) System.Math.Floor(Player.Rigidbody.position.x / cellSize) * cellSize + cellSize;
            targetZ = (float) System.Math.Ceiling(Player.Rigidbody.position.z / cellSize) * cellSize + cellSize;
        } else if (Player.Direction == 'L'){
            targetX = (float) System.Math.Floor(Player.Rigidbody.position.x / cellSize) * cellSize - cellSize;
            targetZ = (float) System.Math.Floor(Player.Rigidbody.position.z / cellSize) * cellSize + cellSize;
        } else {
            targetX = (float) System.Math.Ceiling(Player.Rigidbody.position.x / cellSize) * cellSize + cellSize;
            targetZ = (float) System.Math.Floor(Player.Rigidbody.position.z / cellSize) * cellSize + cellSize;
        }
        target.transform.position = new Vector3(targetX,targetY,targetZ);
    }

    void UseTool(Character worker){
        Tool UsedTool = (Tool) Backpack.StoredItems[Backpack.EquippedItemIndex];
        ToolFunctions UsedToolFunction = UsedTool.Function;
        Collider[] hitColliders = Physics.OverlapBox(target.transform.position, new Vector3(0.1f,0.1f,0.1f), Quaternion.identity, interactiveLayer);
        GameObject hitCollider = null;
        if (hitColliders.Length > 0){
            hitCollider = hitColliders[0].gameObject;
        }
        if (hitCollider == null){
            if (UsedToolFunction.Equals(ToolFunctions.WATER)){
                GameObject prepared = Instantiate(Prepared);
                prepared.transform.position = target.transform.position;
                placedObjects.Add(new AlteredObject(Prepared.name, SceneManager.GetActiveScene(), prepared.transform.position, prepared.GetHashCode()));
            }
        } else {
            if (UsedToolFunction.Equals(ToolFunctions.SEED) && hitCollider.tag == "Prepared"){
                GameObject.Destroy(hitCollider);
                GameObject placed = Instantiate(Placed);
                placed.transform.position = target.transform.position;
                placedObjects.Add(new AlteredObject(Placed.name, SceneManager.GetActiveScene(), placed.transform.position, placed.GetHashCode()));
                if (UsedTool.Durability > 1){
                    UsedTool.Durability -= 1;
                } else {
                    Backpack.StoredItems.Remove(UsedTool);
                    Backpack.EquippedItemIndex = 0;
                }
            } else if (Backpack.MaxCapacity > Backpack.StoredItems.Count){
                if (UsedToolFunction.Equals(ToolFunctions.SHOVEL) && hitCollider.tag == "Placed"){
                    GameObject.Destroy(hitCollider);
                    placedObjects.Remove(placedObjects.Find(x => x.Identifier.Equals(hitCollider.GetHashCode())));
                    if (Backpack.StoredItems.Contains(Tool.Seed)){
                        Tool collectedSeed = (Tool) Backpack.StoredItems.Find(x => x.Equals(Tool.Seed));
                        collectedSeed.Durability += 1;
                    } else {
                        Backpack.StoredItems.Add(Tool.Seed);
                    }
                } else if (UsedToolFunction.Equals(ToolFunctions.SHOVEL) && Backpack.MaxCapacity >= Backpack.StoredItems.Count + Food.Plant.Strength  && hitCollider.tag == "Ready"){
                    GameObject.Destroy(hitCollider);
                    placedObjects.Remove(placedObjects.Find(x => x.Identifier.Equals(hitCollider.GetHashCode())));
                    for (int i = 0; i < Food.Plant.Strength; i++){
                        Backpack.StoredItems.Add(Food.Plant);
                    }
                } else if (hitCollider.tag == "Rock" && UsedToolFunction.Equals(ToolFunctions.PICKAXE)){
                    GameObject.Destroy(hitCollider);
                    Backpack.StoredItems.Add(Item.Stone);
                } else if (hitCollider.tag == "Tree" && UsedToolFunction.Equals(ToolFunctions.AXE)){
                    GameObject.Destroy(hitCollider);
                    Backpack.StoredItems.Add(Item.Wood);
                }
            }
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
        Player.Type.Add(CharacterTypes.PLAYER);

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
