using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

public static class Actions{
    public static LayerMask InteractiveLayer{get;} = LayerMask.GetMask("Interactable");
    public static GameObject Target{get;set;}
    public static float cellSize{get;} = 1.0f;
    public static List<UnityProjectile> ShotProjectiles{get;set;} = new List<UnityProjectile>();

    public static void FindTarget(){
        if (Target == null){
            Target = GameObject.Instantiate(Resources.Load<GameObject>("Target"));
        }
        float targetX, targetY, targetZ;
        targetY = 0.04f;
        if (GameController.Instance.Player.Rigidbody.rotation.z < 90){
            targetX = (float) System.Math.Floor(GameController.Instance.Player.Rigidbody.position.x / cellSize) * cellSize + cellSize;
            targetZ = (float) System.Math.Floor(GameController.Instance.Player.Rigidbody.position.z / cellSize) * cellSize - cellSize;
        } else if (GameController.Instance.Player.Rigidbody.rotation.z < 180){
            targetX = (float) System.Math.Floor(GameController.Instance.Player.Rigidbody.position.x / cellSize) * cellSize - cellSize;
            targetZ = (float) System.Math.Floor(GameController.Instance.Player.Rigidbody.position.z / cellSize) * cellSize + cellSize;
        } else if (GameController.Instance.Player.Rigidbody.rotation.z < 270){
            targetX = (float) System.Math.Floor(GameController.Instance.Player.Rigidbody.position.x / cellSize) * cellSize + cellSize;
            targetZ = (float) System.Math.Ceiling(GameController.Instance.Player.Rigidbody.position.z / cellSize) * cellSize + cellSize;
        } else {
            targetX = (float) System.Math.Ceiling(GameController.Instance.Player.Rigidbody.position.x / cellSize) * cellSize + cellSize;
            targetZ = (float) System.Math.Floor(GameController.Instance.Player.Rigidbody.position.z / cellSize) * cellSize + cellSize;
        }
        Target.transform.position = new Vector3(targetX,targetY,targetZ);
    }

    public static void UseTool(GameObject collidedObject){
        Tool UsedTool = (Tool) Inventory.StoredItems[Inventory.EquippedItemIndex];
        ToolFunctions UsedToolFunction = UsedTool.Function;
        Collider[] hitColliders = Physics.OverlapBox(Target.transform.position, new Vector3(0.1f,0.1f,0.1f), Quaternion.identity, InteractiveLayer);
        GameObject hitCollider = null;
        if (hitColliders.Length > 0){
            hitCollider = hitColliders[0].gameObject;
        }
        if (hitCollider == null){
            if (UsedToolFunction.Equals(ToolFunctions.WATER)){
                GameObject Prepared = GameObject.Instantiate(Resources.Load<GameObject>("Prepared"));
                Prepared.transform.position = Target.transform.position;
                Saves.GameData.AlteredObjects.Add(new AlteredObject(Prepared.name, SceneManager.GetActiveScene(), Prepared.transform.position, Prepared.GetHashCode()));
            }
        } else {
            if (UsedToolFunction.Equals(ToolFunctions.SEED) && hitCollider.tag == "Prepared"){
                GameObject.Destroy(hitCollider);
                GameObject Placed = GameObject.Instantiate(Resources.Load<GameObject>("Placed"));
                Placed.transform.position = Target.transform.position;
                Saves.GameData.AlteredObjects.Add(new AlteredObject(Placed.name, SceneManager.GetActiveScene(), Placed.transform.position, Placed.GetHashCode()));
                if (UsedTool.Durability > 1){
                    UsedTool.Durability -= 1;
                } else {
                    Inventory.StoredItems.Remove(UsedTool);
                    Inventory.EquippedItemIndex = 0;
                }
            } else if (Inventory.MaxCapacity > Inventory.StoredItems.Count){
                if (UsedToolFunction.Equals(ToolFunctions.SHOVEL) && hitCollider.tag == "Placed"){
                    GameObject.Destroy(hitCollider);
                    Saves.GameData.AlteredObjects.Remove(Saves.GameData.AlteredObjects.Find(x => x.Identifier.Equals(hitCollider.GetHashCode())));
                    if (Inventory.StoredItems.Contains(ItemList.Seed)){
                        Tool collectedSeed = (Tool) Inventory.StoredItems.Find(x => x.Equals(ItemList.Seed));
                        collectedSeed.Durability += 1;
                    } else {
                        Inventory.StoredItems.Add(ItemList.Seed);
                    }
                } else if (UsedToolFunction.Equals(ToolFunctions.SHOVEL) && Inventory.MaxCapacity >= Inventory.StoredItems.Count + ItemList.Plant.Strength  && hitCollider.tag == "Ready"){
                    GameObject.Destroy(hitCollider);
                    Saves.GameData.AlteredObjects.Remove(Saves.GameData.AlteredObjects.Find(x => x.Identifier.Equals(hitCollider.GetHashCode())));
                    for (int i = 0; i < ItemList.Plant.Strength; i++){
                        Inventory.StoredItems.Add(ItemList.Plant);
                    }
                } else if (hitCollider.tag == "Rock" && UsedToolFunction.Equals(ToolFunctions.PICKAXE) && Input.GetKey(Controls.UseItem)){
                    GameObject.Destroy(hitCollider);
                    Inventory.StoredItems.Add(ItemList.Stone);
                } else if (hitCollider.tag == "Tree" && UsedToolFunction.Equals(ToolFunctions.AXE) && Input.GetKey(Controls.UseItem)){
                    GameObject.Destroy(hitCollider);
                    Inventory.StoredItems.Add(ItemList.Wood);
                }
            }
        }
    }

    public static void ShootProjectile(UnityCharacter shooter){
        UnityProjectile projectile = new UnityProjectile("Projectile");
        ShotProjectiles.Add(projectile);
        projectile.Origin = shooter.Rigidbody.position;
        projectile.Rigidbody.transform.position = projectile.Origin;
        projectile.Rigidbody.AddForce(projectile.Rigidbody.transform.forward * 20, ForceMode.Impulse);
    }
}
