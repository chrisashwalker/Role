using UnityEngine;

class Init : MonoBehaviour
{
    void Start()
    {
        Map.GetScenes();
        Map.GetCharacters();
        Saves.LoadGame();
        Items.GetItems();
        Map.GetObjects();
        UI.GetMainCamera();
        Timeflow.GetSunlight();
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
        Timeflow.Tick();
    }
}
