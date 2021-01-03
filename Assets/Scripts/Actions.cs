using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

public static class Actions{
    public static LayerMask InteractiveLayer{get;set;}
    public static GameObject Target{get;set;}
    public static float cellSize{get;} = 1.0f;
    public static List<UnityProjectile> ShotProjectiles{get;set;} = new List<UnityProjectile>();
    public static List<UnityProjectile> SpentProjectiles{get;set;} = new List<UnityProjectile>();

    public static void FindTarget(){
        if (Target == null){
            Target = GameObject.Instantiate(Resources.Load<GameObject>("Target"));
        }
        float targetX, targetY, targetZ;
        targetY = 0.04f;
        if (GameController.Instance.Player.Rigidbody.rotation.eulerAngles.y < 90){
            targetX = (float) System.Math.Floor(GameController.Instance.Player.Rigidbody.position.x / cellSize) * cellSize + cellSize;
            targetZ = (float) System.Math.Ceiling(GameController.Instance.Player.Rigidbody.position.z / cellSize) * cellSize + cellSize;
        } else if  (GameController.Instance.Player.Rigidbody.rotation.eulerAngles.y < 180){
            targetX = (float) System.Math.Ceiling(GameController.Instance.Player.Rigidbody.position.x / cellSize) * cellSize + cellSize;
            targetZ = (float) System.Math.Floor(GameController.Instance.Player.Rigidbody.position.z / cellSize) * cellSize + cellSize;
        } else if (GameController.Instance.Player.Rigidbody.rotation.eulerAngles.y < 270){
            targetX = (float) System.Math.Floor(GameController.Instance.Player.Rigidbody.position.x / cellSize) * cellSize + cellSize;
            targetZ = (float) System.Math.Floor(GameController.Instance.Player.Rigidbody.position.z / cellSize) * cellSize - cellSize;
        } else {
            targetX = (float) System.Math.Floor(GameController.Instance.Player.Rigidbody.position.x / cellSize) * cellSize - cellSize;
            targetZ = (float) System.Math.Floor(GameController.Instance.Player.Rigidbody.position.z / cellSize) * cellSize + cellSize;
        }
        Target.transform.position = new Vector3(targetX,targetY,targetZ);
        Target.transform.rotation = GameController.Instance.Player.Rigidbody.rotation;
    }

