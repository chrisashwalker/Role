using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class Saves {

    public static bool loaded = false;

    [System.Serializable]
    public class Data {
        
        public int day, progress, location;
        public float time;
        public string items; 
        public List<placedObject> placedObjects;

        public Data(int setDay = 1, float setTime = 0.0f, int setProgress = 1, int setLocation = 1, string setItems = "", List<placedObject> setPlacedObjects = null){
            day = setDay;
            time = setTime;
            progress = setProgress;
            location = setLocation;
            items = setItems;
            placedObjects = setPlacedObjects;
        }

    }

    public static void SaveGame(Data data){
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/save.bin";
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, data);
        stream.Close();
    }
    
    public static Saves.Data LoadGame(){
        string path = Application.persistentDataPath + "/save.bin";
        if (File.Exists(path)){
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            Data data = (Data) formatter.Deserialize(stream);
            stream.Close();
            return data;
        } else {
            return null;
        }
    }
}