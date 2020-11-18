using System.Collections.Generic;
using UnityEngine;

public enum TagList{
    PLAYER,
    CHARACTER,
}

public class Character{

    public GameObject Object{
        get;set;
    }
    public Rigidbody Rigidbody{
        get;set;
    }
    public Collider Collider{
        get;set;
    }
    public List<TagList> Tags{
        get;set;
    }
    public string Name{
        get;set;
    }
    public float Speed{
        get;set;
    }
    public int MaxHealth{
        get;set;
    } 
    public int MaxStamina{
        get;set;
    } 
    public int Strength{
        get;set;
    } 
    public int Health{
        get;set;
    } 
    public int Stamina{
        get;set;
    }
    
    public Character(string setName = "", float setSpeed = 1.0f, int setMaxHealth = 1, int setMaxStamina = 1, int setStrength = 1){
        Name = setName;
        Speed = setSpeed;
        MaxHealth = setMaxHealth;
        MaxStamina = setMaxStamina;
        Strength = setStrength;
        Health = MaxHealth;
        Stamina = MaxStamina;
        Tags = new List<TagList>();
    }

    public string say(string phrase){ // TODO: Use this
        string speech = Name + ": " + phrase;
        return speech;
    }

    public string eat(Food food){ // TODO: Use this
        string message = Name + " ate " + food;
        int newHealth = Health + food.Strength * food.Condition;
        if (newHealth > MaxHealth){
            Health = MaxHealth;
        } else {
            Health = newHealth;
        }
        return message;
    }

    public void attack(Character target, Item weapon){ // TODO: Use this
        if (Stamina > 0){
            target.Health -= Strength + weapon.Strength;
        }
    }

}
