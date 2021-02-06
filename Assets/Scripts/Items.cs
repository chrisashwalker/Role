using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public enum ItemTypes
{
    ITEM,
    TOOL,
    PROJECTILE,
    FOOD
}

public enum ToolFunctions
{
    WEAPON,
    SHOVEL,
    AXE,
    PICKAXE,
    WATER,
    SEED
}

[System.Serializable]
public class Item
{
    public int Identifier {get; set;}
    public ItemTypes Type {get; set;}
    public string Name {get; set;}
    public int Value {get; set;}
    public int Strength {get; set;}

    public Item(int setIdentifier = 0, ItemTypes setType = ItemTypes.ITEM, string setName = "???", int setValue = 1, int setStrength = 1)
    {
        Identifier = setIdentifier;
        Type = setType;
        Name = setName;
        Value = setValue;
        Strength = setStrength;
    }
}

[System.Serializable]
public class Tool : Item
{
    public ToolFunctions Function {get; set;}
    public int Durability {get; set;}

    public Tool(int setIdentifier = 0, string setName = "???", int setValue = 1, int setStrength = 1, ToolFunctions setFunction = ToolFunctions.WEAPON, int setDurability = 0)
    {
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
public class Projectile : Item
{
    public float Distance {get; set;}
    public int Quantity {get; set;}

    public Projectile(int setIdentifier = 0, string setName = "???", int setValue = 1, int setStrength = 1, float setDistance = 24.0f, int setQuantity = 5)
    {
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
public class Food : Item
{
    public int Condition {get; set;}
    public int Quantity {get; set;}

    public Food(int setIdentifier = 0, string setName = "???", int setValue = 1, int setStrength = 1, int setCondition = 1, int setQuantity = 1)
    {
        Identifier = setIdentifier;
        Name = setName;
        Value = setValue;
        Strength = setStrength;
        Type = ItemTypes.FOOD;
        Condition = setCondition;
        Quantity = setQuantity;
    }
}

public sealed class ItemList
{
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

public class Items
{
    public int MaxCapacity {get; set;} = 10;
    public List<Item> StoredItems {get; set;}  = new List<Item>();
    public int EquippedItemIndex {get; set;} = 0;
    public static Dictionary<int, Item> GameItems  {get; set;}
    public static void GetItems()
    {
        GameItems = new Dictionary<int, Item>();
        GameItems.Add(ItemList.Sword.Identifier, ItemList.Sword);
        GameItems.Add(ItemList.Shovel.Identifier, ItemList.Shovel);
        GameItems.Add(ItemList.Pickaxe.Identifier, ItemList.Pickaxe);
        GameItems.Add(ItemList.Axe.Identifier, ItemList.Axe);
        GameItems.Add(ItemList.WateringCan.Identifier, ItemList.WateringCan);
        GameItems.Add(ItemList.Bow.Identifier, ItemList.Bow);
        GameItems.Add(ItemList.Seed.Identifier, ItemList.Seed);
        GameItems.Add(ItemList.Plant.Identifier, ItemList.Plant);
        GameItems.Add(ItemList.Wood.Identifier, ItemList.Wood);
        GameItems.Add(ItemList.Stone.Identifier, ItemList.Stone);
        if (Saves.GameState.InventoryItems.Count > 0)
        {
            Map.Player.Storage.StoredItems = Saves.GameState.InventoryItems;
        }
        else{
            Items.LoadStandardItems();
        }        
    }

    public static void LoadStandardItems()
    {
        Map.Player.Storage.StoredItems.Add(ItemList.Sword);
        Map.Player.Storage.StoredItems.Add(ItemList.Shovel);
        Map.Player.Storage.StoredItems.Add(ItemList.Pickaxe);
        Map.Player.Storage.StoredItems.Add(ItemList.Axe);
        Map.Player.Storage.StoredItems.Add(ItemList.WateringCan);
        Map.Player.Storage.StoredItems.Add(ItemList.Bow);
        Map.Player.Storage.StoredItems.Add(ItemList.Seed);
    }

    public static float toggleWidth = 100.0f;
    public static float toggleHeight = 25.0f;

    public static void UpdateToggles()
    {
        foreach (GameObject toggle in UI.AllShortcutToggles)
        {
            toggle.SetActive(false);
            GameObject.Destroy(toggle);
        }
        UI.AllShortcutToggles = new GameObject[0];
        foreach (Item item in Map.Player.Storage.StoredItems)
        {
            GameObject newToggleObject = GameObject.Instantiate(Resources.Load<GameObject>("ShortcutToggle"));
            newToggleObject.tag = Tags.ShortcutToggle;
            newToggleObject.transform.SetParent(UI.ShortcutCanvas.transform, false);
            string itemLabel;
            int itemDurability;
            if (item.Type == ItemTypes.TOOL)
            {
                Tool thisTool = (Tool) item;
                itemDurability = thisTool.Durability;
                if (itemDurability > 0)
                {
                    itemLabel = thisTool.Name + " (" + itemDurability + ")";
                } 
                else
                {
                    itemLabel = item.Name;
                }
            } 
            else 
            {
                itemLabel = item.Name;
            }
            newToggleObject.GetComponentInChildren<Text>().text = itemLabel;
        }
        UI.AllShortcutToggles = GameObject.FindGameObjectsWithTag(Tags.ShortcutToggle);
        int toggleCount = UI.AllShortcutToggles.Length;
        foreach (GameObject toggle in UI.AllShortcutToggles)
        {
            int toggleIndex = System.Array.IndexOf(UI.AllShortcutToggles, toggle);
            float positionFromCenter = toggleIndex - ((float) toggleCount / 2) + 0.5f;
            toggle.GetComponent<RectTransform>().anchoredPosition = new Vector2(positionFromCenter * toggleWidth,toggleHeight);
            if (toggleIndex == Map.Player.Storage.EquippedItemIndex)
            {
                toggle.GetComponentInChildren<Text>().text = toggle.GetComponentInChildren<Text>().text.ToUpper();
            }
        }
    }

    public static void Equip(Character character, GameObject clickedToggle)
    {
        foreach (Item item in character.Storage.StoredItems)
        {
            if (clickedToggle.GetComponentInChildren<Text>().text == item.Name)
            {
                character.Storage.EquippedItemIndex = character.Storage.StoredItems.IndexOf(item);
                break;
            }
        }
        UpdateToggles();
    }

    public static void ItemUseCheck()
    {
        int itemShift = 0;
        if (Control.PressedKey == Control.ScrollLeft)
        {
            itemShift = -1;
        } else if (Control.PressedKey == Control.ScrollRight)
        {
            itemShift = 1;
        }
        if (itemShift != 0)
        {
            if (Map.Player.Storage.EquippedItemIndex + itemShift < 0)
            {
                Map.Player.Storage.EquippedItemIndex = Map.Player.Storage.StoredItems.Count - 1;
            } else if (Map.Player.Storage.EquippedItemIndex + itemShift > Map.Player.Storage.StoredItems.Count - 1)
            {
                Map.Player.Storage.EquippedItemIndex = 0;
            } else {
                Map.Player.Storage.EquippedItemIndex += itemShift;
            }
            UpdateToggles();
        }
        if (Control.PressedKey == Control.UseItem)
        {
            if (Map.Player.Storage.StoredItems[Map.Player.Storage.EquippedItemIndex].Type.Equals(ItemTypes.PROJECTILE))
            {
                Control.ShootProjectile(Map.Player, (Projectile) Map.Player.Storage.StoredItems[Map.Player.Storage.EquippedItemIndex]);
            } else if (Map.Player.Storage.StoredItems[Map.Player.Storage.EquippedItemIndex].Type.Equals(ItemTypes.TOOL))
            {
                Control.UseTool(Collisions.CollidedObject);
            }
            UpdateToggles();     
        }
    }

}

public class UnityProjectile : Projectile
{
    public GameObject Object {get; set;}
    public Rigidbody Rigidbody {get; set;}
    public Collider Collider {get; set;}
    public int Instance {get; set;}
    public Vector3 Origin {get; set;}

    public UnityProjectile(int setIdentifier = 0)
    {
        Identifier = setIdentifier;
        Object = (GameObject) GameObject.Instantiate(Resources.Load("Projectile", typeof(GameObject)));
        Name = Object.name;
        Rigidbody = Object.GetComponent<Rigidbody>();
        Collider = Object.GetComponent<Collider>();
        Instance = Object.GetInstanceID();
    }
}
