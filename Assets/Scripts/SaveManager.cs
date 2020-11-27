using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using UnityEngine;

public static class Saves{
    public static bool Loaded{get;set;} = false;
    
    [System.Serializable]
    public class SaveData{
        public int GameDay{get;set;}
        public int Progress{get;set;}
        public int CurrentLocation{get;set;}
        public int FarthestLocation{get;set;}
        public float GameTime{get;set;}
        public List<Item> InventoryItems{get;set;}
        public List<AlteredObject> AlteredObjects{get;set;}
        public int Funds{get;set;}

        public SaveData(int setDay = 1, float setTime = 0.0f, int setProgress = 1, int setLocation = 1, List<Item> setInventoryItems = null, List<AlteredObject> setAlteredObjects = null, int setFunds = 10){
            GameDay = setDay;
            GameTime = setTime;
            Progress = setProgress;
            CurrentLocation = setLocation;
            InventoryItems = setInventoryItems;
            if (setAlteredObjects != null){
                AlteredObjects = setAlteredObjects;
            } else {
                AlteredObjects = new List<AlteredObject>();
            }
            Funds = setFunds;
        }
    }

    public static SaveData GameData{get;set;}

    public static void SaveGame(SaveData data){
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/save.bin";
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, data);
        stream.Close();
    }
    
    public static SaveData LoadGame(){
        string path = Application.persistentDataPath + "/save.bin";
        if (File.Exists(path)){
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            SaveData data = (SaveData) formatter.Deserialize(stream);
            stream.Close();
            return data;
        } else {
            return null;
        }
    }
}
