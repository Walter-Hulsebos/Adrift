using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class FireButton : MonoBehaviour
    {
        bool state = false;

        public void Fire()
        {
            PlayerController.Instance.GetComponent<Shooter>().Fire();
        }

        public void Lock()
        {
            PlayerController.Instance.GetComponent<Targeter>().LockTarget = !state;
            state = !state;
        }
    }
}
