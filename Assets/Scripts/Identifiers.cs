using UnityEngine;

public class Identifiers : MonoBehaviour{
    public int identifier;

    void Awake(){
        if (this.gameObject.tag == "Gate"){
            UnityGate newGate = new UnityGate(this.gameObject);
            newGate.Destination = this.identifier;
            World.GateList.Add(newGate);
        } else if (this.gameObject.tag == "MapItem"){
            UnityMapItem foundItem = new UnityMapItem(this.gameObject, Items.GameItems[this.identifier]);
            World.MapItemList.Add(foundItem);
        }
    }
}
