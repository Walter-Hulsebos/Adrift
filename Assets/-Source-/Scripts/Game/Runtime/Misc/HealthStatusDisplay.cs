using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    using Actors.Health;

    public class HealthStatusDisplay : MonoBehaviour
    {
        [SerializeField] private Image shields;
        [SerializeField] private Image hull;

        [SerializeField] private Color shieldColor, invisibleShield;
        [SerializeField] private Color hullColor, invisibleHull;

        [SerializeField] private List<Image> healthBar = new List<Image>();

        private Health playerHealth;
        private PlayerShield playerShield;

        


        // Start is called before the first frame update
        void Start()
        {
            playerHealth = PlayerController.Instance.GetComponent<Health>();
            playerShield = PlayerController.Instance.GetComponent<PlayerShield>();

            playerShield.onShieldsDown += ShieldsDown;
            playerShield.onShieldsRecharged += ShieldsRecharged;
        }

        // Update is called once per frame
        void Update()
        {
            shields.color = Color.Lerp(invisibleShield, shieldColor, playerShield.currentShieldHealth / playerShield.maxShieldHealth);
            hull.color = Color.Lerp(invisibleHull, hullColor, playerHealth.Percentage);

            healthBar[0].fillAmount = Mathf.Clamp01((playerHealth.Percentage - .75f) / .25f);
            healthBar[1].fillAmount = Mathf.Clamp01((playerHealth.Percentage - .5f) / .25f);
            healthBar[2].fillAmount = Mathf.Clamp01((playerHealth.Percentage - .25f) / .25f);
            healthBar[3].fillAmount = Mathf.Clamp01((playerHealth.Percentage) / .25f);
        }

        void ShieldsDown()
        {
            VoiceManager.Instance.PlayClip(11);
        }

        void ShieldsRecharged()
        {
            VoiceManager.Instance.PlayClip(12);
        }
    }
}
