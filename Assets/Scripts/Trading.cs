using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public static class Trading
{
    public static bool InTrade {get;set;}
    public static Character Buyer {get;set;}
    public static Character Seller {get;set;}
    public static List<Item> SaleItems {get;set;}
    public static void FindSaleItems(Character buyer, Character seller)
    {
        Buyer = buyer;
        Seller = seller;
        SaleItems = new List<Item>();
        foreach (Item item in Seller.Storage.StoredItems)
        {
            SaleItems.Add(item);
        }
        InTrade = true;
        UI.LaunchTrading();
    }

    public static void TradeItem(GameObject clickedToggle)
    {
        Item tradedItem = null;
        foreach (Item item in SaleItems)
        {
            if (clickedToggle.GetComponentInChildren<Text>().text == item.Name + " : " + item.Value)
            {
                tradedItem = item;
            }
        }
        if (Buyer.Coins >= tradedItem.Value && Buyer.Storage.StoredItems.Count < Buyer.Storage.MaxCapacity)
        {
            Buyer.Coins -= tradedItem.Value;
            Seller.Coins += tradedItem.Value;
            Seller.Storage.StoredItems.Remove(tradedItem);
            Buyer.Storage.StoredItems.Add(tradedItem);
            InTrade = false;
            Items.UpdateToggles();
        } else {
            InTrade = true;
        }
    }
}
