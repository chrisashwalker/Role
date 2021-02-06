using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

public static class Control
{
    public static KeyCode PressedKey {get; set;}
    public static KeyCode MapZoom {get;} = KeyCode.V;
    public static KeyCode Interact {get;} = KeyCode.E;
    public static KeyCode ScrollLeft {get;} = KeyCode.Comma;
    public static KeyCode ScrollRight {get;} = KeyCode.Period;
    public static KeyCode UseItem {get;} = KeyCode.Slash;
    public static KeyCode Buy {get;} = KeyCode.LeftBracket;
    public static KeyCode Sell {get;} = KeyCode.RightBracket;
    public static Vector3 TargetPosition {get; set;} = Vector3.zero;
    public static LayerMask InteractiveLayer {get; set;}
    public static float ProjectileSpeed {get; set;} = 10.0f;

    public static void MoveCharacter(UnityCharacter character)
    {
        Ray ray = UI.MainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        LayerMask ground = LayerMask.GetMask(Tags.Ground); 
        if (character.Grounded){
            Vector3 inputVector = new Vector3(Input.GetAxis("Horizontal"),0,Input.GetAxis("Vertical"));
            if (inputVector != Vector3.zero){
                TargetPosition = character.Rigidbody.position + inputVector;
            } 
            else if (Input.GetMouseButtonDown(0) && Physics.Raycast(ray.origin, ray.direction, out hit, Mathf.Infinity, ground))
            {
                TargetPosition = hit.point;
            }
            if (TargetPosition != Vector3.zero && (TargetPosition - character.Rigidbody.position).magnitude > 0.1)
            {
                character.Rigidbody.velocity = (TargetPosition - character.Rigidbody.position).normalized * character.Speed;
                Quaternion angle = Quaternion.LookRotation(character.Rigidbody.velocity);
                character.Rigidbody.MoveRotation(angle);
                character.Object.GetComponent<Animator>().SetBool(Tags.Moving, true);
            } 
            else 
            {
                TargetPosition = Vector3.zero;
                character.Rigidbody.velocity = Vector3.zero;
                if (character.Object.GetComponent<Animator>().GetBool(Tags.Moving))
                {
                    character.Object.GetComponent<Animator>().SetBool(Tags.Moving, false);
                }
            }
        }
    }

    public static void GetKeyPress()
    {
        List<KeyCode> keys = new List<KeyCode>();
        keys.Add(MapZoom);
        keys.Add(KeyCode.Alpha1);
        keys.Add(KeyCode.Alpha2);
        keys.Add(Interact);
        keys.Add(ScrollLeft);
        keys.Add(ScrollRight);
        keys.Add(UseItem);
        keys.Add(Buy);
        keys.Add(Sell);

        foreach (KeyCode key in keys)
        {
            if (Input.GetKeyDown(key))
            {
                PressedKey = key;
                break;
            }
            else
            {
                PressedKey = KeyCode.Clear;
            }
        }

        if (PressedKey == MapZoom)
        {
            UI.ToggleCamera();
        } else if (PressedKey == KeyCode.Alpha1)
        {
            Map.FastTravel(1);
        } else if (PressedKey == KeyCode.Alpha2)
        {
            Map.FastTravel(2);
        }
    }

    public static void GetLayers()
    {
        InteractiveLayer = LayerMask.GetMask(Tags.Interactable);
    }

