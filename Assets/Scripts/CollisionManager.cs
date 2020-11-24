using UnityEngine;
using UnityEngine.SceneManagement;

public class CollisionManager : MonoBehaviour{
    public static GameObject CollidedObject;
    private void OnCollisionStay(Collision collision){
        CollidedObject = CollisionCheck(collision);
    }

    private GameObject CollisionCheck(Collision collision){
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
                    foreach (Item i in Inventory.StoredItems){
                        savedItems += i.Name + ";";
                    }
                    Saves.GameData = new Saves.SaveData(Saves.GameData.GameDay, Saves.GameData.GameTime, Saves.GameData.FarthestLocation, Saves.GameData.CurrentLocation, savedItems, Saves.GameData.AlteredObjects);
                    Saves.SaveGame(Saves.GameData);
                    return null;
                }
            }
            foreach (UnityMapItem mapItem in World.MapItemList){
                if (mapItem.Collider == collision.collider && Input.GetKey(Controls.Interact)){
                    Destroy(mapItem.Object);
                    Inventory.StoredItems.Add(mapItem.linkedItem);
                    Inventory.UpdateToggles();
                    return null;
                }
            }
            if (World.TreeList.Count > 0){
                foreach (GameObject rock in World.RockList){
                    if (rock.GetComponent<Collider>() == collision.collider){
                        return rock;
                    }
                }
            }
            if (World.TreeList.Count > 0){
                foreach (GameObject tree in World.TreeList){
                    if (tree.GetComponent<Collider>() == collision.collider){
                        return tree;
                    }
                }
            }
            return null;
        } else {
            return null;
        }
    }
}
