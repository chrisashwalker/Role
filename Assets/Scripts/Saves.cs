using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using UnityEngine;
using UnityEngine.SceneManagement;

public static class Saves
{
    public static Data GameState {get; set;}
    public static DateTime LastSave {get; set;}
    
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
            if (setInventoryItems != null)
            {
                InventoryItems = setInventoryItems;
            } 
            else 
            {
                InventoryItems = new List<Item>();
            }
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
    
    public static void LoadGame()
    {
        if (GameState != null)
        {
            string path = Application.persistentDataPath + "/save.bin";
            if (File.Exists(path))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(path, FileMode.Open);
                Data data = (Data) formatter.Deserialize(stream);
                stream.Close();
                GameState = data;
            }
        }
        else
        {
            GameState = new Data();
            GameState.GameDay = 1;
            GameState.GameTime = 300.0f;
            GameState.Progress = 0;
            GameState.CurrentLocation = 0;
        }
        SceneManager.LoadScene(Map.Scenes[GameState.CurrentLocation]);
    }

    public static void CheckStatus()
    {
        if (Map.Player.Health <= 0)
        {
            Map.DiscardAllObjects();
            LoadGame();
        }
    }
}
