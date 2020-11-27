using System.Collections.Generic;

public static class Trading{
    public static bool InTrade{get;set;} = false;
    public static Character Buyer{get;set;}
    public static Character Seller{get;set;}
    public static List<Item> SaleItems{get;set;}
    public static void FindSaleItems(Character buyer, Character seller){
        Buyer = buyer;
        Seller = seller;
        SaleItems = new List<Item>();
        foreach (Item item in Seller.Storage.StoredItems){
            SaleItems.Add(item);
        }
    }

    public static void TradeItem(Item item){
        if (Buyer.Coins >= item.Value && Buyer.Storage.StoredItems.Count < Buyer.Storage.MaxCapacity){
            Buyer.Coins -= item.Value;
            Seller.Coins += item.Value;
            Seller.Storage.StoredItems.Remove(item);
            Buyer.Storage.StoredItems.Add(item);
            InTrade = false;
        } else {
            InTrade = true;
        }
    }
}
