using UnityEngine;

public class CollisionTest : MonoBehaviour{
    void OnCollisionStay(Collision collision){
        GameController.Instance.CollisionCheck(collision);
    }
}
