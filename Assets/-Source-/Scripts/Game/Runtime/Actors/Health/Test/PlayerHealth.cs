using UnityEngine;

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
        [SerializeField] private int multiple = 10;
        
        [Space(10)]

        [SerializeField] private float healthPerSecond = 2;

        private bool IsNearestMultipleOf(in int multiple) => ((HealthPoints % multiple) == 0);
        
        private void Update()
        {
            HealOverTime();
        }

        private void HealOverTime()
        {
            if (IsNearestMultipleOf(multiple)) return; //Stop healing at nearest Multiple.
            
            int __healthPerFrame = Mathf.RoundToInt(healthPerSecond * Time.deltaTime);

            HealthPoints += __healthPerFrame;
        }

        public override void Kill()
        {
            throw new System.NotImplementedException();
        }
    }
}
