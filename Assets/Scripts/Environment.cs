using UnityEngine;
using UnityEngine.SceneManagement;

public class Barrier{
        public int Durability{ // TODO: Use this
            get;set;
        }

        public GameObject Object{
            get;set;
        }

        public Collider Collider{
            get;set;
        }

        public Barrier(int setDurability = 0){
            Durability = setDurability;
        }
    }

public class Gate : Barrier{
    public int Destination{
        get;set;
    }
    public bool Locked{ // TODO: Use this
        get;set;
    }
    
    public Gate(bool setLocked = true){
        Locked = setLocked;
    }
    
}

public class Plant : Barrier{
    public string Name{
        get;
    }
    public bool Growing{ // TODO: Use this
        get;set;
    }
    
    public Plant(int setDurability = 0, bool setGrowing = false, string setName = "Plant"){
        Durability = setDurability;
        Growing = setGrowing;
        Name = setName;
    }
}

[System.Serializable]
public class placedObject{
    public string changeType;
    public string scene;
    public int positionx;
    public int positiony;
    public int positionz;
    public int daysPlaced;

    public placedObject(string setChangeType, Scene setScene, Vector3Int setPosition, int setDaysPlaced){
        changeType = setChangeType;
        scene = setScene.name;
        positionx = setPosition.x;
        positiony = setPosition.y;
        positionz = setPosition.z;
        daysPlaced = setDaysPlaced;
    }
}