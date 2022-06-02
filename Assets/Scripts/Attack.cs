using System;
using System.Data.SqlTypes;
using DefaultNamespace;
using UnityEngine;

public class Attack : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 11)
        {
            var enemy = other.gameObject.GetComponent(nameof(EnemyController)) as EnemyController;
            enemy.TakeDamageFromPlayer(gameObject);
            
        }
    }
}