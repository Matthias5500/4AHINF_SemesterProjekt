using System.Collections;
using UnityEngine;

namespace DefaultNamespace
{
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] private GameObject enemy;

        [SerializeField] private Sprite zombieSprite;
        
        Zombie zombie = new Zombie(0,0,0,0,null);

    
        
        //Move to player
        private void OnTriggerStay2D(Collider2D other)
        {
            {
                Sprite sprite = enemy.GetComponent<Sprite>();
                var player = other.GetComponent(nameof(PlayerController)) as PlayerController;
                if (other.CompareTag("Player"))
                {
                    if (player)
                    {
                        {
                            float dist = Vector3.Distance(other.transform.position, transform.position);
                            if (dist <= 0.2f)
                            {
                                
                                if (sprite == zombieSprite)
                                {
                                    zombie.DealDamage(10, player);
                                }
                            }
                                
                            Debug.Log("Test");
                            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, .01f);
                        }
                    }
                }
            }
        }
    }
}
