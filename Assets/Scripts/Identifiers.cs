using UnityEngine;

public class Identifiers : MonoBehaviour
{
    public int Identifier {get; set;}

    void Start()
    {
        if (this.gameObject.tag == "Gate")
        {
            UnityGate newGate = new UnityGate(this.gameObject);
            newGate.Destination = this.Identifier;
            Map.Gates.Add(newGate);
        } 
        else if (this.gameObject.tag == "MapItem")
        {
            UnityMapItem foundItem = new UnityMapItem(this.gameObject, Items.GameItems[this.Identifier]);
            Map.MapItems.Add(foundItem);
        }
    }
}
