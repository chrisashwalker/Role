using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public enum ItemTypes{
    ITEM,
    TOOL,
    PROJECTILE,
    FOOD
}

public enum ToolFunctions{
    WEAPON,
    SHOVEL,
    AXE,
    PICKAXE,
    WATER,
    SEED
}

[System.Serializable]
public class Item{
    public int Identifier{get;set;}
    public ItemTypes Type{get;set;}
    public string Name{get;set;}
    public int Value{get;set;}
    public int Strength{get;set;}

    public Item(int setIdentifier = 0, ItemTypes setType = ItemTypes.ITEM, string setName = "???", int setValue = 1, int setStrength = 1){
        Identifier = setIdentifier;
        Type = setType;
        Name = setName;
        Value = setValue;
        Strength = setStrength;
    }
}

[System.Serializable]
public class Tool : Item{
    public ToolFunctions Function{get;set;}
    public int Durability{get;set;}

    public Tool(int setIdentifier = 0, string setName = "???", int setValue = 1, int setStrength = 1, ToolFunctions setFunction = ToolFunctions.WEAPON, int setDurability = 0){
        Identifier = setIdentifier;
        Name = setName;
        Value = setValue;
        Strength = setStrength;
        Type = ItemTypes.TOOL;
        Function = setFunction;
        Durability = setDurability;
    }
}

[System.Serializable]
public class Projectile : Item{
    public float Distance{get;set;}
    public int Quantity{get;set;}

    public Projectile(int setIdentifier = 0, string setName = "???", int setValue = 1, int setStrength = 1, float setDistance = 24.0f, int setQuantity = 5){
        Identifier = setIdentifier;
        Name = setName;
        Value = setValue;
        Strength = setStrength;
        Type = ItemTypes.PROJECTILE;
        Distance = setDistance;
        Quantity = setQuantity;
    }
}

[System.Serializable]
public class Food : Item{
    public int Condition{get;set;}
    public int Quantity{get;set;}

    public Food(int setIdentifier = 0, string setName = "???", int setValue = 1, int setStrength = 1, int setCondition = 1, int setQuantity = 1){
        Identifier = setIdentifier;
        Name = setName;
        Value = setValue;
        Strength = setStrength;
        Type = ItemTypes.FOOD;
        Condition = setCondition;
        Quantity = setQuantity;
    }
}

public sealed class ItemList{
    public static Tool Sword = new Tool(setIdentifier:0, setName:"Sword", setStrength:2, setDurability:0, setFunction:ToolFunctions.WEAPON);
    public static Tool Shovel = new Tool(setIdentifier:1, setName:"Shovel", setStrength:1, setDurability:0, setFunction:ToolFunctions.SHOVEL);
    public static Tool Axe = new Tool(setIdentifier:2, setName:"Axe", setStrength:1, setDurability:0, setFunction:ToolFunctions.AXE);
    public static Tool Pickaxe = new Tool(setIdentifier:3, setName:"Pickaxe", setStrength:1, setDurability:0, setFunction:ToolFunctions.PICKAXE);
    public static Tool WateringCan = new Tool(setIdentifier:4, setName:"Watering can", setStrength:1, setDurability:0, setFunction:ToolFunctions.WATER);
    public static Tool Seed = new Tool(setIdentifier:5, setName:"Seed", setStrength:5, setDurability: 3, setFunction:ToolFunctions.SEED);
    public static Projectile Bow = new Projectile(setIdentifier:6, setName:"Projectile", setStrength:1, setDistance:24.0f, setQuantity:5);
    public static Food Plant = new Food(setIdentifier:7, setName:"Plant", setCondition:2);
    public static Item Wood = new Item(setIdentifier:8, setName:"Wood");
    public static Item Stone = new Item(setIdentifier:9, setName:"Stone");
}

public class Inventory{
    public int MaxCapacity{get;set;} = 10;
    public List<Item> StoredItems{get;set;}  = new List<Item>();
    public int EquippedItemIndex{get;set;} = 0;
    public static Dictionary<int, Item> GameItemList{get;set;} = new Dictionary<int, Item>();
    public static void LoadGameItems(){
        GameItemList.Add(ItemList.Sword.Identifier, ItemList.Sword);
        GameItemList.Add(ItemList.Shovel.Identifier, ItemList.Shovel);
        GameItemList.Add(ItemList.Pickaxe.Identifier, ItemList.Pickaxe);
        GameItemList.Add(ItemList.Axe.Identifier, ItemList.Axe);
        GameItemList.Add(ItemList.WateringCan.Identifier, ItemList.WateringCan);
        GameItemList.Add(ItemList.Bow.Identifier, ItemList.Bow);
        GameItemList.Add(ItemList.Seed.Identifier, ItemList.Seed);
        GameItemList.Add(ItemList.Plant.Identifier, ItemList.Plant);
        GameItemList.Add(ItemList.Wood.Identifier, ItemList.Wood);
        GameItemList.Add(ItemList.Stone.Identifier, ItemList.Stone);
    }

