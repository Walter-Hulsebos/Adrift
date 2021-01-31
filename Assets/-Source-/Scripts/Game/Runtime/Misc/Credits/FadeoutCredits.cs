using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using CommonGames.Utilities.CGTK;

namespace Game
{
    public class FadeoutCredits : MonoBehaviour
    {
        public MultiScene scene;

        public Image image;

        int fadeoutTime = 2;

        bool selected = false;

        public void Quit()
        {
            if (!selected)
            {
                selected = true;
                Application.Quit();
            }
        }

        public void Retry()
        {
            if (!selected)
            {
                StartCoroutine(Fadeout());
            }         
        }

        IEnumerator Fadeout()
        {
            selected = true;
            float timer = 0;

            while (timer < fadeoutTime)
            {
                timer += Time.deltaTime;

                image.color = Color.Lerp(new Color(0, 0, 0, 0), new Color(0, 0, 0, 1), timer / fadeoutTime);
                yield return null;
            }

            scene.Load();
        }
    }
}
