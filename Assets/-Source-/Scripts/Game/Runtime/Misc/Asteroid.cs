using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Asteroid : MonoBehaviour
    {
        #region Variables

        #region Public

        public int amountOfNeutronium = 0;
        public bool hasNeutronium = false;

        #endregion

        #region Private

        #region Exposed

        [SerializeField] private Color emptyColor, fullColor;

        #endregion

        private bool isTurning;
        private float turnSpeed;

        private int minNeutronium = 10, maxNeutronium = 150;
        private const float minSize = 0.5f, maxSize = 1.5f;
        private const int minRot = 0, maxRot = 260;
        private const int minTurnSpeed = 25, maxTurnSpeed = 60;

        private const int minMineValue = 1, maxMineValue = 10;

        private SpriteRenderer renderer;

        #endregion

        #endregion

        #region Methods

        #region Unity Methods

        void Start()
        {
            if (hasNeutronium) amountOfNeutronium = Random.Range(minNeutronium, maxNeutronium);

            renderer = gameObject.GetComponent<SpriteRenderer>();
            renderer.color = emptyColor;

            if (hasNeutronium)
            {
                renderer.color = Color.Lerp(emptyColor, fullColor, amountOfNeutronium / maxNeutronium);
            }

            transform.localScale = Vector3.one * Random.Range(minSize, maxSize);
            transform.localRotation = new Quaternion(0, 0, Random.Range(minRot, maxRot), 0);
            turnSpeed = Random.Range(minTurnSpeed, maxTurnSpeed);

            isTurning = (Random.Range(0, 2) != 0);

            if(isTurning) turnSpeed = Random.Range(minTurnSpeed, maxTurnSpeed);

            int sign = Random.Range(0, 2);
            if (sign == 0) sign = -1;

            turnSpeed *= sign;
        }

        private void Update()
        {
            if(isTurning)
            {
                transform.Rotate(0, 0, turnSpeed * Time.deltaTime);
            }
        }

        #endregion

        #region Public

        public int Mine()
        {
            int amount = Random.Range(minMineValue, maxMineValue);
            amountOfNeutronium -= amount;

            if (hasNeutronium)
            {
                renderer.color = Color.Lerp(emptyColor, fullColor, amountOfNeutronium / maxNeutronium);
            }

            return amount;
        }

        #endregion

        #endregion
    }
}
