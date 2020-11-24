using UnityEngine;
using UnityEngine.EventSystems;

public class UserInterface : MonoBehaviour, IPointerClickHandler{

    public void OnPointerClick(PointerEventData pointerEventData){
        GameObject clickedToggle = pointerEventData.pointerPress;
        if (clickedToggle.tag == "ItemToggle"){
            Inventory.Equip(clickedToggle);
        }
    }
}
