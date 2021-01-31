using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CGTK.Utilities.Singletons;

namespace Game
{
    public class MusicPlayer : PersistentSingleton<MusicPlayer>
    {
        public void Toggle(bool state)
        {
            if (state) GetComponent<AudioSource>().Play();
            else GetComponent<AudioSource>().Stop();
        }
    }
}
