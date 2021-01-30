using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using UnityEngine;

public static class Saves
{
    public static bool Loaded {get;set;}
    public static DateTime LastSave {get;set;}
    
    [System.Serializable]
    public class Data
    {
        public int GameDay {get;set;}
        public float GameTime {get;set;}
        public int Progress {get;set;}
        public int CurrentLocation {get;set;}
        public int Funds {get;set;}
        public List<Item> InventoryItems {get;set;}
        public List<AlteredObject> AlteredObjects {get;set;}

        public Data(int setDay = 1, float setTime = 0.0f, int setProgress = 1, int setLocation = 1, List<Item> setInventoryItems = null, List<AlteredObject> setAlteredObjects = null, int setFunds = 10)
        {
            GameDay = setDay;
            GameTime = setTime;
            Progress = setProgress;
            CurrentLocation = setLocation;
            Funds = setFunds;
            InventoryItems = setInventoryItems;
            if (setAlteredObjects != null)
            {
                AlteredObjects = setAlteredObjects;
            } 
            else 
            {
                AlteredObjects = new List<AlteredObject>();
            }
        }
    }

    public static void SaveGame(Data data)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/save.bin";
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, data);
        stream.Close();
    }
    
    public static Data LoadGame()
    {
        if (!Loaded){
            string path = Application.persistentDataPath + "/save.bin";
            if (File.Exists(path)){
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(path, FileMode.Open);
                Data data = (Data) formatter.Deserialize(stream);
                stream.Close();
                Loaded = true;
                return data;
            } else {
                return null;
            }
        }
    }

    public static void CheckStatus()
    {
        if (Map.Player.Health <= 0)
        {
            Map.DiscardAllObjects();
            Loaded = false;
            SceneManager.LoadScene(World.SceneList[Saves.GameData.CurrentLocation]);
        }
    }
}
