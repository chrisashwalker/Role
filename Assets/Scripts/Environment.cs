using UnityEngine;
using UnityEngine.SceneManagement;

public enum BarrierTypes{
    BARRIER,
    GATE
}

public class Barrier{
        public BarrierTypes Type{get;set;}
        public int Durability{get;set;}

        public Barrier(int setDurability = 0){
            Type = BarrierTypes.BARRIER;
            Durability = setDurability;
        }
    }

public class Gate : Barrier{
    public int Destination{get;set;}
    public bool Locked{get;set;}

    public Gate(bool setLocked = false){
        Type = BarrierTypes.GATE;
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
