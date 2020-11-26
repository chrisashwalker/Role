using UnityEngine;

public static class Trading{
    public static List<Item> FindSaleItems(Character character){
        List<Item> saleItems = new List<Item>();
        foreach (Item item in character.Inventory.StoredItems){
            saleItems.Add(item);
        }
        return saleItems;
    }

    public static void SellItem(Character sellingCharacter, Character buyingCharacter, Item item){
        sellingCharacter.Inventory.StoredItems.Remove(item);
        sellingCharacter.Coins += item.Value;
        buyingCharacter.Inventory.StoredItems.Add(item);
        buyingCharacter.Coins += item.Value;
    }
}
