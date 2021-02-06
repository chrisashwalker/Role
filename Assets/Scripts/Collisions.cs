using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class Collisions : MonoBehaviour
{
    public static GameObject CollidedObject {get; set;}

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == Tags.Ground && this.gameObject.tag == Tags.Player)        
        {
            Map.Player.Grounded = true;
        }
        Control.GetKeyPress();
        CollidedObject = CollisionCheck(collision);
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == Tags.Ground && this.gameObject.tag == Tags.Player)
        {
            Map.Player.Grounded = false;
        }
    }
    
    private GameObject CollisionCheck(Collision collision)
    {
        if (collision.gameObject.tag == Tags.Bed)
        {
            return CollisionWithBed(collision);
        }
        else if (collision.gameObject.tag == Tags.Projectile)
        {
            return CollisionWithProjectile(collision);
        }
        else if (collision.gameObject.tag == Tags.Character)
        {
            return CollisionWithCharacter(collision);
        }
        else if (collision.gameObject.tag == Tags.Gate)
        {
            return CollisionWithGate(collision);
        }
        else if (collision.gameObject.tag == Tags.MapItem)
        {
            return CollisionWithMapItem(collision);
        }
        else if (collision.gameObject.tag == Tags.Rock)
        {
            return CollisionWithRock(collision);
        }
        else if (collision.gameObject.tag == Tags.Tree)
        {
            return CollisionWithTree(collision);
        }
        else 
        {
            return null;
        }
    }

    private GameObject CollisionWithBed(Collision collision)
    {
        if (Control.PressedKey == Control.Interact && DateTime.Now >= Saves.LastSave.AddSeconds(3))
        {
            Map.Player.Health = Map.Player.MaxHealth;
            Saves.GameState.GameDay += 1;
            Saves.GameState.GameTime = Timeflow.Sunrise;
            Saves.GameState.InventoryItems = Map.Player.Storage.StoredItems;
            Saves.GameState.Funds = Map.Player.Coins;
            Saves.SaveGame(Saves.GameState);
            Saves.LastSave = DateTime.Now;
        }
            return null;
    }
    
    private GameObject CollisionWithProjectile(Collision collision)
    {
        foreach (UnityProjectile projectile in Map.ShotProjectiles)
        {
            if (projectile.Collider == collision.collider)
            {
                Map.SpentProjectiles.Add(projectile);
                if (gameObject.tag == Tags.Player)
                {
                    Map.Player.Health -= 1;
                    Map.Player.Rigidbody.AddForce(new Vector3(0,10,0), ForceMode.Impulse); 
                }
                else if (gameObject.tag == Tags.Enemy)
                {
                    foreach (UnityCharacter enemy in Map.Enemies)
                    {
                        if (enemy.Object == this.gameObject)
                        {
                            enemy.Health -= 1;
                        }
                    }
                }
            }
        }
        return null;
    }

    private GameObject CollisionWithCharacter(Collision collision)
    {
        foreach (UnityCharacter character in Map.Characters)
        {
            if (character.Collider == collision.collider && Control.PressedKey == Control.Buy)
            {
                Trading.FindSaleItems(Map.Player, character);
            }
            else if (character.Collider == collision.collider && Control.PressedKey == Control.Sell)
            {
                Trading.FindSaleItems(character, Map.Player);
            }
        }
        return null;
    }

    private GameObject CollisionWithGate(Collision collision)
    {
        foreach (UnityGate gate in Map.Gates){
            if (gate.Collider == collision.collider && Control.PressedKey == Control.Interact)
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
            }
        }
        return null;
    }

    private GameObject CollisionWithMapItem(Collision collision)
    {
        foreach (UnityMapItem mapItem in Map.MapItems)
        {
            if (mapItem.Collider == collision.collider && Control.PressedKey == Control.Interact & Map.Player.Storage.StoredItems.Count < Map.Player.Storage.MaxCapacity)
            {
                Destroy(mapItem.Object);
                Saves.GameState.AlteredObjects.Add(new AlteredObject("Removal", mapItem.Object.name, SceneManager.GetActiveScene(), mapItem.Object.transform.position, mapItem.Object.GetInstanceID()));
                Map.Player.Storage.StoredItems.Add(mapItem.linkedItem);
                Items.UpdateToggles();
            }
        }
        return null;
    }

    private GameObject CollisionWithRock(Collision collision)
    {
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
        return null;
    }

    private GameObject CollisionWithTree(Collision collision)
    {
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
