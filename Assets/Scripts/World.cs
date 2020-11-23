using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

public static class World{
    public static Dictionary<int, string> SceneList;
    public static List<UnityGate> GateList;
    public static List<UnityMapItem> MapItemList;
    public static List<UnityCharacter> CharacterList;

    public static void BuildScenes(){
        SceneList.Add(1, "1_home");
        SceneList.Add(2, "2_field");
    }

    public static void FindCharacters(){
        Player = new Character();
        Player.Object = GameObject.FindWithTag("Player");
        Player.Rigidbody = Player.Object.GetComponent<Rigidbody>();
        Player.Collider = Player.Object.GetComponent<Collider>();
        Player.Type.Add(CharacterTypes.Player);

        if (GameObject.FindGameObjectsWithTag("Character").Length > 0){
            CharacterArray = GameObject.FindGameObjectsWithTag("Character");
            foreach (GameObject charObject in CharacterArray){
                Character newChar = new Character();
                newChar.Object = charObject;
                newChar.Rigidbody = newChar.Object.GetComponent<Rigidbody>();
                newChar.Collider = newChar.Object.GetComponent<Collider>();
                newChar.Type.Add(CharacterTypes.CHARACTER);
            }
        }
    }

    public static void FindObjects(){
        Rocks = new List<GameObject>();
        Trees = new List<GameObject>();

        if (GameObject.FindGameObjectsWithTag("Rock").Length > 0){
            GameObject[] rockArray = GameObject.FindGameObjectsWithTag("Rock");
            foreach (GameObject rock in rockArray){
                Rocks.Add(rock);
            }
        }

        if (GameObject.FindGameObjectsWithTag("Tree").Length > 0){
            GameObject[] treeArray = GameObject.FindGameObjectsWithTag("Tree");
            foreach (GameObject tree in treeArray){
                Trees.Add(tree);
            }
        }
    }

    public static void FastTravel(int sceneNumber){
        //if (farthestScene >= sceneNumber && currentScene != sceneNumber){ TODO: Reactivate after testing
            currentScene = sceneNumber;
            SceneManager.LoadScene(SceneList[sceneNumber], LoadSceneMode.Single);
            string savedItems = "";
            foreach (Item i in Backpack.StoredItems){
                savedItems += i.Name + ";";
            }
            gameData = new Saves.SaveData(gameDayNumber, gameTime, farthestScene, currentScene, savedItems, placedObjects);
            Saves.SaveGame(gameData);
        //}
    }

    
  

    public static void FindGates(){
        GateList = new List<Gate>();
        foreach (GameObject gate in GameObject.FindGameObjectsWithTag("Gate")){
            Collider gateCollider = gate.GetComponent<Collider>();
            Gate newGate = new Gate();
            newGate.Destination = int.Parse(gate.name.Substring(gate.name.Length - 1));
            newGate.Object = gate;
            newGate.Collider = gateCollider;
            GateList.Add(newGate);
        }
    }

    public static void FindMapItems(){
        foreach (GameObject mapItem in GameObject.FindGameObjectsWithTag("MapItem")){
            Collider mapItemCollider = mapItem.GetComponent<Collider>();
            UnityMapItem foundItem = new UnityMapItem("mapItemPrefab", Inventory.GameItemList.Find(x => x.Name == mapItem.name));
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
    public string Prefab{get;set;}
    public string Scene{get;set;}
    public float PositionX{get;set;}
    public float PositionY{get;set;}
    public float PositionZ{get;set;}
    public int DaysAltered{get;set;}
    public int Identifier{get;set;}

    public AlteredObject(string setPrefab, Scene setScene, Vector3 setPosition, int setIdentifier, int setDaysAltered = 0){
        Prefab = setPrefab;
        Scene = setScene.name;
        PositionX = setPosition.x;
        PositionY = setPosition.y;
        PositionZ = setPosition.z;
        DaysAltered = setDaysAltered;
        Identifier = setIdentifier;
    }
}