    public static void UseTool(GameObject collidedObject){
        Tool UsedTool = (Tool) GameController.Instance.Player.Storage.StoredItems[GameController.Instance.Player.Storage.EquippedItemIndex];
        ToolFunctions UsedToolFunction = UsedTool.Function;
        Collider[] hitColliders = Physics.OverlapBox(Target.transform.position, new Vector3(0.1f,0.1f,0.1f), Quaternion.identity, InteractiveLayer);
        GameObject hitCollider = null;
        if (hitColliders.Length > 0){
            hitCollider = hitColliders[0].gameObject;
        }
        if (hitCollider == null){
            if (UsedToolFunction.Equals(ToolFunctions.WATER)){
                GameObject Soil = GameObject.Instantiate(Resources.Load<GameObject>("Soil"));
                Soil.transform.position = Target.transform.position;
                Saves.GameData.AlteredObjects.Add(new AlteredObject("Addition", Soil.name, SceneManager.GetActiveScene(), Soil.transform.position, Soil.GetInstanceID()));
            }
        } else {
            if (UsedToolFunction.Equals(ToolFunctions.SEED) && hitCollider.tag == "Soil"){
                Saves.GameData.AlteredObjects.Remove(Saves.GameData.AlteredObjects.Find(x => x.Identifier.Equals(hitCollider.GetInstanceID())));
                GameObject.Destroy(hitCollider);
                GameObject Plant = GameObject.Instantiate(Resources.Load<GameObject>("CarrotStart"));
                Plant.transform.position = Target.transform.position;
                Saves.GameData.AlteredObjects.Add(new AlteredObject("Addition", Plant.name, SceneManager.GetActiveScene(), Plant.transform.position, Plant.GetInstanceID()));
                if (UsedTool.Durability > 1){
                    UsedTool.Durability -= 1;
                } else {
                    GameController.Instance.Player.Storage.StoredItems.Remove(UsedTool);
                    GameController.Instance.Player.Storage.EquippedItemIndex = 0;
                }
            } else if (GameController.Instance.Player.Storage.MaxCapacity > GameController.Instance.Player.Storage.StoredItems.Count){
                if (UsedToolFunction.Equals(ToolFunctions.SHOVEL) && hitCollider.tag == "Plant"){
                    GameObject.Destroy(hitCollider);
                    Saves.GameData.AlteredObjects.Remove(Saves.GameData.AlteredObjects.Find(x => x.Identifier.Equals(hitCollider.GetInstanceID())));
                    if (GameController.Instance.Player.Storage.StoredItems.Contains(ItemList.Seed)){
                        Tool collectedSeed = (Tool) GameController.Instance.Player.Storage.StoredItems.Find(x => x.Equals(ItemList.Seed));
                        collectedSeed.Durability += 1;
                    } else {
                        GameController.Instance.Player.Storage.StoredItems.Add(ItemList.Seed);
                    }
                } else if (UsedToolFunction.Equals(ToolFunctions.SHOVEL) && GameController.Instance.Player.Storage.MaxCapacity >= GameController.Instance.Player.Storage.StoredItems.Count + ItemList.Plant.Strength  && hitCollider.tag == "Crop"){
                    GameObject.Destroy(hitCollider);
                    Saves.GameData.AlteredObjects.Remove(Saves.GameData.AlteredObjects.Find(x => x.Identifier.Equals(hitCollider.GetInstanceID())));
                    for (int i = 0; i < ItemList.Plant.Strength; i++){
                        GameController.Instance.Player.Storage.StoredItems.Add(ItemList.Plant);
                    }
                } else if (hitCollider.tag == "Rock" && UsedToolFunction.Equals(ToolFunctions.PICKAXE) && Input.GetKey(Controls.UseItem)){
                    Saves.GameData.AlteredObjects.Add(new AlteredObject("Removal", hitCollider.gameObject.name, SceneManager.GetActiveScene(), hitCollider.gameObject.transform.position, hitCollider.gameObject.GetInstanceID()));
                    GameObject.Destroy(hitCollider);
                    World.RockList.Remove(hitCollider);
                    GameController.Instance.Player.Storage.StoredItems.Add(ItemList.Stone);
                } else if (hitCollider.tag == "Tree" && UsedToolFunction.Equals(ToolFunctions.AXE) && Input.GetKey(Controls.UseItem)){
                    Saves.GameData.AlteredObjects.Add(new AlteredObject("Removal", hitCollider.gameObject.name, SceneManager.GetActiveScene(), hitCollider.gameObject.transform.position, hitCollider.gameObject.GetInstanceID()));
                    GameObject.Destroy(hitCollider);
                    World.TreeList.Remove(hitCollider);
                    GameController.Instance.Player.Storage.StoredItems.Add(ItemList.Wood);
                }
            }
        }
    }

    public static void FollowCharacter(UnityCharacter leader, UnityCharacter follower){
        if ((leader.Rigidbody.position - follower.Rigidbody.position).magnitude >= 2 && (leader.Rigidbody.position - follower.Rigidbody.position).magnitude <= 20) {
            follower.Rigidbody.position = Vector3.MoveTowards(follower.Rigidbody.position, leader.Rigidbody.position, 1.0f * Time.fixedDeltaTime);
            float angle = Vector3.SignedAngle(leader.Rigidbody.position - follower.Rigidbody.position, follower.Rigidbody.position, Vector3.up);
            follower.Rigidbody.MoveRotation(Quaternion.Euler(0, angle, 0));
            if (follower.Object.tag == "Enemy" && leader.Object.tag == "Player"){
                if (follower.LastShot <= Saves.GameData.GameTime - 1){
                    follower.LastShot = Saves.GameData.GameTime;
                    ShootProjectile(follower, ItemList.Bow);
                }
            }
        }
    }

    public static void ShootProjectile(UnityCharacter shooter, Item usedItem){
        UnityProjectile projectile = new UnityProjectile(usedItem.Identifier);
        ShotProjectiles.Add(projectile);
        float offsetX, offsetZ;
        Vector3 direction;
        float speed = 10f;
        if (shooter.Rigidbody.rotation.eulerAngles.y < 90){
            offsetX = 0f;
            offsetZ = 1f;
            direction = Vector3.forward;
        } else if (shooter.Rigidbody.rotation.eulerAngles.y < 180){
            offsetX = 1f;
            offsetZ = 0f;
            direction = Vector3.right;
        } else if  (shooter.Rigidbody.rotation.eulerAngles.y < 270){
            offsetX = 0f;
            offsetZ = -1f;
            direction = Vector3.back;
        } else {
            offsetX = -1f;
            offsetZ = 0f;
            direction = Vector3.left;
        }
        projectile.Origin = new Vector3(shooter.Rigidbody.position.x + offsetX, shooter.Rigidbody.position.y, shooter.Rigidbody.position.z + offsetZ);
        projectile.Rigidbody.transform.position = projectile.Origin;
        projectile.Rigidbody.AddForce(direction * speed, ForceMode.Impulse);
    }
}
