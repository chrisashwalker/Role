using UnityEngine;
using UnityEngine.SceneManagement;

public class UnityBarrier{
    public GameObject Object{get;set;}
    public Rigidbody Rigidbody{get;set;}
    public Collider Collider{get;set;}
    public int Identifier{get;set;}
}

public class Barrier : UnityBarrier{
        public int Durability{get;set;} // TODO: Use this

        public Barrier(int setDurability = 0){
            Durability = setDurability;
        }
    }

public class Gate : Barrier{
    public int Destination{get;set;}
    public bool Locked{get;set;} // TODO: Use this

    public Gate(bool setLocked = false){
        Locked = setLocked;
    }
}

public class Plant : Barrier{
    public string Name{get;set;}
    public bool Growing{get;set;} // TODO: Use this

    public Plant(int setDurability = 0, bool setGrowing = false, string setName = "Plant"){
        Durability = setDurability;
        Growing = setGrowing;
        Name = setName;
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
    public int Identifier{get;set;} // TODO: not necessarily unique

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
