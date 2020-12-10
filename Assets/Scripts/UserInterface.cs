using UnityEngine;
using UnityEngine.EventSystems;

public class UserInterface : MonoBehaviour, IPointerClickHandler{

    public void OnPointerClick(PointerEventData pointerEventData){
        GameObject clickedToggle = pointerEventData.pointerPress;
        if (clickedToggle.tag == "ShortcutToggle" && Trading.InTrade == false){
            Inventory.Equip(GameController.Instance.Player, clickedToggle);
        } else if (clickedToggle.tag == "ShortcutToggle" && Trading.InTrade == true){
            Trading.TradeItem(clickedToggle);
        }
    }
}
