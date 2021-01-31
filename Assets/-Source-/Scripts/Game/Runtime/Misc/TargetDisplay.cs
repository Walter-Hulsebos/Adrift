using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{

    public class TargetDisplay : MonoBehaviour
    {
        [SerializeField] private Image targetDisplay;

        void Update()
        {
            if (PlayerController.Instance.TryGetComponent<Targeter>(out Targeter _targeter))
            {
                if(_targeter.TargetSprite != null)
                {
                    targetDisplay.gameObject.SetActive(true);
                    targetDisplay.sprite = _targeter.TargetSprite;
                    targetDisplay.color = _targeter.CurrentTarget.transform.GetComponent<Image>().color;
                }
                else
                {
                    targetDisplay.gameObject.SetActive(false);
                }

                
            }
            else
            {
                targetDisplay.gameObject.SetActive(false);
            }
        }
    }
}
