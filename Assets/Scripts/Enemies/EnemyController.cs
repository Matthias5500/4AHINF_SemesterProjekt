using System;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;

namespace DefaultNamespace {
    public class EnemyController : MonoBehaviour {
        [SerializeField] private GameObject enemy;
        [SerializeField] private GameObject PlayerObject;
        [SerializeField] private Image EnemyHealthbar;
        [SerializeField] private GameObject chest;
        Zombie zombie = new Zombie(0,0,0,0,null);
        Bomber bomber = new Bomber(0,0,0,0,null);

        private bool inRadius = true;
        
        private float nextActionTime = 1;

        private void FixedUpdate() {
            DrawLineToPlayer();
        }

        void OnTriggerEnter2D(Collider2D other) {
            if (other.gameObject.CompareTag("Player")) {
                inRadius = true;
            }
        }
        
        void OnTriggerExit2D(Collider2D other) {
            if (other.gameObject.CompareTag("Player")) {
                inRadius = false;
            }
        }
        
        public void TakeDamageFromPlayer(GameObject weapon) {
            var weaponDamage = 0f;
            var position = transform.position;
            if (weapon.CompareTag("Missile")) { //Missile deals double damage
                weaponDamage = 66f;
            }else if (weapon.CompareTag("Sword")) {
                weaponDamage = 33f;
            }
            //knockback
            if (PlayerObject.transform.position.x < position.x) {
                position = new Vector3(position.x + 0.2f, position.y, position.z);
            }
            else {
                position = new Vector3(position.x - 0.2f, position.y, position.z);
            }
            //Damage enemy
            if(enemy.gameObject.CompareTag("Bomber"))
               Destroy(enemy.gameObject);
            if (enemy.gameObject.CompareTag("Zombie"))
                EnemyHealthbar.fillAmount -= weaponDamage / 100f;
            //if healthbar is empty, destroy enemy
            if(EnemyHealthbar.fillAmount <=0.1f) {
                //spawn chest
                GameObject newchest = Instantiate(chest);
                newchest.transform.position = gameObject.transform.position;
                Destroy(enemy.gameObject);
            }
        }

        private void DrawLineToPlayer() {
            int layermask = 1 << 7 | 1 << 9;//only collide with layer 7 (player) and 9 (platforms)
            //layermask = ~layermask;  //inverts the LayerMask. Collides with everything except the Player
           Vector3 raycastdirection = PlayerObject.transform.position - transform.position;
           //Raycast to the direction of the player
           RaycastHit2D hit = Physics2D.Raycast(transform.position, raycastdirection, 2,layermask);
           
           if(hit.collider != null && hit.collider.tag == "Player") {
               Debug.DrawLine(transform.position, hit.point, Color.red);
               SeesPlayer();
           }
        }
        
        private void SeesPlayer() {
            float period = 1f;
            var enemyTag = enemy.tag;
            var player = PlayerObject.GetComponent(nameof(PlayerController)) as PlayerController;
            var dist = Vector3.Distance(PlayerObject.transform.position, transform.position);
            if (player.CompareTag("Player")) { //if player is in range
                if (player) { //if player is not null
                    if (dist <= 0.2f) { //if player is in attack range
                        if (enemyTag == "Zombie"){ //if zombie
                            if (Time.time > nextActionTime) { //Do every x seconds
                                nextActionTime = period + Time.time; //set next action time to x seconds in the future
                                zombie.DealDamage(10, player); //deal damage to player
                            }
                        }
                        else if (enemyTag == "Bomber") {    //if bomber
                            bomber.DealDamage(50, player);
                            Destroy(enemy);
                        }
                    }
                    //turn towards player
                    if (player.transform.position.x < transform.position.x) {
                        transform.localScale = Vector3.one;
                    }
                    else {
                        transform.localScale = new Vector3(-1,1,1);
                    }
                    //move towards player
                    transform.position = Vector3.MoveTowards(transform.position, player.transform.position, .01f);
                }
            }
        }
    }
}
