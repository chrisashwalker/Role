using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class Collisions : MonoBehaviour
{
    public static GameObject CollidedObject {get; set;}

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Ground" && this.gameObject.tag == "Player")        
        {
            Map.Player.Grounded = true;
        }
        CollidedObject = CollisionCheck(collision);
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Ground" && this.gameObject.tag == "Player")
        {
            Map.Player.Grounded = false;
        }
    }
    
    private GameObject CollisionCheck(Collision collision)
    {
        if (collision.gameObject.tag == "Bed" && Input.GetKey(Control.Interact) && DateTime.Now >= Saves.LastSave.AddSeconds(3))
        {
            Map.Player.Health = Map.Player.MaxHealth;
            Saves.GameState.GameDay += 1;
            Saves.GameState.GameTime = Timeflow.Sunrise;
            Saves.GameState.InventoryItems = Map.Player.Storage.StoredItems;
            Saves.GameState.Funds = Map.Player.Coins;
            Saves.SaveGame(Saves.GameState);
            Saves.LastSave = DateTime.Now;
            return null;
        }
        foreach (UnityProjectile projectile in Map.ShotProjectiles)
        {
            if (projectile.Collider == collision.collider)
            {
                Map.SpentProjectiles.Add(projectile);
                if (gameObject.tag == "Player")
                {
                   Map.Player.Health -= 1;
                Map.Player.Rigidbody.AddForce(new Vector3(0,10,0), ForceMode.Impulse); 
                }
                else if (gameObject.tag == "Enemy")
                {
                    foreach (UnityCharacter enemy in Map.Enemies)
                    {
                        if (enemy.Object == this.gameObject)
                        {
                            enemy.Health -= 1;
                        }
                    }
                }
                return null;
            }
        }
        foreach (UnityCharacter character in Map.Characters)
        {
            if (character.Collider == collision.collider && Input.GetKey(Control.Buy))
            {
                Trading.FindSaleItems(Map.Player, character);
                return null;
            }
            if (character.Collider == collision.collider && Input.GetKey(Control.Sell))
            {
                Trading.FindSaleItems(character, Map.Player);
                return null;
            }
        }
        foreach (UnityGate gate in Map.Gates){
            if (gate.Collider == collision.collider && Input.GetKey(Control.Interact))
            {
                Saves.GameState.CurrentLocation = gate.Destination;
                if (Saves.GameState.Progress < gate.Destination)
                {
                    Saves.GameState.Progress = gate.Destination;
                }
                Map.Rocks.Clear();
                Map.Trees.Clear();
                Map.MapItems.Clear();
                Saves.GameState.InventoryItems = Map.Player.Storage.StoredItems;
                Saves.GameState.Funds = Map.Player.Coins;
                Saves.SaveGame(Saves.GameState);
                SceneManager.LoadScene(Map.Scenes[gate.Destination], LoadSceneMode.Single);
                return null;
            }
        }
        foreach (UnityMapItem mapItem in Map.MapItems)
        {
            if (mapItem.Collider == collision.collider && Input.GetKey(Control.Interact) & Map.Player.Storage.StoredItems.Count < Map.Player.Storage.MaxCapacity)
            {
                Destroy(mapItem.Object);
                Saves.GameState.AlteredObjects.Add(new AlteredObject("Removal", mapItem.Object.name, SceneManager.GetActiveScene(), mapItem.Object.transform.position, mapItem.Object.GetInstanceID()));
                Map.Player.Storage.StoredItems.Add(mapItem.linkedItem);
                Items.UpdateToggles();
                return null;
            }
        }
        if (Map.Rocks.Count > 0)
        {
            foreach (GameObject rock in Map.Rocks)
            {
                if (rock.GetComponent<Collider>() == collision.collider)
                {
                    return rock;
                }
            }
        }
        if (Map.Trees.Count > 0)
        {
            foreach (GameObject tree in Map.Trees)
            {
                if (tree.GetComponent<Collider>() == collision.collider)
                {
                    return tree;
                }
            }
        }
        return null;
    }
}
