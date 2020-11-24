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

public class Item{
    public ItemTypes Type{get;set;}
    public string Name{get;set;}
    public int Value{get;set;}
    public int Strength{get;set;}

    public Item(ItemTypes setType = ItemTypes.ITEM, string setName = "???", int setValue = 1, int setStrength = 1){
        Type = setType;
        Name = setName;
        Value = setValue;
        Strength = setStrength;
    }
}

public class Tool : Item{
    public ToolFunctions Function{get;set;}
    public int Durability{get;set;}

    public Tool(string setName = "???", int setValue = 1, int setStrength = 1, ToolFunctions setFunction = ToolFunctions.WEAPON, int setDurability = 0){
        Name = setName;
        Value = setValue;
        Strength = setStrength;
        Type = ItemTypes.TOOL;
        Function = setFunction;
        Durability = setDurability;
    }
}

public class Projectile : Item{
    public float Distance{get;set;}
    public int Quantity{get;set;}

    public Projectile(string setName = "???", int setValue = 1, int setStrength = 1, float setDistance = 24.0f, int setQuantity = 5){
        Name = setName;
        Value = setValue;
        Strength = setStrength;
        Type = ItemTypes.PROJECTILE;
        Distance = setDistance;
        Quantity = setQuantity;
    }
}

public class Food : Item{
    public int Condition{get;set;}
    public int Quantity{get;set;}

    public Food(string setName = "???", int setValue = 1, int setStrength = 1, int setCondition = 1, int setQuantity = 1){
        Name = setName;
        Value = setValue;
        Strength = setStrength;
        Type = ItemTypes.FOOD;
        Condition = setCondition;
        Quantity = setQuantity;
    }
}

public sealed class ItemList{
    public static Item Stone = new Item(setName:"Stone");
    public static Item Wood = new Item(setName:"Wood");
    public static Tool Sword = new Tool(setName:"Sword", setStrength:2, setDurability:0, setFunction:ToolFunctions.WEAPON);
    public static Tool Shovel = new Tool(setName:"Shovel", setStrength:1, setDurability:0, setFunction:ToolFunctions.SHOVEL);
    public static Tool Axe = new Tool(setName:"Axe", setStrength:1, setDurability:0, setFunction:ToolFunctions.AXE);
    public static Tool Pickaxe = new Tool(setName:"Pickaxe", setStrength:1, setDurability:0, setFunction:ToolFunctions.PICKAXE);
    public static Tool WateringCan = new Tool(setName:"Watering can", setStrength:1, setDurability:0, setFunction:ToolFunctions.WATER);
    public static Tool Seed = new Tool(setName:"Seed", setStrength:5, setDurability: 3, setFunction:ToolFunctions.SEED);
    public static Projectile Bow = new Projectile(setName:"Bow", setStrength:1, setDistance:24.0f, setQuantity:5);
    public static Food Plant = new Food(setName:"Plant", setCondition:2);
}

public static class Inventory{
    public static int MaxCapacity{get;set;} = 10;
    public static List<Item> StoredItems{get;set;}  = new List<Item>();
    public static int EquippedItemIndex{get;set;} = 0;
    public static List<Item> GameItemList{get;set;} = new List<Item>();

    public static void LoadGameItems(){
        GameItemList.Add(ItemList.Sword);
        GameItemList.Add(ItemList.Shovel);
        GameItemList.Add(ItemList.Pickaxe);
        GameItemList.Add(ItemList.Axe);
        GameItemList.Add(ItemList.WateringCan);
        GameItemList.Add(ItemList.Bow);
        GameItemList.Add(ItemList.Seed);
        GameItemList.Add(ItemList.Stone);
        GameItemList.Add(ItemList.Wood);
        GameItemList.Add(ItemList.Plant);
    }

    public static void LoadStandardItems(){
        StoredItems.Add(ItemList.Sword);
        StoredItems.Add(ItemList.Shovel);
        StoredItems.Add(ItemList.Pickaxe);
        StoredItems.Add(ItemList.Axe);
        StoredItems.Add(ItemList.WateringCan);
        StoredItems.Add(ItemList.Bow);
        StoredItems.Add(ItemList.Seed);
    }

    public static float toggleWidth = 100.0f;
    public static float toggleHeight = 25.0f;

    // TODO: Improve
    public static void UpdateToggles(){
        GameObject fullCanvas = GameObject.FindWithTag("FullCanvas");
        GameObject[] allItemToggles = GameObject.FindGameObjectsWithTag("ItemToggle");
        foreach (GameObject toggle in allItemToggles){
            toggle.SetActive(false);
            GameObject.Destroy(toggle);
        }
        foreach (Item item in StoredItems){
            GameObject newToggleObject = GameObject.Instantiate(Resources.Load<GameObject>("ItemToggle"));
            newToggleObject.tag = "ItemToggle";
            newToggleObject.transform.SetParent(fullCanvas.transform, false);
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
        allItemToggles = GameObject.FindGameObjectsWithTag("ItemToggle");
        int toggleCount = allItemToggles.Length;
        foreach (GameObject toggle in allItemToggles){
            int toggleIndex = System.Array.IndexOf(allItemToggles, toggle);
            float positionFromCenter = toggleIndex - ((float) toggleCount / 2) + 0.5f;
            toggle.GetComponent<RectTransform>().anchoredPosition = new Vector2(positionFromCenter * toggleWidth,toggleHeight);
            if (toggleIndex == EquippedItemIndex){
                toggle.GetComponentInChildren<Text>().text = toggle.GetComponentInChildren<Text>().text.ToUpper();
            }
        }
    }

    public static void Equip(GameObject clickedToggle){
        foreach (Item item in StoredItems){
            if (clickedToggle.GetComponentInChildren<Text>().text == item.Name){
                EquippedItemIndex = StoredItems.IndexOf(item);
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
            if (EquippedItemIndex + itemShift < 0){
                EquippedItemIndex = StoredItems.Count - 1;
            } else if (EquippedItemIndex + itemShift > StoredItems.Count - 1){
                EquippedItemIndex = 0;
            } else {
                EquippedItemIndex += itemShift;
            }
            UpdateToggles();
        }
        if (Input.GetKeyDown(Controls.UseItem)){
            if (StoredItems[EquippedItemIndex].Type.Equals(ItemTypes.PROJECTILE)){
                Actions.ShootProjectile(GameController.Instance.Player);
            } else if (StoredItems[EquippedItemIndex].Type.Equals(ItemTypes.TOOL)){
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
    public int Identifier{get;set;}
    public Vector3 Origin{get;set;}

    public UnityProjectile(string setName){
        Object = (GameObject) GameObject.Instantiate(Resources.Load(setName, typeof(GameObject)));
        Name = Object.name;
        Rigidbody = Object.GetComponent<Rigidbody>();
        Collider = Object.GetComponent<Collider>();
        Identifier = Object.GetInstanceID();
    }
}
