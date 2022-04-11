using System;
using UnityEngine;
using UnityEngine.U2D;
using Random = UnityEngine.Random;

namespace DefaultNamespace
{
    public class Item : MonoBehaviour
    {
        [SerializeField] private GameObject item;
        [SerializeField] Sprite speedItemSprite;
        [SerializeField] Sprite jumpItemSprite;
        private int ItemCount;
        private SpriteRenderer _spriteRenderer;

        public Item(SpriteRenderer spriteRenderer)
        {
            _spriteRenderer = spriteRenderer;
        }
        
        public Sprite GETRandomSprite()
        {
            ItemCount = 3;
            Sprite randItemSprite = null;
            int rand = Random.Range(1, ItemCount);
            if (rand == 1) // 1
                randItemSprite = speedItemSprite;
            else if(rand == 2) // 2
                randItemSprite = jumpItemSprite;
            
            return randItemSprite;
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player")) {
                var playerController = other.GetComponent(nameof(PlayerController)) as PlayerController;
                if (playerController) {
                    if (item.GetComponent<SpriteRenderer>().sprite == speedItemSprite) {
                        playerController.IncreasePlayerSpeed(1.2f);
                    }else if(item.GetComponent<SpriteRenderer>().sprite == jumpItemSprite) {
                        playerController.IncreasePlayerjumpSpeed(1.2f);
                    }
                    Destroy(item);
                }
            }
        }

        
    }
}