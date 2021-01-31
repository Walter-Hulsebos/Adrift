using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class IntroManager : MonoBehaviour
    {

        int fxfadeInTime = 5;
        int fxFadeoutTime = 6;

        float vignetteIntensityTarget = 0.441f;
        float vignetteSmoothnessTarget = .2f;

        [SerializeField] private AudioSource audio;
        [SerializeField] private GameObject blocker;

        bool notAwake = true;
        bool playingFX = true;
        bool notFinishedIntro = true;

        void Start()
        {
            MusicPlayer.Instance.Toggle(false);

            //Audio fx afspelen (sparks, etc)
            //Screenshake starten!

            //Audio spelen (voice)
            //Tijdens voice afspelen effects weghalen.
            //wachten tot voice klaar is, dan blocker weghalen.

            StartCoroutine(PlayIntro());
        }

        IEnumerator PlayIntro()
        {
            float timer = 0;
            while (timer < fxfadeInTime && notAwake)
            {
                timer += Time.deltaTime;

                audio.volume = Mathf.Lerp(0, .7f, timer / fxfadeInTime);

                yield return null;
            }

            notAwake = false;
            timer = 0;
            Debug.Log("Awake");

            //screenshake afbouwen
            while (timer < fxFadeoutTime && playingFX)
            {
                timer += Time.deltaTime;

                if(timer > fxFadeoutTime/2) VoiceManager.Instance.PlayClip(0); 

                audio.volume = Mathf.Lerp(.7f, 0, timer / fxFadeoutTime);

                yield return null;
            }

            Debug.Log("Stopped FX");

            playingFX = false;

            

            while(VoiceManager.Instance.IsPlaying && notFinishedIntro)
            {
                yield return null;
            }

            Debug.Log("Finished Intro");

            notFinishedIntro = false;
            blocker.SetActive(false);

            MusicPlayer.Instance.Toggle(true);
        }
    }
}
