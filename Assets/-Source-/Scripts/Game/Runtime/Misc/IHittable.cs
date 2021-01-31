using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public interface IHittable
    {
        void Hit(float damage);
    }
}
