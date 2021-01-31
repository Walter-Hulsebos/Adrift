using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CGTK.Utilities.Singletons;

namespace Game
{
    public class ScreenshakeManager : Singleton<ScreenshakeManager>
    {
        [SerializeField] private AudioSource source;
        [SerializeField] private AudioClip asteroidImpact;
        [SerializeField] private AudioClip energyHit;

        private void Start()
        {
            PlayerController.Instance.onHitObstacle += ShakeAsteroid;
        }

        private void ShakeAsteroid(float force) //(force is 0-1 percentage)
        {
            source.clip = asteroidImpact;
            source.Play();
        }

        public void ShakeHit(int type) //Hit by enemy 0 = shield hit, 1 = hull
        {
            source.clip = energyHit;
            source.Play();
        }
    }
}
