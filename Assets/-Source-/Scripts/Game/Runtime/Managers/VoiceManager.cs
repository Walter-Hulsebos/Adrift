using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CGTK.Utilities.Singletons;

namespace Game
{
    public class VoiceManager : Singleton<VoiceManager>
    {
        [SerializeField] AudioSource source;

        [SerializeField] List<AudioClip> clips = new List<AudioClip>();

        public bool IsPlaying => (source.isPlaying);

        bool state = true;

        public bool PlayClip(int index)
        {
            if(!IsPlaying)
            {
                source.clip = clips[index];
                source.Play();
                return true;
            }
            return false;
        }

        public void ToggleMute()
        {
            source.clip = null;

            state = !state;

            if(state)
            {
                source.volume = 1;
            }
            else
            {
                source.volume = 0;
            }
        }
    }
}
