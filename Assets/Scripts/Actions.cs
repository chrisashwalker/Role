using UnityEngine;

public sealed class Actions{
    public LayerMask interactiveLayer;
    public GameObject targetPrefab;
    public GameObject target;
    public float cellSize = 1.0f;
    private Inventory Backpack = Inventory.Backpack;
    private Character Player = GameController.Instance.Player;


    void ShootProjectile(Character shooter){
        ShotProjectile = (Projectile) Backpack.StoredItems[Backpack.EquippedItemIndex];
        ShotProjectileObject = new GameObject("ShotProjectile");
        SpriteRenderer ShotProjectileSpriteRenderer = ShotProjectileObject.AddComponent<SpriteRenderer>();
        Rigidbody ShotProjectileRigidbody = ShotProjectileObject.AddComponent<Rigidbody>();
        ShotProjectileRigidbody.useGravity = false;
        Sprite ShotProjectileSprite = Sprite.Create(ProjectileTexture, new Rect(0, 0, 128.0f, 256.0f), new Vector2(0.5f, 0.5f), 256.0f);
        ShotProjectileSpriteRenderer.sprite = ShotProjectileSprite;
        ShotProjectileSpriteRenderer.sortingLayerName = "Player";
        int projectileShiftX = 0, projectileShiftY = 0, projectileShiftZ = 0;
        float projectileVelocityX = 0.0f, projectileVelocityY = 0.0f, projectileVelocityZ = 0.0f;
        if (Player.Direction == 'D'){
            projectileVelocityZ = -24.0f;
        } else if (Player.Direction == 'U'){
            projectileVelocityZ = 24.0f;
        } else if (Player.Direction == 'L'){
            projectileVelocityX = -24.0f;
        } else if (Player.Direction == 'R'){
            projectileVelocityX = 24.0f;
        }
        ShotProjectileObject.transform.position = new Vector3(shooter.Rigidbody.position.x + projectileShiftX, shooter.Rigidbody.position.y + projectileShiftY, shooter.Rigidbody.position.z + projectileShiftZ);
        ShotProjectileRigidbody.velocity = new Vector3(projectileVelocityX, projectileVelocityY, projectileVelocityZ);
        ShotProjectileStart = ShotProjectileObject.transform.position;            
    }

    void findTarget(){
        if (target == null){
            target = Instantiate(targetPrefab);
        }
        float targetX, targetY, targetZ;
        targetY = 0.04f;
        if (Player.Direction == 'D'){
            targetX = (float) System.Math.Floor(Player.Rigidbody.position.x / cellSize) * cellSize + cellSize;
            targetZ = (float) System.Math.Floor(Player.Rigidbody.position.z / cellSize) * cellSize - cellSize;
        } else if (Player.Direction == 'U'){
            targetX = (float) System.Math.Floor(Player.Rigidbody.position.x / cellSize) * cellSize + cellSize;
            targetZ = (float) System.Math.Ceiling(Player.Rigidbody.position.z / cellSize) * cellSize + cellSize;
        } else if (Player.Direction == 'L'){
            targetX = (float) System.Math.Floor(Player.Rigidbody.position.x / cellSize) * cellSize - cellSize;
            targetZ = (float) System.Math.Floor(Player.Rigidbody.position.z / cellSize) * cellSize + cellSize;
        } else {
            targetX = (float) System.Math.Ceiling(Player.Rigidbody.position.x / cellSize) * cellSize + cellSize;
            targetZ = (float) System.Math.Floor(Player.Rigidbody.position.z / cellSize) * cellSize + cellSize;
        }
        target.transform.position = new Vector3(targetX,targetY,targetZ);
    }

    void UseTool(Character worker){
        Tool UsedTool = (Tool) Backpack.StoredItems[Backpack.EquippedItemIndex];
        ToolFunctions UsedToolFunction = UsedTool.Function;
        Collider[] hitColliders = Physics.OverlapBox(target.transform.position, new Vector3(0.1f,0.1f,0.1f), Quaternion.identity, interactiveLayer);
        GameObject hitCollider = null;
        if (hitColliders.Length > 0){
            hitCollider = hitColliders[0].gameObject;
        }
        if (hitCollider == null){
            if (UsedToolFunction.Equals(ToolFunctions.WATER)){
                GameObject prepared = Instantiate(Prepared);
                prepared.transform.position = target.transform.position;
                placedObjects.Add(new AlteredObject(Prepared.name, SceneManager.GetActiveScene(), prepared.transform.position, prepared.GetHashCode()));
            }
        } else {
            if (UsedToolFunction.Equals(ToolFunctions.SEED) && hitCollider.tag == "Prepared"){
                GameObject.Destroy(hitCollider);
                GameObject placed = Instantiate(Placed);
                placed.transform.position = target.transform.position;
                placedObjects.Add(new AlteredObject(Placed.name, SceneManager.GetActiveScene(), placed.transform.position, placed.GetHashCode()));
                if (UsedTool.Durability > 1){
                    UsedTool.Durability -= 1;
                } else {
                    Backpack.StoredItems.Remove(UsedTool);
                    Backpack.EquippedItemIndex = 0;
                }
            } else if (Backpack.MaxCapacity > Backpack.StoredItems.Count){
                if (UsedToolFunction.Equals(ToolFunctions.SHOVEL) && hitCollider.tag == "Placed"){
                    GameObject.Destroy(hitCollider);
                    placedObjects.Remove(placedObjects.Find(x => x.Identifier.Equals(hitCollider.GetHashCode())));
                    if (Backpack.StoredItems.Contains(Tool.Seed)){
                        Tool collectedSeed = (Tool) Backpack.StoredItems.Find(x => x.Equals(Tool.Seed));
                        collectedSeed.Durability += 1;
                    } else {
                        Backpack.StoredItems.Add(Tool.Seed);
                    }
                } else if (UsedToolFunction.Equals(ToolFunctions.SHOVEL) && Backpack.MaxCapacity >= Backpack.StoredItems.Count + Food.Plant.Strength  && hitCollider.tag == "Ready"){
                    GameObject.Destroy(hitCollider);
                    placedObjects.Remove(placedObjects.Find(x => x.Identifier.Equals(hitCollider.GetHashCode())));
                    for (int i = 0; i < Food.Plant.Strength; i++){
                        Backpack.StoredItems.Add(Food.Plant);
                    }
                } else if (hitCollider.tag == "Rock" && UsedToolFunction.Equals(ToolFunctions.PICKAXE)){
                    GameObject.Destroy(hitCollider);
                    Backpack.StoredItems.Add(Item.Stone);
                } else if (hitCollider.tag == "Tree" && UsedToolFunction.Equals(ToolFunctions.AXE)){
                    GameObject.Destroy(hitCollider);
                    Backpack.StoredItems.Add(Item.Wood);
                }
            }
        }
    }
}