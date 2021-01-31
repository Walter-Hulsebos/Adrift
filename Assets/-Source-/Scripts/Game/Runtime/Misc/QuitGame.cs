using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CGTK.Utilities.Singletons;

namespace Game
{
    public class QuitGame : Singleton<QuitGame>
    {
        public void Quit()
        {
            Destroy(SystemManager.Instance.gameObject);
            Destroy(EffectManager.Instance.gameObject);
        }
    }
}
