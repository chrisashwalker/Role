using System.Collections.Generic;

[System.Serializable]
public class SaveData{
    public static bool Loaded{get;set;} = false;
    public int GameDay{get;set;}
    public int Progress{get;set;}
    public int CurrentLocation{get;set;}
    public float GameTime{get;set;}
    public string InventoryItems{get;set;}
    public List<AlteredObject> AlteredObjects{get;set;}

    public SaveData(int setDay = 1, float setTime = 0.0f, int setProgress = 1, int setLocation = 1, string setItems = "", List<AlteredObject> setAlteredObjects = null){
        GameDay = setDay;
        GameTime = setTime;
        Progress = setProgress;
        CurrentLocation = setLocation;
        InventoryItems = setItems;
        if (setAlteredObjects != null){
            AlteredObjects = setAlteredObjects;
        } else {
            AlteredObjects = new List<AlteredObject>();
        }
    }
}
