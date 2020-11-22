using System.Collections.Generic;

public sealed class Inventory{
    public int MaxCapacity{get;set;}
    public List<Item> StoredItems{get;set;} 
    public int EquippedItemIndex{get;set;}
    
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
