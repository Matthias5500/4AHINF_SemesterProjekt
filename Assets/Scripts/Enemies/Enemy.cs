using UnityEngine;

namespace DefaultNamespace
{
    public class Enemy
    {
        private float size;
        private float speed;
        private float damage;
        private float health;
        private Sprite sprite;

        public Enemy(float size, float speed, float damage, float health, Sprite sprite)
        {
            this.size = size;
            this.speed = speed;
            this.damage = damage;
            this.health = health;
            this.sprite = sprite;
        }
    }
}