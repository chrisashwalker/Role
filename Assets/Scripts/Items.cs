using System.Collections.Generic;
using UnityEngine;

public sealed class Inventory{

    public int MaxCapacity{
        get;set;
    }
    public List<Item> StoredItems{
        get;set;
    } 
    public int EquippedItemIndex{
        get;set;
    }
    
    private Inventory(){
        MaxCapacity = 10;
        StoredItems = new List<Item>();
        EquippedItemIndex = 0;
    }
    public static Inventory Backpack = new Inventory();
}

public enum ItemTypes{
    ITEM,
    TOOL,
    FOOD,
    PROJECTILE
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

    public GameObject Object{
        get;set;
    }

    public Collider Collider{
        get;set;
    }
        
    protected string name;
    protected ItemTypes type;
    protected int value, strength;
    public string Name{
        get{return name;}
    }
    public ItemTypes Type{
        get{return type;}
    }
    public int Value{
        get{return value;}
    }
    public int Strength{
        get{return strength;}
    }

    public Item(string setName = "Item", int setValue = 1, int setStrength = 1){
        name = setName;
        value = setValue;
        strength = setStrength;
        type = ItemTypes.ITEM;
    }

    public static Item Stone = new Item(setName:"Stone");
    public static Item Wood = new Item(setName:"Wood");
}

public class Tool : Item{
    public ToolFunctions Function{
        get;
    }
    public int Durability{
        get;set;
    }

    public Tool(string setName = "Item", int setValue = 1, int setStrength = 1, int setDurability = 0, ToolFunctions setFunction = ToolFunctions.WEAPON){
        name = setName;
        value = setValue;
        strength = setStrength;
        type = ItemTypes.TOOL;
        Durability = setDurability;
        Function = setFunction;
        
    }

    public static Tool Sword = new Tool(setName:"Sword", setStrength:2, setDurability:0, setFunction:ToolFunctions.WEAPON);
    public static Tool Shovel = new Tool(setName:"Shovel", setStrength:1, setDurability:0, setFunction:ToolFunctions.SHOVEL);
    public static Tool Axe = new Tool(setName:"Axe", setStrength:1, setDurability:0, setFunction:ToolFunctions.AXE);
    public static Tool Pickaxe = new Tool(setName:"Pickaxe", setStrength:1, setDurability:0, setFunction:ToolFunctions.PICKAXE);
    public static Tool WateringCan = new Tool(setName:"Watering can", setStrength:1, setDurability:0, setFunction:ToolFunctions.WATER);
    public static Tool Seed = new Tool(setName:"Seed", setStrength:5, setDurability: 3, setFunction:ToolFunctions.SEED);
}

public class Food : Item{
    public int Condition{
        get;set;
    }
    public int Quantity{
        get;set;
    }

    public Food(string setName = "Item", int setValue = 1, int setStrength = 1, int setCondition = 1, int setQuantity = 1){
        name = setName;
        value = setValue;
        strength = setStrength;
        Condition = setCondition;
        Quantity = setQuantity;
        type = ItemTypes.FOOD;
    }

    public static Food Plant = new Food(setName:"Plant", setCondition:2);
}

public class Projectile : Item{
    public float Distance{
        get;
    }
    public int Quantity{
        get;set;
    }

    public Projectile(string setName = "Item", int setValue = 1, int setStrength = 1, float setDistance = 3.0f, int setQuantity = 5){
        name = setName;
        value = setValue;
        strength = setStrength;
        Distance = setDistance;
        Quantity = setQuantity;
        type = ItemTypes.PROJECTILE;
    }

    public static Projectile Bow = new Projectile(setName:"Bow", setStrength:1, setDistance:3.0f, setQuantity:5);
}
