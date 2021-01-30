using System.Collections.Generic;

using UnityEngine;

class Init : MonoBehaviour
{
    Dictionary<int, string> Scenes {get;set;}

    void GetScenes(){
        Scenes = new Dictionary<int, string>();
        Scenes.Add(0, "0_Home");
        Scenes.Add(1, "1_Field");
        Scenes.Add(2, "2_Village");
        Scenes.Add(3, "3_Wood");
        Scenes.Add(4, "4_Quarry");
        Scenes.Add(5, "5_Lake");
    }

    void Start()
    {
        GetScenes();
        Map.GetCharacters();
        Saves.LoadGame();
        Items.GetItems();
        Map.GetObjects();
        UI.GetMainCamera();
        Time.GetSunlight();
        Control.GetLayers();
        UI.LoadHUD();
    }

    void FixedUpdate()
    {
        Control.MoveCharacter(Map.Player);
        UI.RelocateCamera();
        UI.RelocateTarget();
        UI.UpdateHUD();
        Map.DiscardObsoleteObjects();
        Saves.CheckStatus();
    }

    void Update()
    {
        Control.GetKeyPress();
        Time.Tick();
    }
}
