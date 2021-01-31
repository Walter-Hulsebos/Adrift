using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Actors.Health
{
    public class AsteroidHealth : Health
    {
        protected override float Max => 20;

        public override void Kill()
        {
            GetComponent<Asteroid>().Kill();
        }
    }
}
