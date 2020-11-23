using UnityEngine;
using UnityEngine.SceneManagement;

public class CollisionManager : MonoBehaviour{
    private void OnCollisionStay(Collision collision){
        CollisionCheck(collision);
    }

    private void CollisionCheck(Collision collision){
        UnityCharacter character = null; 
        foreach (UnityCharacter uc in World.CharacterList){
            if (uc.Collider == this.GetComponent<Collider>()){
                character = uc;
            }
        }
        if (character != null){
            foreach (UnityGate gate in World.GateList){
                if (gate.Collider == collision.collider && Input.GetKey(Controls.Interact)){
                    Saves.GameData.CurrentLocation = gate.Destination;
                    if (Saves.GameData.FarthestLocation < gate.Destination){
                        Saves.GameData.FarthestLocation = gate.Destination;
                    }
                    SceneManager.LoadScene(World.SceneList[gate.Destination], LoadSceneMode.Single);
                    string savedItems = "";
                    foreach (Item i in Inventory.Backpack.StoredItems){
                        savedItems += i.Name + ";";
                    }
                    Saves.GameData = new Saves.SaveData(Saves.GameData.GameDay, Saves.GameData.GameTime, Saves.GameData.FarthestLocation, Saves.GameData.CurrentLocation, savedItems, Saves.GameData.AlteredObjects);
                    Saves.SaveGame(Saves.GameData);
                }
            }
            foreach (UnityMapItem mapItem in World.MapItemList){
                if (mapItem.Collider == collision.collider && Input.GetKey(Controls.Interact)){
                    Destroy(mapItem.Object);
                    Inventory.Backpack.StoredItems.Add(mapItem.linkedItem);
                    Inventory.UpdateToggles();
                }
            }
            if (GameObject.FindGameObjectsWithTag("Rock").Length > 0){
                GameObject[] rockArray = GameObject.FindGameObjectsWithTag("Rock");
                foreach (GameObject rock in rockArray){
                    if (rock.GetComponent<Collider>() == collision.collider){
                        Actions.UseTool(character, rock);
                    }
                }
            }
            if (GameObject.FindGameObjectsWithTag("Tree").Length > 0){
                GameObject[] treeArray = GameObject.FindGameObjectsWithTag("Tree");
                foreach (GameObject tree in treeArray){
                    if (tree.GetComponent<Collider>() == collision.collider){
                        Actions.UseTool(character, tree);
                    }
                }
            }
        }
    }
}