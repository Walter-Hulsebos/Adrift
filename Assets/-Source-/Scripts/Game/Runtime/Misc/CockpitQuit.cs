using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using CommonGames.Utilities.CGTK;

namespace Game
{
    public class CockpitQuit : MonoBehaviour
    {
        [SerializeField] private SceneReference credits;
        public void Quit()
        {
            QuitGame.Instance.Quit();
            SceneManager.LoadScene(credits);
        }
    }
}
