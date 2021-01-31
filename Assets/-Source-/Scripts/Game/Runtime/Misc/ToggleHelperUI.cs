using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class ToggleHelperUI : MonoBehaviour
    {
        bool currentState = true;

        [SerializeField] private List<GameObject> uiObjects = new List<GameObject>();

        public void ToggleUI()
        {
            foreach(GameObject ui in uiObjects)
            {
                ui.SetActive(!currentState);
            }

            currentState = !currentState;
        }
    }
}
