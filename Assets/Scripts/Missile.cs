using System.Data.Common;
using UnityEngine;

namespace DefaultNamespace
{
    public class Missile : MonoBehaviour
    {

        private int RaysToShoot = 30;
        float angle = 0;
        private float speed = .01f;

        //Scans surrounding area for enemies
        private void FixedUpdate() {
            //create an array of raycasts
            RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, 1f, Vector2.zero);
            foreach (var hit in hits) { //for each raycast
                if(hit.collider.gameObject.layer == 11) { //if it hits an enemy
                    //draw a Line in the debug window to the enemy
                    Debug.DrawLine(transform.position, hit.collider.gameObject.transform.position, Color.green);
                    //direction to enemy
                    var dir = (hit.collider.gameObject.transform.position - transform.position ).normalized;
                    if (dir != Vector3.zero) { //if the direction is not zero
                        //calculate the angle between the direction and the x axis
                        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                        //set the rotation of the missile to the angle so it faces the enemy
                        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                    }
                    //shoot the missile
                    transform.position = Vector3.MoveTowards(transform.position, hit.transform.position, speed);
                    //calculate the distance between the missile and the enemy
                    float distance = Vector3.Distance(transform.position, hit.transform.position);
                    if (distance < 0.1f) { //if the distance is less than 0.1 destroy the missile and damage the enemy
                        var enemy = hit.collider.gameObject.GetComponent(nameof(EnemyController)) as EnemyController;
                        enemy.TakeDamageFromPlayer(gameObject);
                        Destroy(gameObject);
                    }
                }
            }
        }
    }
}