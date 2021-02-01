using UnityEngine;

public class Core : MonoBehaviour
{
    public void Start()
    {
        Map.GetScenes();
        Saves.LoadGame();
        UI.GetMainCamera();
        Timeflow.GetSunlight();
        Control.GetLayers();
        Map.InitialiseLists();
        Map.GetCharacters();
        Items.GetItems();
        Map.GetObjects();
        UI.LoadHUD();
    }

    public void FixedUpdate()
    {
        Control.MoveCharacter(Map.Player);
        UI.RelocateCamera();
        UI.RelocateTarget();
        UI.UpdateHUD();
        Map.DiscardObsoleteObjects();
        Saves.CheckStatus();
    }

    public void Update()
    {
        Control.GetKeyPress();
        Items.ItemUseCheck();
        Timeflow.Tick();
    }
}
