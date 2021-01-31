using UnityEngine;
using UnityEngine.SceneManagement;

public static class Control
{
    public static KeyCode MapZoom {get;} = KeyCode.V;
    public static KeyCode Interact {get;} = KeyCode.E;
    public static KeyCode ScrollLeft {get;} = KeyCode.Comma;
    public static KeyCode ScrollRight {get;} = KeyCode.Period;
    public static KeyCode UseItem {get;} = KeyCode.Slash;
    public static KeyCode Buy {get;} = KeyCode.LeftBracket;
    public static KeyCode Sell {get;} = KeyCode.RightBracket;
    public static Vector3 TargetPosition {get; set;} = Vector3.zero;

    public static void MoveCharacter(UnityCharacter character)
    {
        float speed = 10.0f;
        Ray ray = UI.MainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        LayerMask ground = LayerMask.GetMask("Ground"); 
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
                character.Rigidbody.velocity = (TargetPosition - character.Rigidbody.position).normalized * speed;
                Quaternion angle = Quaternion.LookRotation(character.Rigidbody.velocity);
                character.Rigidbody.MoveRotation(angle);
                character.Object.GetComponent<Animator>().SetBool("Moving", true);
            } 
            else 
            {
                TargetPosition = Vector3.zero;
                character.Rigidbody.velocity = Vector3.zero;
                if (character.Object.GetComponent<Animator>().GetBool("Moving"))
                {
                    character.Object.GetComponent<Animator>().SetBool("Moving", false);
                }
            }
        }
    }

    public static void GetKeyPress()
    {
        if (Input.GetKeyDown(MapZoom))
        {
            UI.ToggleCamera();
        } else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Map.FastTravel(1);
        } else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Map.FastTravel(2);
        }
        Items.ItemUseCheck();
    }

    public static LayerMask InteractiveLayer {get; set;}
    public static float cellSize {get;} = 1.0f;

    public static void GetLayers()
    {
        InteractiveLayer = LayerMask.GetMask("Interactive");
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
                GameObject Soil = GameObject.Instantiate(Resources.Load<GameObject>("Soil"));
                Soil.transform.position = UI.Target.transform.position;
                Saves.GameState.AlteredObjects.Add(new AlteredObject("Addition", Soil.name, SceneManager.GetActiveScene(), Soil.transform.position, Soil.GetInstanceID()));
            }
        } 
        else 
        {
            if (UsedToolFunction.Equals(ToolFunctions.SEED) && hitCollider.tag == "Soil")
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
                if (UsedToolFunction.Equals(ToolFunctions.SHOVEL) && hitCollider.tag == "Plant")
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
                else if (hitCollider.tag == "Rock" && UsedToolFunction.Equals(ToolFunctions.PICKAXE) && Input.GetKey(Control.UseItem))
                {
                    Saves.GameState.AlteredObjects.Add(new AlteredObject("Removal", hitCollider.gameObject.name, SceneManager.GetActiveScene(), hitCollider.gameObject.transform.position, hitCollider.gameObject.GetInstanceID()));
                    GameObject.Destroy(hitCollider);
                    Map.Rocks.Remove(hitCollider);
                    Map.Player.Storage.StoredItems.Add(ItemList.Stone);
                } 
                else if (hitCollider.tag == "Tree" && UsedToolFunction.Equals(ToolFunctions.AXE) && Input.GetKey(Control.UseItem))
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
            if (follower.Object.tag == "Enemy" && leader.Object.tag == "Player" && (leader.Rigidbody.position - follower.Rigidbody.position).magnitude >= 10)
            {
                if (follower.LastShot <= Saves.GameState.GameTime - 1)
                {
                    follower.LastShot = Saves.GameState.GameTime;
                    ShootProjectile(follower, ItemList.Bow);
                }
            }
        } else if ((leader.Rigidbody.position - follower.Rigidbody.position).magnitude < 2)
        {
            if (follower.Object.tag == "Enemy" && leader.Object.tag == "Player" && follower.LastShot <= Saves.GameState.GameTime - 1)
            {
                    follower.LastShot = Saves.GameState.GameTime;
                    leader.Health -= 1;
                    leader.Rigidbody.AddForce(new Vector3(0,10,0), ForceMode.Impulse);
            }
        }
    }

    public static void ShootProjectile(UnityCharacter shooter, Item usedItem)
    {
        UnityProjectile projectile = new UnityProjectile(usedItem.Identifier);
        Map.ShotProjectiles.Add(projectile);
        float speed = 10f;
        projectile.Origin = shooter.Rigidbody.position + shooter.Rigidbody.transform.forward;
        projectile.Rigidbody.transform.SetPositionAndRotation(projectile.Origin, shooter.Rigidbody.rotation);
        projectile.Rigidbody.AddForce(projectile.Rigidbody.transform.forward * speed, ForceMode.Impulse);
    }
}