    public static void UseTool(GameObject collidedObject)
    {
        Tool UsedTool = (Tool) Map.Player.Storage.StoredItems[Map.Player.Storage.EquippedItemIndex];
        ToolFunctions UsedToolFunction = UsedTool.Function;
        Collider[] hitColliders = Physics.OverlapBox(UI.Target.transform.position, new Vector3(0.1f,0.1f,0.1f), Quaternion.identity, InteractiveLayer);
        GameObject hitCollider = null;
        if (hitColliders.Length > 0)
        {
            hitCollider = hitColliders[0].gameObject;
        }
        if (hitCollider == null)
        {
            if (UsedToolFunction.Equals(ToolFunctions.WATER))
            {
                GameObject Soil = GameObject.Instantiate(Resources.Load<GameObject>(Tags.Soil));
                Soil.transform.position = UI.Target.transform.position;
                Saves.GameState.AlteredObjects.Add(new AlteredObject("Addition", Soil.name, SceneManager.GetActiveScene(), Soil.transform.position, Soil.GetInstanceID()));
            }
        } 
        else 
        {
            if (UsedToolFunction.Equals(ToolFunctions.SEED) && hitCollider.tag == Tags.Soil)
            {
                Saves.GameState.AlteredObjects.Remove(Saves.GameState.AlteredObjects.Find(x => x.Identifier.Equals(hitCollider.GetInstanceID())));
                GameObject.Destroy(hitCollider);
                GameObject Plant = GameObject.Instantiate(Resources.Load<GameObject>("CarrotStart"));
                Plant.transform.position = UI.Target.transform.position;
                Saves.GameState.AlteredObjects.Add(new AlteredObject("Addition", Plant.name, SceneManager.GetActiveScene(), Plant.transform.position, Plant.GetInstanceID()));
                if (UsedTool.Durability > 1)
                {
                    UsedTool.Durability -= 1;
                } 
                else 
                {
                    Map.Player.Storage.StoredItems.Remove(UsedTool);
                    Map.Player.Storage.EquippedItemIndex = 0;
                }
            } 
            else if (Map.Player.Storage.MaxCapacity > Map.Player.Storage.StoredItems.Count)
            {
                if (UsedToolFunction.Equals(ToolFunctions.SHOVEL) && hitCollider.tag == Tags.Plant)
                {
                    GameObject.Destroy(hitCollider);
                    Saves.GameState.AlteredObjects.Remove(Saves.GameState.AlteredObjects.Find(x => x.Identifier.Equals(hitCollider.GetInstanceID())));
                    if (Map.Player.Storage.StoredItems.Contains(ItemList.Seed))
                    {
                        Tool collectedSeed = (Tool) Map.Player.Storage.StoredItems.Find(x => x.Equals(ItemList.Seed));
                        collectedSeed.Durability += 1;
                    } 
                    else 
                    {
                        Map.Player.Storage.StoredItems.Add(ItemList.Seed);
                    }
                } 
                else if (UsedToolFunction.Equals(ToolFunctions.SHOVEL) && Map.Player.Storage.MaxCapacity >= Map.Player.Storage.StoredItems.Count + ItemList.Plant.Strength  && hitCollider.tag == "Crop")
                {
                    GameObject.Destroy(hitCollider);
                    Saves.GameState.AlteredObjects.Remove(Saves.GameState.AlteredObjects.Find(x => x.Identifier.Equals(hitCollider.GetInstanceID())));
                    for (int i = 0; i < ItemList.Plant.Strength; i++)
                    {
                        Map.Player.Storage.StoredItems.Add(ItemList.Plant);
                    }
                } 
                else if (hitCollider.tag == Tags.Rock && UsedToolFunction.Equals(ToolFunctions.PICKAXE) && Control.PressedKey == Control.UseItem)
                {
                    Saves.GameState.AlteredObjects.Add(new AlteredObject("Removal", hitCollider.gameObject.name, SceneManager.GetActiveScene(), hitCollider.gameObject.transform.position, hitCollider.gameObject.GetInstanceID()));
                    GameObject.Destroy(hitCollider);
                    Map.Rocks.Remove(hitCollider);
                    Map.Player.Storage.StoredItems.Add(ItemList.Stone);
                } 
                else if (hitCollider.tag == Tags.Tree && UsedToolFunction.Equals(ToolFunctions.AXE) && Control.PressedKey == Control.UseItem)
                {
                    Saves.GameState.AlteredObjects.Add(new AlteredObject("Removal", hitCollider.gameObject.name, SceneManager.GetActiveScene(), hitCollider.gameObject.transform.position, hitCollider.gameObject.GetInstanceID()));
                    GameObject.Destroy(hitCollider);
                    Map.Trees.Remove(hitCollider);
                    Map.Player.Storage.StoredItems.Add(ItemList.Wood);
                }
            }
        }
    }

    public static void FollowCharacter(UnityCharacter leader, UnityCharacter follower)
    {
        if ((leader.Rigidbody.position - follower.Rigidbody.position).magnitude >= 2 && (leader.Rigidbody.position - follower.Rigidbody.position).magnitude <= 20) 
        {
            follower.Rigidbody.position = Vector3.MoveTowards(follower.Rigidbody.position, leader.Rigidbody.position, 1.0f * Time.fixedDeltaTime);
            Quaternion angle = Quaternion.LookRotation(leader.Rigidbody.position - follower.Rigidbody.position);
            follower.Rigidbody.MoveRotation(angle);
            if (follower.Object.tag == Tags.Enemy && leader.Object.tag == Tags.Player && (leader.Rigidbody.position - follower.Rigidbody.position).magnitude >= 10)
            {
                if (follower.TimeOfLastAction <= Saves.GameState.GameTime - 1)
                {
                    Projectile usedWeapon = (Projectile) follower.Storage.StoredItems.Find(x => x.Type == ItemTypes.PROJECTILE);
                    if (usedWeapon != null && usedWeapon.Quantity > 0)
                    {
                        follower.TimeOfLastAction = Saves.GameState.GameTime;
                        ShootProjectile(follower, usedWeapon);
                    }
                }
            }
        } else if ((leader.Rigidbody.position - follower.Rigidbody.position).magnitude < 2)
        {
            if (follower.Object.tag == Tags.Enemy && leader.Object.tag == Tags.Player && follower.TimeOfLastAction <= Saves.GameState.GameTime - 1)
            {
                    follower.TimeOfLastAction = Saves.GameState.GameTime;
                    leader.Health -= 1;
                    leader.Rigidbody.AddForce(new Vector3(0,10,0), ForceMode.Impulse);
            }
        }
    }

    public static void ShootProjectile(UnityCharacter shooter, Projectile usedWeapon)
    {
        UnityProjectile projectile = new UnityProjectile(usedWeapon.Identifier);
        Map.ShotProjectiles.Add(projectile);
        projectile.Origin = shooter.Rigidbody.position + shooter.Rigidbody.transform.forward;
        projectile.Rigidbody.transform.SetPositionAndRotation(projectile.Origin, shooter.Rigidbody.rotation);
        projectile.Rigidbody.AddForce(projectile.Rigidbody.transform.forward * ProjectileSpeed, ForceMode.Impulse);
        usedWeapon.Quantity -= 1;
    }
}
