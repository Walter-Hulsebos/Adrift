using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Game
{
    public class NeutroniumCounter : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;
        [SerializeField] private LevelManager levelManager;

        void Start()
        {
            ResourceManager.Instance.onNeutroniumChanged += UpdateDisplay;
            UpdateDisplay(ResourceManager.Instance.storedNeutronium);
        }

        void UpdateDisplay(int newValue)
        {
            text.text = $"{newValue}/{levelManager.requiredNeutronium}N";
        }
    }
}
