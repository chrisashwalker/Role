using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

public static class World{
    public static Dictionary<int, string> SceneList{get;set;} = new Dictionary<int, string>();
    public static List<UnityGate> GateList{get;set;} = new List<UnityGate>();
    public static List<UnityMapItem> MapItemList{get;set;} = new List<UnityMapItem>();
    public static List<UnityCharacter> CharacterList{get;set;} = new List<UnityCharacter>();
    public static List<GameObject> TreeList{get;set;} = new List<GameObject>();
    public static List<GameObject> RockList{get;set;} = new List<GameObject>();

    public static void BuildScenes(){
        SceneList.Add(1, "1_home");
        SceneList.Add(2, "2_field");
    }

    public static void FindCharacters(){
        GameController.Instance.Player = new UnityCharacter(GameObject.FindGameObjectWithTag("Player"));
        foreach (GameObject character in GameObject.FindGameObjectsWithTag("Character")){
            UnityCharacter newCharacter = new UnityCharacter(character);
        }
    }

    public static void FindObjects(){
        foreach (GameObject rock in GameObject.FindGameObjectsWithTag("Rock")){
            if (RockList.Contains(rock) == false){
                RockList.Add(rock);
            }
        }
        foreach (GameObject tree in GameObject.FindGameObjectsWithTag("Tree")){
            if (TreeList.Contains(tree) == false){
                TreeList.Add(tree);
            }
        }
    }

    public static void FastTravel(int sceneNumber){
        //if (Saves.GameData.FarthestLocation >= sceneNumber && Saves.GameData.CurrentLocation != sceneNumber){ 
            Saves.GameData.CurrentLocation = sceneNumber;
            SceneManager.LoadScene(SceneList[sceneNumber], LoadSceneMode.Single);
            string savedItems = "";
            foreach (Item i in Inventory.StoredItems){
                savedItems += i.Identifier + ",";
            }
            Saves.GameData.InventoryItems = savedItems;
            Saves.SaveGame(Saves.GameData);
        //}
    }

    
  

    public static void FindGates(){
        foreach (GameObject gate in GameObject.FindGameObjectsWithTag("Gate")){
            Collider gateCollider = gate.GetComponent<Collider>();
            UnityGate newGate = new UnityGate("gatePrefab");
            newGate.Destination = int.Parse(gate.name.Substring(gate.name.Length - 1));
            newGate.Object = gate;
            newGate.Collider = gateCollider;
            GateList.Add(newGate);
        }
    }

    public static void FindMapItems(){
        foreach (GameObject mapItem in GameObject.FindGameObjectsWithTag("MapItem")){
            Collider mapItemCollider = mapItem.GetComponent<Collider>();
            int mapItemIdentifier = int.Parse(mapItem.name.Substring(mapItem.name.Length - 1));
            UnityMapItem foundItem = new UnityMapItem("mapItemPrefab", Inventory.GameItemList[mapItemIdentifier]);
            foundItem.Object = mapItem;
            foundItem.Collider = mapItemCollider;
            MapItemList.Add(foundItem);
        }
    }

}

public enum BarrierTypes{
    Barrier,
    Gate
}

public abstract class Barrier{
        public BarrierTypes Type{get;set;}
        public int Durability{get;set;}

        public Barrier(int setDurability = 0){
            Type = BarrierTypes.Barrier;
            Durability = setDurability;
        }
    }

public abstract class Gate : Barrier{
    public int Destination{get;set;}
    public bool Locked{get;set;}

    public Gate(bool setLocked = false){
        Type = BarrierTypes.Gate;
        Locked = setLocked;
    }
}

public class UnityBarrier : Barrier{
    public GameObject Object{get;set;}
    public Rigidbody Rigidbody{get;set;}
    public Collider Collider{get;set;}
    public int Identifier{get;set;}

    public UnityBarrier(string barrierPrefab){
        Object = (GameObject) GameObject.Instantiate(Resources.Load(barrierPrefab, typeof(GameObject)));
        Rigidbody = Object.GetComponent<Rigidbody>();
        Collider = Object.GetComponent<Collider>();
        Identifier = Object.GetInstanceID();
    }
}

public class UnityGate : Gate{
    public GameObject Object{get;set;}
    public Rigidbody Rigidbody{get;set;}
    public Collider Collider{get;set;}
    public int Identifier{get;set;}

    public UnityGate(string gatePrefab){
        Object = (GameObject) GameObject.Instantiate(Resources.Load(gatePrefab, typeof(GameObject)));
        Rigidbody = Object.GetComponent<Rigidbody>();
        Collider = Object.GetComponent<Collider>();
        Identifier = Object.GetInstanceID();
    }
}

public class UnityMapItem{
    public GameObject Object{get;set;}
    public Rigidbody Rigidbody{get;set;}
    public Collider Collider{get;set;}
    public int Identifier{get;set;}
    public Item linkedItem{get;set;}

    public UnityMapItem(string mapItemPrefab, Item item){
        Object = (GameObject) GameObject.Instantiate(Resources.Load(mapItemPrefab, typeof(GameObject)));
        Rigidbody = Object.GetComponent<Rigidbody>();
        Collider = Object.GetComponent<Collider>();
        Identifier = Object.GetInstanceID();
    }
}

[System.Serializable]
public class AlteredObject{
    public string Change{get;set;}
    public string Prefab{get;set;}
    public string Scene{get;set;}
    public float PositionX{get;set;}
    public float PositionY{get;set;}
    public float PositionZ{get;set;}
    public int DaysAltered{get;set;}
    public int Identifier{get;set;}

    public AlteredObject(string setChange, string setPrefab, Scene setScene, Vector3 setPosition, int setIdentifier, int setDaysAltered = 0){
        Change = setChange;
        Prefab = setPrefab.Replace("(Clone)","");
        Scene = setScene.name;
        PositionX = setPosition.x;
        PositionY = setPosition.y;
        PositionZ = setPosition.z;
        DaysAltered = setDaysAltered;
        Identifier = setIdentifier;
    }
}
