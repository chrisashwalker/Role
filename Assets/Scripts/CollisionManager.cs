using UnityEngine;
using UnityEngine.SceneManagement;

public class CollisionManager : MonoBehaviour{
    public static GameObject CollidedObject;
    private void OnCollisionStay(Collision collision){
        CollidedObject = CollisionCheck(collision);
    }

    private GameObject CollisionCheck(Collision collision){
        foreach (UnityGate gate in World.GateList){
            if (gate.Collider == collision.collider && Input.GetKey(Controls.Interact)){
                Saves.GameData.CurrentLocation = gate.Destination;
                if (Saves.GameData.FarthestLocation < gate.Destination){
                    Saves.GameData.FarthestLocation = gate.Destination;
                }
                World.RockList.Clear();
                World.TreeList.Clear();
                World.MapItemList.Clear();
                Saves.GameData.InventoryItems = Inventory.StoredItems;
                Saves.SaveGame(Saves.GameData);
                SceneManager.LoadScene(World.SceneList[gate.Destination], LoadSceneMode.Single);
                return null;
            }
        }
        foreach (UnityMapItem mapItem in World.MapItemList){
            if (mapItem.Collider == collision.collider && Input.GetKey(Controls.Interact) & Inventory.StoredItems.Count < Inventory.MaxCapacity){
                Destroy(mapItem.Object);
                Saves.GameData.AlteredObjects.Add(new AlteredObject("Removal", mapItem.Object.name, SceneManager.GetActiveScene(), mapItem.Object.transform.position, mapItem.Object.GetInstanceID()));
                Inventory.StoredItems.Add(mapItem.linkedItem);
                Inventory.UpdateToggles();
                return null;
            }
        }
        if (World.RockList.Count > 0){
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
    }
}
