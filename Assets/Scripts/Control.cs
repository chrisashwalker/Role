using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

public static class Control{
    public static KeyCode MapZoom{get;} = KeyCode.V;
    public static KeyCode Interact{get;} = KeyCode.E;
    public static KeyCode ScrollLeft{get;} = KeyCode.Comma;
    public static KeyCode ScrollRight{get;} = KeyCode.Period;
    public static KeyCode UseItem{get;} = KeyCode.Slash;
    public static KeyCode Buy{get;} = KeyCode.LeftBracket;
    public static KeyCode Sell{get;} = KeyCode.RightBracket;
    public static Vector3 targetPosition = Vector3.zero;

    public static void MoveCharacter(UnityCharacter character){
        float speed = 10.0f;
        Ray ray = CameraManager.MainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        LayerMask ground = LayerMask.GetMask("Ground"); 
        if (character.Grounded){
            Vector3 inputVector = new Vector3(Input.GetAxis("Horizontal"),0,Input.GetAxis("Vertical"));
            if (inputVector != Vector3.zero){
                targetPosition = character.Rigidbody.position + inputVector;
            } else if (Input.GetMouseButtonDown(0) && Physics.Raycast(ray.origin, ray.direction, out hit, Mathf.Infinity, ground)){
                targetPosition = hit.point;
            }
            if (targetPosition != Vector3.zero && (targetPosition - character.Rigidbody.position).magnitude > 0.1){
                character.Rigidbody.velocity = (targetPosition - character.Rigidbody.position).normalized * speed;
                Quaternion angle = Quaternion.LookRotation(character.Rigidbody.velocity);
                character.Rigidbody.MoveRotation(angle);
                GameController.Instance.anim.SetBool("Moving", true);
            } else {
                targetPosition = Vector3.zero;
                character.Rigidbody.velocity = Vector3.zero;
                if (GameController.Instance.anim.GetBool("Moving")){
                    GameController.Instance.anim.SetBool("Moving", false);
                }
            }
        }
    }

    public static void GetKeyPress()
    {
        if (Input.GetKeyDown(MapZoom)){
            UI.ToggleCamera();
        } else if (Input.GetKeyDown(KeyCode.Alpha1)){
            Map.FastTravel(1);
        } else if (Input.GetKeyDown(KeyCode.Alpha2)){
            Map.FastTravel(2);
        }
        Items.ItemUseCheck();
    }

    public static LayerMask InteractiveLayer{get;set;}
    public static float cellSize{get;} = 1.0f;

    public static void GetLayers()
    {
        InteractiveLayer = LayerMask.GetMask("Interactive");
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
            Quaternion angle = Quaternion.LookRotation(leader.Rigidbody.position - follower.Rigidbody.position);
            follower.Rigidbody.MoveRotation(angle);
            if (follower.Object.tag == "Enemy" && leader.Object.tag == "Player" && (leader.Rigidbody.position - follower.Rigidbody.position).magnitude >= 10){
                if (follower.LastShot <= Saves.GameData.GameTime - 1){
                    follower.LastShot = Saves.GameData.GameTime;
                    ShootProjectile(follower, ItemList.Bow);
                }
            }
        } else if ((leader.Rigidbody.position - follower.Rigidbody.position).magnitude < 2){
            if (follower.Object.tag == "Enemy" && leader.Object.tag == "Player" && follower.LastShot <= Saves.GameData.GameTime - 1){
                    follower.LastShot = Saves.GameData.GameTime;
                    leader.Health -= 1;
                    leader.Rigidbody.AddForce(new Vector3(0,10,0), ForceMode.Impulse);
            }
        }
    }

    public static void ShootProjectile(UnityCharacter shooter, Item usedItem){
        UnityProjectile projectile = new UnityProjectile(usedItem.Identifier);
        ShotProjectiles.Add(projectile);
        float speed = 10f;
        projectile.Origin = shooter.Rigidbody.position + shooter.Rigidbody.transform.forward;
        projectile.Rigidbody.transform.SetPositionAndRotation(projectile.Origin, shooter.Rigidbody.rotation);
        projectile.Rigidbody.AddForce(projectile.Rigidbody.transform.forward * speed, ForceMode.Impulse);
    }
}
