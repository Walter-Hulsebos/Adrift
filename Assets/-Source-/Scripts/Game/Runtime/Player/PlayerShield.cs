using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class PlayerShield : MonoBehaviour
    {
        [field: SerializeField]
        public byte team;

        public float currentShieldHealth;
        public int maxShieldHealth;

        bool wasHit;

        int regenSpeed = 4;
        int reGenDelay = 3, reGenDelayFromEmpty = 5;
        float timeSinceHit;

        public delegate void ShieldsDown();
        public ShieldsDown onShieldsDown;

        public delegate void ShieldsRecharged();
        public ShieldsRecharged onShieldsRecharged;

        // Start is called before the first frame update
        void Start()
        {
            currentShieldHealth = maxShieldHealth;
            PlayerController.Instance.onHitObstacle += Impact;
        }

        void Impact(float force)
        {
            currentShieldHealth -= currentShieldHealth * force;
            wasHit = true;
            timeSinceHit = 0;
        }

        // Update is called once per frame
        void Update()
        {
            if(!wasHit)
            {
                ReGen();
            }
            else
            {
                timeSinceHit += Time.deltaTime;

                if(currentShieldHealth <= 0) //Regenning from empty
                {
                    if(timeSinceHit >= reGenDelayFromEmpty)
                    {
                        wasHit = false;
                    }
                }
                else //Regenning from partially full.
                {
                    if (timeSinceHit >= reGenDelay)
                    {
                        wasHit = false;
                    }
                }
            }
        }

        void ReGen()
        {
            if(currentShieldHealth < maxShieldHealth)
            {
                currentShieldHealth += regenSpeed * Time.deltaTime;
                Mathf.Clamp(currentShieldHealth, 0, maxShieldHealth);

                if(currentShieldHealth == maxShieldHealth)
                {
                    onShieldsRecharged?.Invoke();
                }
            }
        }

        public void Damage(int damage)
        {
            wasHit = true;
            timeSinceHit = 0;
            currentShieldHealth -= damage;

            Mathf.Clamp(currentShieldHealth, 0, maxShieldHealth);

            if(currentShieldHealth <= 0)
            {
                onShieldsDown?.Invoke();
            }
        }
    }
}
