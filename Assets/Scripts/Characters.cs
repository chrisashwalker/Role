using UnityEngine;

public enum CharacterTypes
{
    Player,
    NPC,
    Enemy
}

public abstract class Character
{
    public CharacterTypes Type {get; set;}
    public string Name {get; set;}
    public int MaxHealth {get; set;} 
    public int MaxStamina {get; set;} 
    public float Speed {get; set;}
    public int Strength {get; set;} 
    public int Health {get; set;} 
    public int Stamina {get; set;}
    public int Coins {get; set;}
    public Items Storage {get; set;}
    
    public Character(CharacterTypes setType = CharacterTypes.NPC, string setName = "???", float setSpeed = 1.0f, int setMaxHealth = 10, int setMaxStamina = 1, int setStrength = 1, int setCoins = 10)
    {
        Type = setType;
        Name = setName;
        Speed = setSpeed;
        MaxHealth = setMaxHealth;
        MaxStamina = setMaxStamina;
        Strength = setStrength;
        Health = MaxHealth;
        Stamina = MaxStamina;
        Coins = setCoins;
        Storage = new Items();
    }
}

public class UnityCharacter : Character
{
    public GameObject Object {get; set;}
    public Rigidbody Rigidbody {get; set;}
    public Collider Collider {get; set;}
    public int Identifier {get; set;}
    public bool Grounded {get; set;}
    public float TimeOfLastAction {get; set;}

    public UnityCharacter(GameObject go)
    {
        Object = go;
        Name = Object.name;
        Rigidbody = Object.GetComponent<Rigidbody>();
        Collider = Object.GetComponent<Collider>();
        Identifier = Object.GetInstanceID();
        if (Object.tag == Tags.Player)
        {
            Type = CharacterTypes.Player;
        } else if (Object.tag == Tags.Enemy)
        {
            Type = CharacterTypes.Enemy;
        }
        Grounded = false;
        TimeOfLastAction = 0.0f;
    }
}
