using UnityEngine;

public class Combat : MonoBehaviour {
    private void OnCollisionEnter(Collision collision){
        foreach (UnityProjectile projectile in Actions.ShotProjectiles){
            if (projectile.Collider == collision.collider){
                Actions.SpentProjectiles.Add(projectile);
                foreach (UnityCharacter enemy in World.EnemyList){
                    if (enemy.Object == this.gameObject) {
                        enemy.Health -= 1;
                    }
                }
            }
        }
    }
}
