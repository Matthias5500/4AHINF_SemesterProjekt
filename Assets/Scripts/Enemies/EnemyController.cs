using System;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;

namespace DefaultNamespace
{
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] private GameObject enemy;
        [SerializeField] private GameObject PlayerObject;
        [SerializeField] private Image EnemyHealthbar;
        Zombie zombie = new Zombie(0,0,0,0,null);
        Bomber bomber = new Bomber(0,0,0,0,null);

        private bool inRadius = true;
        
        private float nextActionTime = 1;

        private void FixedUpdate()
        {
            DrawLineToPlayer();
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                inRadius = true;
            }
        }
        
        void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                inRadius = false;
            }
        }

        //Move to player
        private void OnTriggerStay2D(Collider2D other)
        {
            
        }

        public void TakeDamageFromPlayer()
        {
            //knockback
            if (PlayerObject.transform.position.x < transform.position.x)
            {
                transform.position = new Vector3(transform.position.x + 0.2f, transform.position.y, transform.position.z);
            }
            else
            {
                transform.position = new Vector3(transform.position.x - 0.2f, transform.position.y, transform.position.z);
            }
            if(enemy.gameObject.CompareTag("Bomber"))
               Destroy(enemy.gameObject);
            if (enemy.gameObject.CompareTag("Zombie"))
                EnemyHealthbar.fillAmount -= 33f / 100f;
            
            if(EnemyHealthbar.fillAmount <=0.1f)
            {
                Destroy(enemy.gameObject);
            }
        }

        private void DrawLineToPlayer()
        {
            int layermask = 1 << 7; //only collide with layer 7 (player)
           //layermask = ~layermask;  //inverts the LayerMask. Collides with everything except the Player
           Vector3 raycastdirection = PlayerObject.transform.position - transform.position;
           //Raycast to the direction of the player
           RaycastHit2D hit = Physics2D.Raycast(transform.position, raycastdirection, 2,layermask);
           
           if(hit.collider != null && hit.collider.tag == "Player")
           {
               Debug.DrawLine(transform.position, hit.point, Color.red);
               SeesPlayer();
           }
           else
           {
               Debug.DrawLine(transform.position,raycastdirection, Color.white);
           }

        }

        private void SeesPlayer()
        {
            float period = 1f;
            {
                var enemyTag = enemy.tag;
                var player = PlayerObject.GetComponent(nameof(PlayerController)) as PlayerController;
                var dist = Vector3.Distance(PlayerObject.transform.position, transform.position);
                //if player is in range
                if (player.CompareTag("Player"))
                {
                    //if player is not null
                    if (player)
                    {
                        if (dist <= 0.2f)
                        {
                            if (enemyTag == "Zombie") //if zombie
                            {
                                //Do every x seconds
                                if (Time.time > nextActionTime)
                                {
                                    nextActionTime = period + Time.time;
                                    zombie.DealDamage(10, player);
                                }
                            }
                            else if (enemyTag == "Bomber") //if bomber
                            {
                                bomber.DealDamage(50, player);
                                Destroy(enemy);
                            }
                        }

                        {
                            //turn towards player
                            if (player.transform.position.x < transform.position.x)
                            {
                                transform.localScale = Vector3.one;
                            }
                            else
                            {
                                transform.localScale = new Vector3(-1,1,1);
                            }
                            
                            //move towards player
                            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, .01f);
                        }
                    }
                }
            }
        }
    }
}
