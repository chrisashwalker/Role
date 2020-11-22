using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using UnityEngine;

public abstract class Saves{
    [System.Serializable]
    public class SaveData{
        public static bool Loaded{get;set;} = false;
        public int GameDay{get;set;}
        public int Progress{get;set;}
        public int CurrentLocation{get;set;}
        public int FarthestLocation{get;set;}
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

    public SaveData GameData{get;set;} = new SaveData();

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
