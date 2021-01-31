using UnityEngine;
using System.Collections;
using CommonGames.Utilities.CGTK;
using UnityEngine.SceneManagement;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace Game.Actors.Health
{
    public sealed class PlayerHealth : Health
    {
        #if ODIN_INSPECTOR
        [LabelText(text: "Heal to multiple of:")]
        #endif
        [SerializeField] private int multiple = 25;
        [SerializeField] private SceneReference credits;
        
        [Space(10)]

        [SerializeField] private float healthPerSecond = 2;

        private bool IsNearestMultipleOf(in int multiple) => ((Mathf.RoundToInt(HealthPoints) % multiple) == 0);
        
        private void Update()
        {
            HealOverTime();
        }

        private void HealOverTime()
        {
            if (IsNearestMultipleOf(multiple))
            {
                HealthPoints = Mathf.RoundToInt(HealthPoints);
                return; //Stop healing at nearest Multiple.
            }

            
            float __healthPerFrame = healthPerSecond * Time.deltaTime;

            HealthPoints += __healthPerFrame;
        }

        public override void Kill()
        {
            Debug.LogWarning("Play audio and delay!");
            StartCoroutine(EndGame());
        }

        IEnumerator EndGame()
        {
            while(false) //check if audio stopped playing
            {
                yield return new WaitForSeconds(1);
            }

            QuitGame.Instance.Quit();
            SceneManager.LoadScene(credits);
        }
    }
}
