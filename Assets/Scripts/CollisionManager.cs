using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class CollisionManager : MonoBehaviour{
    public static GameObject CollidedObject;

    private void OnCollisionStay(Collision collision){
        if (collision.gameObject.tag == "Ground"){
            GameController.Instance.Player.Grounded = true;
        }
        CollidedObject = CollisionCheck(collision);
    }

    private void OnCollisionExit(Collision collision){
        if (collision.gameObject.tag == "Ground"){
            GameController.Instance.Player.Grounded = false;
        }
    }
    
    private GameObject CollisionCheck(Collision collision){
        if (collision.gameObject.tag == "Bed" && Input.GetKey(Controls.Interact) && DateTime.Now >= Saves.LastSave.AddSeconds(3)){
            GameController.Instance.Player.Health = GameController.Instance.Player.MaxHealth;
            Saves.GameData.GameDay += 1;
            Saves.GameData.GameTime = TimeManager.sunrise;
            Saves.GameData.InventoryItems = GameController.Instance.Player.Storage.StoredItems;
            Saves.GameData.Funds = GameController.Instance.Player.Coins;
            Saves.SaveGame(Saves.GameData);
            Saves.LastSave = DateTime.Now;
        }
        foreach (UnityProjectile projectile in Actions.ShotProjectiles){
            if (projectile.Collider == collision.collider){
                Actions.SpentProjectiles.Add(projectile);
                GameController.Instance.Player.Health -= 1;
                GameController.Instance.Player.Rigidbody.AddForce(new Vector3(0,10,0), ForceMode.Impulse);
            }
        }
        foreach (UnityCharacter character in World.CharacterList){
            if (character.Collider == collision.collider && Input.GetKey(Controls.Buy)){
                Trading.FindSaleItems(GameController.Instance.Player, character);
                return null;
            }
            if (character.Collider == collision.collider && Input.GetKey(Controls.Sell)){
                Trading.FindSaleItems(character, GameController.Instance.Player);
                return null;
            }
        }
        foreach (UnityGate gate in World.GateList){
            if (gate.Collider == collision.collider && Input.GetKey(Controls.Interact)){
                Saves.GameData.CurrentLocation = gate.Destination;
                if (Saves.GameData.Progress < gate.Destination){
                    Saves.GameData.Progress = gate.Destination;
                }
                World.RockList.Clear();
                World.TreeList.Clear();
                World.MapItemList.Clear();
                Saves.GameData.InventoryItems = GameController.Instance.Player.Storage.StoredItems;
                Saves.GameData.Funds = GameController.Instance.Player.Coins;
                Saves.SaveGame(Saves.GameData);
                SceneManager.LoadScene(World.SceneList[gate.Destination], LoadSceneMode.Single);
                return null;
            }
        }
        foreach (UnityMapItem mapItem in World.MapItemList){
            if (mapItem.Collider == collision.collider && Input.GetKey(Controls.Interact) & GameController.Instance.Player.Storage.StoredItems.Count < GameController.Instance.Player.Storage.MaxCapacity){
                Destroy(mapItem.Object);
                Saves.GameData.AlteredObjects.Add(new AlteredObject("Removal", mapItem.Object.name, SceneManager.GetActiveScene(), mapItem.Object.transform.position, mapItem.Object.GetInstanceID()));
                GameController.Instance.Player.Storage.StoredItems.Add(mapItem.linkedItem);
                Items.UpdateToggles();
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
