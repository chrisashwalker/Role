using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

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
        InTrade = true;
        GenerateUI();
    }

    public static void TradeItem(GameObject clickedToggle){
        Item tradedItem = null;
        foreach (Item item in SaleItems){
            if (clickedToggle.GetComponentInChildren<Text>().text == item.Name + " : " + item.Value){
                tradedItem = item;
            }
        }
        if (Buyer.Coins >= tradedItem.Value && Buyer.Storage.StoredItems.Count < Buyer.Storage.MaxCapacity){
            Buyer.Coins -= tradedItem.Value;
            Seller.Coins += tradedItem.Value;
            Seller.Storage.StoredItems.Remove(tradedItem);
            Buyer.Storage.StoredItems.Add(tradedItem);
            InTrade = false;
            Inventory.UpdateToggles();
        } else {
            InTrade = true;
        }
    }

     public static void GenerateUI(){
        foreach (GameObject toggle in GameController.Instance.AllShortcutToggles){
            toggle.SetActive(false);
            GameObject.Destroy(toggle);
        }
        foreach (Item item in SaleItems){
            GameObject newToggleObject = GameObject.Instantiate(Resources.Load<GameObject>("Control/ShortcutToggle"));
            newToggleObject.tag = "ShortcutToggle";
            newToggleObject.transform.SetParent(GameController.Instance.ShortcutCanvas.transform, false);
            string itemLabel;
            itemLabel = item.Name + " : " + item.Value;
            newToggleObject.GetComponentInChildren<Text>().text = itemLabel;
        }
        GameController.Instance.AllShortcutToggles = GameObject.FindGameObjectsWithTag("ShortcutToggle");
        int toggleCount = GameController.Instance.AllShortcutToggles.Length;
        foreach (GameObject toggle in GameController.Instance.AllShortcutToggles){
            int toggleIndex = System.Array.IndexOf(GameController.Instance.AllShortcutToggles, toggle);
            float positionFromCenter = toggleIndex - ((float) toggleCount / 2) + 0.5f;
            toggle.GetComponent<RectTransform>().anchoredPosition = new Vector2(positionFromCenter * Inventory.toggleWidth, Inventory.toggleHeight);
        }
    }
}
