using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Map
{    
    public static Dictionary<int, string> Scenes {get; set;}
    public static UnityCharacter Player {get; set;}
    public static List<UnityCharacter> Characters {get; set;}
    public static List<UnityCharacter> Enemies {get; set;}
    public static List<GameObject> Rocks {get; set;}
    public static List<GameObject> Trees {get; set;}
    public static List<AlteredObject> SpentRemovals {get; set;}
    public static int RegenDays = 3;
    public static List<UnityProjectile> ShotProjectiles {get; set;}
    public static List<UnityProjectile> SpentProjectiles {get; set;}
    public static List<UnityGate> Gates {get; set;}
    public static List<UnityMapItem> MapItems {get; set;}
    public static List<UnityCharacter> DefeatedEnemies {get; set;}

    public static void GetScenes()
    {
        Scenes = new Dictionary<int, string>();
        Scenes.Add(0, "0_Home");
        Scenes.Add(1, "1_Field");
        Scenes.Add(2, "2_Village");
        Scenes.Add(3, "3_Wood");
        Scenes.Add(4, "4_Quarry");
        Scenes.Add(5, "5_Lake");
    }

    public static void GetCharacters()
    {
        Player = new UnityCharacter(GameObject.FindGameObjectWithTag(Tags.Player));
        Characters = new List<UnityCharacter>();
        foreach (GameObject character in GameObject.FindGameObjectsWithTag(Tags.Character))
        {
            UnityCharacter newCharacter = new UnityCharacter(character);
            Characters.Add(newCharacter);
        }
        Enemies = new List<UnityCharacter>();
        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag(Tags.Enemy))
        {
            UnityCharacter newEnemy = new UnityCharacter(enemy);
            Enemies.Add(newEnemy);
        }
    }

    public static void GetObjects()
    {
        Rocks = new List<GameObject>();
        Trees = new List<GameObject>();
        foreach (GameObject rock in GameObject.FindGameObjectsWithTag(Tags.Rock))
        {
            if (Rocks.Contains(rock) == false)
            {
                Rocks.Add(rock);
            }
        }
        foreach (GameObject tree in GameObject.FindGameObjectsWithTag(Tags.Tree))
        {
            if (Trees.Contains(tree) == false){
                Trees.Add(tree);
            }
        }
        SpentRemovals = new List<AlteredObject>();
        foreach (AlteredObject po in Saves.GameState.AlteredObjects)
        {
            if (po.Scene == SceneManager.GetActiveScene().name)
            {
                if (po.Change == "Addition")
                {
                    GameObject loadedPo;
                    if (po.DaysAltered >= RegenDays)
                    {
                        loadedPo = GameObject.Instantiate(Resources.Load("Plants/" + po.endPrefab, typeof(GameObject))) as GameObject;
                    } 
                    else 
                    {
                    loadedPo = GameObject.Instantiate(Resources.Load("Plants/" + po.startPrefab, typeof(GameObject))) as GameObject;
                    }
                    po.Identifier = loadedPo.GetInstanceID();
                    loadedPo.transform.position = new Vector3(po.PositionX, po.PositionY, po.PositionZ);
                } 
                else if (po.Change == "Removal")
                {
                    Vector3 poPosition = new Vector3(po.PositionX, po.PositionY, po.PositionZ);
                    if (po.startPrefab == "MapItem")
                    {
                        foreach (UnityMapItem mapItem in MapItems)
                        {
                            if (mapItem.Object.transform.position.x == poPosition.x && mapItem.Object.transform.position.z == poPosition.z)
                            {
                                GameObject.Destroy(mapItem.Object);
                                MapItems.Remove(mapItem);
                                break;
                            }
                        }
                    } else if (po.DaysAltered < RegenDays)
                    {
                        if (po.startPrefab == "Tree")
                        {
                            foreach (GameObject tree in Trees)
                            {
                                if (tree.transform.position.x == poPosition.x && tree.transform.position.z == poPosition.z)
                                {
                                    GameObject.Destroy(tree);
                                    Trees.Remove(tree);
                                    break;
                                }
                            }
                        } else if (po.startPrefab == "Rock")
                        {
                            foreach (GameObject rock in Rocks)
                            {
                                if (rock.transform.position.x == poPosition.x && rock.transform.position.z == poPosition.z)
                                {
                                    GameObject.Destroy(rock);
                                    Rocks.Remove(rock);
                                    break;
                                }
                            }
                        }
                    } else {
                        SpentRemovals.Add(po);
                    }
                }
            }
        }
        foreach (AlteredObject po in SpentRemovals)
        {
            Saves.GameState.AlteredObjects.Remove(po);
        }
        SpentRemovals.Clear();
    }

    public static void InitialiseLists()
    {
        ShotProjectiles = new List<UnityProjectile>();
        SpentProjectiles = new List<UnityProjectile>();
        Gates = new List<UnityGate>();
        MapItems = new List<UnityMapItem>();
        DefeatedEnemies = new List<UnityCharacter>();
    }

    public static void DiscardObsoleteObjects()
    {
        foreach (UnityProjectile projectile in ShotProjectiles)
        {
            if ((projectile.Rigidbody.transform.position - projectile.Origin).magnitude >= projectile.Distance || ((projectile.Rigidbody.transform.position - projectile.Origin).magnitude >= 0.1 && (projectile.Rigidbody.velocity - Vector3.zero).magnitude <= 1))
            {
                SpentProjectiles.Add(projectile);
            }
        }
        foreach (UnityProjectile projectile in SpentProjectiles)
        {
            ShotProjectiles.Remove(projectile);
            GameObject.Destroy(projectile.Object);
        }
        SpentProjectiles.Clear();
        foreach (UnityCharacter enemy in Enemies)
        {
            Control.FollowCharacter(Player, enemy);
            if (enemy.Health <= 0){
                DefeatedEnemies.Add(enemy);
            }
        }
        foreach (UnityCharacter defeated in DefeatedEnemies)
        {
            Enemies.Remove(defeated);
            GameObject.Destroy(defeated.Object);
        }
        DefeatedEnemies.Clear();
    }

    public static void DiscardAllObjects()
    {
        ShotProjectiles.Clear();
        SpentProjectiles.Clear();
        Characters.Clear();
        Enemies.Clear();
    }

    public static void FastTravel(int sceneNumber)
    {
        if (Saves.GameState.Progress >= sceneNumber && Saves.GameState.CurrentLocation != sceneNumber)
        { 
            Saves.GameState.CurrentLocation = sceneNumber;
            Rocks.Clear();
            Trees.Clear();
            MapItems.Clear();
            SceneManager.LoadScene(Scenes[sceneNumber], LoadSceneMode.Single);
            Saves.GameState.InventoryItems = Player.Storage.StoredItems;
            Saves.GameState.Funds = Player.Coins;
            Saves.SaveGame(Saves.GameState);
        }
    }
}

