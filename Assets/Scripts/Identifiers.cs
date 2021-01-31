using UnityEngine;

public class Identifiers : MonoBehaviour
{
    public int identifier;

    void Start()
    {
        if (this.gameObject.tag == Tags.Gate)
        {
            UnityGate newGate = new UnityGate(this.gameObject);
            newGate.Destination = this.identifier;
            Map.Gates.Add(newGate);
        } 
        else if (this.gameObject.tag == Tags.MapItem)
        {
            UnityMapItem foundItem = new UnityMapItem(this.gameObject, Items.GameItems[this.identifier]);
            Map.MapItems.Add(foundItem);
        }
    }
}
