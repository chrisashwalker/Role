using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour{
    public static GameController Instance;
    public UnityCharacter Player;

    void Awake(){
        Instance = this;
        World.BuildScenes();
        Inventory.LoadGameItems();
        Saves.GameData = Saves.LoadGame();
        if (Saves.GameData != null){
            if (Saves.Loaded == false){ 
                SceneManager.LoadScene(World.SceneList[Saves.GameData.CurrentLocation]);
                Saves.Loaded = true;
            }
            string savedItems = Saves.GameData.InventoryItems;
            List<string> SavedItemsList = new List<string>(savedItems.Split(';'));
            if (savedItems == ""){
                Inventory.LoadStandardItems();
            } else if (Inventory.StoredItems.Count == 0) {
                foreach (string itemName in SavedItemsList){
                    if (Inventory.GameItemList.Find(x => x.Name == itemName) != null){
                        Inventory.StoredItems.Add(Inventory.GameItemList.Find(x => x.Name == itemName));
                    }
                }
            }
            Saves.GameData.AlteredObjects = Saves.GameData.AlteredObjects;
        } else {
            Saves.GameData.GameDay = 1;
            Saves.GameData.GameTime = 0.0f;
            Saves.GameData.FarthestLocation = 1;
            Saves.GameData.CurrentLocation = 1;
            Inventory.LoadStandardItems();
        }
        foreach (AlteredObject po in Saves.GameData.AlteredObjects){
            if (po.Scene == SceneManager.GetActiveScene().name){
                GameObject loadedPo = Instantiate(Resources.Load(po.Prefab, typeof(GameObject))) as GameObject;
                loadedPo.transform.position = new Vector3(po.PositionX, po.PositionY, po.PositionZ);
            }
        }
        Inventory.EquippedItemIndex = 0;
        Inventory.UpdateToggles();
        World.FindCharacters();
        World.FindGates();
        World.FindMapItems();
        World.FindObjects();
    }

    void FixedUpdate(){
        Controls.MoveCharacter(Player);
        Actions.FindTarget();
         if (Input.GetKeyDown(Controls.MapZoom)){
            CameraManager.MapToggle();
        } else if (Input.GetKeyDown(KeyCode.Alpha1)){
            World.FastTravel(1);
        } else if (Input.GetKeyDown(KeyCode.Alpha2)){
            World.FastTravel(2);
        }
        Inventory.ItemUseCheck();
        foreach (UnityProjectile projectile in Actions.ShotProjectiles){
            if ((projectile.Rigidbody.transform.position - projectile.Origin).magnitude >= projectile.Distance){
                GameObject.Destroy(projectile.Object);
                Actions.ShotProjectiles.Remove(projectile);
            }
        }
    }

    void Update(){
        TimeManager.ClockTick();
    }
}