public enum BarrierTypes
{
    Barrier,
    Gate
}

public abstract class Barrier
{
    public BarrierTypes Type {get; set;}
    public int Durability {get; set;}

    public Barrier(int setDurability = 0)
    {
        Type = BarrierTypes.Barrier;
        Durability = setDurability;
    }
}

public abstract class Gate : Barrier {
    public int Destination {get; set;}
    public bool Locked {get; set;}

    public Gate(bool setLocked = false)
    {
        Type = BarrierTypes.Gate;
        Locked = setLocked;
    }
}

public class UnityBarrier : Barrier
{
    public GameObject Object {get; set;}
    public Rigidbody Rigidbody {get; set;}
    public Collider Collider {get; set;}
    public int Identifier {get; set;}

    public UnityBarrier(string barrierPrefab)
    {
        Object = (GameObject) GameObject.Instantiate(Resources.Load("Objects/" + barrierPrefab, typeof(GameObject)));
        Rigidbody = Object.GetComponent<Rigidbody>();
        Collider = Object.GetComponent<Collider>();
        Identifier = Object.GetInstanceID();
    }
}

public class UnityGate : Gate
{
    public GameObject Object {get; set;}
    public Rigidbody Rigidbody {get; set;}
    public Collider Collider {get; set;}
    public int Identifier {get; set;}

    public UnityGate(GameObject gate)
    {
        Object = gate;
        Rigidbody = Object.GetComponent<Rigidbody>();
        Collider = Object.GetComponent<Collider>();
        Identifier = Object.GetInstanceID();
    }
}

public class UnityMapItem
{
    public GameObject Object {get; set;}
    public Rigidbody Rigidbody {get; set;}
    public Collider Collider {get; set;}
    public int Identifier {get; set;}
    public Item linkedItem {get; set;}

    public UnityMapItem(GameObject mapItem, Item item)
    {
        Object = mapItem;
        Rigidbody = Object.GetComponent<Rigidbody>();
        Collider = Object.GetComponent<Collider>();
        Identifier = Object.GetInstanceID();
        linkedItem = item;
    }
}

[System.Serializable]
public class AlteredObject
{
    public string Change {get; set;}
    public string startPrefab {get; set;}
    public string endPrefab {get; set;}
    public string Scene {get; set;}
    public float PositionX {get; set;}
    public float PositionY {get; set;}
    public float PositionZ {get; set;}
    public int DaysAltered {get; set;}
    public int Identifier {get; set;}

    public AlteredObject(string setChange, string setPrefab, Scene setScene, Vector3 setPosition, int setIdentifier, int setDaysAltered = 0)
    {
        Change = setChange;
        startPrefab = setPrefab.Replace("(Clone)","");
        endPrefab = startPrefab.Replace("Start","End");
        Scene = setScene.name;
        PositionX = setPosition.x;
        PositionY = setPosition.y;
        PositionZ = setPosition.z;
        DaysAltered = setDaysAltered;
        Identifier = setIdentifier;
    }
}
