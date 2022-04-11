using UnityEngine;

namespace DefaultNamespace
{
    public class Zombie : Enemy
    {
        public Zombie(float size, float speed, float damage, float health, Sprite sprite) : base(size, speed, damage, health, sprite)
        {
        }

        public void DealDamage(int decrement, PlayerController player)
        {
            player.DecreasePlayerHealth(decrement);
        }
        
    }
}