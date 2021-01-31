using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using CommonGames.Utilities.CGTK;
using UnityEngine.SceneManagement;

namespace Game
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private SceneReference credits;
        [SerializeField] private GameObject scanner;

        public int requiredNeutronium = 150;
        [SerializeField] private TMP_Text text;
        int currentOption = 0;
        int selectedOption = -1;

        bool allowJump = false;

        //0 = not enough fuel, 1 = no destination, 2 = ready
        [SerializeField] private List<Color> stateColours = new List<Color>();
        private List<string> stateMessages = new List<string>()
        {
            "Not Ready",
            "Ready To Select",
            "Ready"
        };

        [SerializeField] private List<GameObject> destinations = new List<GameObject>();

        Dictionary<int, int[]> options = new Dictionary<int, int[]>()
        {
            { 0, new int[] {1} },
            { 1, new int[] {2} },
            { 2, new int[] {3} },
            { 3, new int[] {4,9} },
            { 4, new int[] {5} },
            { 5, new int[] {6} },
            { 6, new int[] {7} },
            { 7, new int[] {8} },
            { 8, new int[] {14} },

            { 9, new int[] {10,11} },
            { 10, new int[] {4} },
            { 11, new int[] {12} },
            { 12, new int[] {13} },
            { 13, new int[] {8} },
        };

        // Start is called before the first frame update
        void Start()
        {
            SystemManager.Instance.GenerateLevel(-1, -1, -1, -1, 0);
        }

        private void Update()
        {
            if(ResourceManager.Instance.storedNeutronium >= requiredNeutronium && !allowJump)
            {
                OnReceiveNeutronium(ResourceManager.Instance.storedNeutronium);
            }
        }

        public void OnReceiveNeutronium(int currentNeutronium)
        {
            if(currentNeutronium >= requiredNeutronium)
            {
                VoiceManager.Instance.PlayClip(4);
                allowJump = true;

                int[] optionsToActivate = options[currentOption];
                foreach(int i in optionsToActivate)
                {
                    destinations[i - 1].SetActive(true);
                }

                text.color = stateColours[1];
                text.text = stateMessages[1];
            }
        }

        [ContextMenu("Jump")]
        public void Jump()
        {
            Debug.Log("Test");

            if(!allowJump)
            {
                Debug.Log("Not ready");
                VoiceManager.Instance.PlayClip(8);

                return;
            }

            if(selectedOption != -1)
            {
                if (selectedOption == 14)
                {
                    Debug.Log("Game end!");
                    
                    StartCoroutine(Victory());
                    return;
                }

                ResourceManager.Instance.RemoveNeutronium(requiredNeutronium);

                if(ResourceManager.Instance.storedNeutronium < 0) //Shouldnt be possible but just to be safe.
                {
                    ResourceManager.Instance.storedNeutronium = 0;
                }

                

                //TODO: Jump (I.e. start sequence and refresh level) & play audio

                //TODO: ADD DELAY
                StartCoroutine(Jumping());
                Debug.Log("Jumping!");
            }
            else
            {
                Debug.Log("No Destination");
                VoiceManager.Instance.PlayClip(9);
            }
        }

        public void SelectOption(int option)
        {
            int[] optionsToActivate = options[currentOption];
            foreach (int i in optionsToActivate)
            {
                destinations[i - 1].SetActive(false);
            }

            text.color = stateColours[2];
            text.text = stateMessages[2];

            selectedOption = option;
            VoiceManager.Instance.PlayClip(13);
        }

        IEnumerator Jumping()
        {
            scanner.SetActive(false);

            VoiceManager.Instance.PlayClip(6);

            //TODO: Screenshake
            while(VoiceManager.Instance.IsPlaying)
            {
                yield return null;
            }

            yield return new WaitForSeconds(2);

            SystemManager.Instance.GenerateLevel();
            scanner.SetActive(true);
            VoiceManager.Instance.PlayClip(1);

            while (VoiceManager.Instance.IsPlaying)
            {
                yield return null;
            }

            if(SystemManager.Instance.hasEnemies)
            {
                VoiceManager.Instance.PlayClip(7);
            }

            currentOption = selectedOption;
            selectedOption = -1;
            allowJump = false;

            text.color = stateColours[0];
            text.text = stateMessages[0];
        }

        IEnumerator Victory()
        {
            while(VoiceManager.Instance.IsPlaying)//Check if player finished playing audio.
            {
                yield return new WaitForSeconds(1);
            }

            VoiceManager.Instance.PlayClip(15);

            while (VoiceManager.Instance.IsPlaying)//Check if player finished playing audio.
            {
                yield return null;
            }

            QuitGame.Instance.Quit();
            SceneManager.LoadScene(credits);
        }
    }
}
