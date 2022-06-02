using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DefaultNamespace {
    public class Spawner : MonoBehaviour {
        [SerializeField] private GameObject zombie;
        [SerializeField] private GameObject bomber;
        [SerializeField] private GameObject PlayerObject;
        private int EnemyCount = 3;//must be 1 higher than the number of enemies
        private float nextActionTime = 1;
        [SerializeField] private float TimeTilEnemySpawns = 1f;

        private void FixedUpdate() {
            //Do every x seconds
            if (Time.time > nextActionTime) {
                nextActionTime = TimeTilEnemySpawns + Time.time;
                GameObject newEnemy = Instantiate(GetRandomEnemy());
                newEnemy.transform.position = transform.position;
            }

        }
        
        public GameObject GetRandomEnemy() {
            GameObject randomEnemy = null;
            int rand = Random.Range(1, EnemyCount);
            if (rand == 1) // 1
                randomEnemy = zombie;
            else if(rand == 2) // 2
                randomEnemy = bomber;
            return randomEnemy;
        }
    }   
}