    public static void LoadStandardItems(){
        GameController.Instance.Player.Storage.StoredItems.Add(ItemList.Sword);
        GameController.Instance.Player.Storage.StoredItems.Add(ItemList.Shovel);
        GameController.Instance.Player.Storage.StoredItems.Add(ItemList.Pickaxe);
        GameController.Instance.Player.Storage.StoredItems.Add(ItemList.Axe);
        GameController.Instance.Player.Storage.StoredItems.Add(ItemList.WateringCan);
        GameController.Instance.Player.Storage.StoredItems.Add(ItemList.Bow);
        GameController.Instance.Player.Storage.StoredItems.Add(ItemList.Seed);
    }

    public static float toggleWidth = 100.0f;
    public static float toggleHeight = 25.0f;

    public static void UpdateToggles(){
        foreach (GameObject toggle in GameController.Instance.AllShortcutToggles){
            toggle.SetActive(false);
            GameObject.Destroy(toggle);
        }
        GameController.Instance.AllShortcutToggles = new GameObject[0];
        foreach (Item item in GameController.Instance.Player.Storage.StoredItems){
            GameObject newToggleObject = GameObject.Instantiate(Resources.Load<GameObject>("ShortcutToggle"));
            newToggleObject.tag = "ShortcutToggle";
            newToggleObject.transform.SetParent(GameController.Instance.ShortcutCanvas.transform, false);
            string itemLabel;
            int itemDurability;
            if (item.Type == ItemTypes.TOOL){
                Tool thisTool = (Tool) item;
                itemDurability = thisTool.Durability;
                if (itemDurability > 0){
                    itemLabel = thisTool.Name + " (" + itemDurability + ")";
                } else{
                    itemLabel = item.Name;
                }
            } else {
                itemLabel = item.Name;
            }
            newToggleObject.GetComponentInChildren<Text>().text = itemLabel;
        }
        GameController.Instance.AllShortcutToggles = GameObject.FindGameObjectsWithTag("ShortcutToggle");
        int toggleCount = GameController.Instance.AllShortcutToggles.Length;
        foreach (GameObject toggle in GameController.Instance.AllShortcutToggles){
            int toggleIndex = System.Array.IndexOf(GameController.Instance.AllShortcutToggles, toggle);
            float positionFromCenter = toggleIndex - ((float) toggleCount / 2) + 0.5f;
            toggle.GetComponent<RectTransform>().anchoredPosition = new Vector2(positionFromCenter * toggleWidth,toggleHeight);
            if (toggleIndex == GameController.Instance.Player.Storage.EquippedItemIndex){
                toggle.GetComponentInChildren<Text>().text = toggle.GetComponentInChildren<Text>().text.ToUpper();
            }
        }
    }

    public static void Equip(Character character, GameObject clickedToggle){
        foreach (Item item in character.Storage.StoredItems){
            if (clickedToggle.GetComponentInChildren<Text>().text == item.Name){
                GameController.Instance.Player.Storage.EquippedItemIndex = character.Storage.StoredItems.IndexOf(item);
                break;
            }
        }
        UpdateToggles();
    }

    public static void ItemUseCheck(){
        int itemShift = 0;
        if (Input.GetKeyDown(Controls.ScrollLeft)){
            itemShift = -1;
        } else if (Input.GetKeyDown(Controls.ScrollRight)){
            itemShift = 1;
        }
        if (itemShift != 0){
            if (GameController.Instance.Player.Storage.EquippedItemIndex + itemShift < 0){
                GameController.Instance.Player.Storage.EquippedItemIndex = GameController.Instance.Player.Storage.StoredItems.Count - 1;
            } else if (GameController.Instance.Player.Storage.EquippedItemIndex + itemShift > GameController.Instance.Player.Storage.StoredItems.Count - 1){
                GameController.Instance.Player.Storage.EquippedItemIndex = 0;
            } else {
                GameController.Instance.Player.Storage.EquippedItemIndex += itemShift;
            }
            UpdateToggles();
        }
        if (Input.GetKeyDown(Controls.UseItem)){
            if (GameController.Instance.Player.Storage.StoredItems[GameController.Instance.Player.Storage.EquippedItemIndex].Type.Equals(ItemTypes.PROJECTILE)){
                Actions.ShootProjectile(GameController.Instance.Player, GameController.Instance.Player.Storage.StoredItems[GameController.Instance.Player.Storage.EquippedItemIndex]);
            } else if (GameController.Instance.Player.Storage.StoredItems[GameController.Instance.Player.Storage.EquippedItemIndex].Type.Equals(ItemTypes.TOOL)){
                Actions.UseTool(CollisionManager.CollidedObject);
            }
            UpdateToggles();     
        }
    }

}

public class UnityProjectile : Projectile{
    public GameObject Object{get;set;}
    public Rigidbody Rigidbody{get;set;}
    public Collider Collider{get;set;}
    public int Instance{get;set;}
    public Vector3 Origin{get;set;}

    public UnityProjectile(int setIdentifier = 0){
        Identifier = setIdentifier;
        Object = (GameObject) GameObject.Instantiate(Resources.Load("Projectile1", typeof(GameObject)));
        Name = Object.name;
        Rigidbody = Object.GetComponent<Rigidbody>();
        Collider = Object.GetComponent<Collider>();
        Instance = Object.GetInstanceID();
    }
}
