using System.Collections;
using System.Collections.Generic;
using Game.Actors.Health;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game
{
    public class DamageTester : MonoBehaviour
    {
        [SerializeField] private Health health;
        
        [Button]
        private void PrintHealth()
        {
            Debug.Log(message: $"Health = {(int)health}");
        }

        [Button]
        private void DoDamage(int hp)
        {
            health -= hp;
            Debug.Log(message: $"Health = {(int)health}");
        }
        
        [Button]
        private void DoHeal(int hp)
        {
            health += hp;
            Debug.Log(message: $"Health = {(int)health}");
        }

        [Button]
        private void Damage()
        {
            health--;
            Debug.Log(message: $"Health = {(int)health}");
        }
        
        [Button]
        private void Heal()
        {
            health++;
            Debug.Log(message: $"Health = {(int)health}");
        }
        
        [Button]
        private void HasDamage()
        {
            Debug.Log(message: $"Has Damage = {health.HasDamage}");
        }
        
        [Button]
        private void IsDead()
        {
            Debug.Log(message: $"Has Damage = {health.IsDead}");
        }
    }
}
