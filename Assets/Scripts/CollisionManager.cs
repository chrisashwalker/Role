using UnityEngine;

public class CollisionManager : MonoBehaviour{
    void OnCollisionStay(Collision collision){
        GameController.Instance.CollisionCheck(collision);
    }
}
