using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class PlayerController : MonoBehaviour
    {
        #region Variables

        #region Private

        #region Exposed

        [Range(-1, 1)]
        [SerializeField] private float rotationSlider = 0;
        [Range(0, 1)]
        [SerializeField] private float accelerationSlider = 0;

        [SerializeField] private float accelerationSpeed = 1.5f;
        [SerializeField] private float deccelerationSpeed = 3;
        [SerializeField] private int maxSpeed = 4;

        [SerializeField] private int rotationSpeed = 5;

        #endregion

        private float maxAllowedSpeed = 0;
        private float currentSpeed = 0;

        #endregion

        #endregion

        #region Methods

        #region Unity Methods


        void Update()
        {
            Rotate(-rotationSlider);
            Accelerate(accelerationSlider);

            //TODO [Wybren]: Potentially add retaining of intertia

            if(currentSpeed < maxAllowedSpeed) //Accellerating
            {
                currentSpeed += accelerationSpeed * Time.deltaTime;

                if (currentSpeed > maxAllowedSpeed) currentSpeed = maxAllowedSpeed;
            }
            else if(currentSpeed > maxAllowedSpeed) //Decellerating
            {
                currentSpeed -= deccelerationSpeed * Time.deltaTime;

                if (currentSpeed < maxAllowedSpeed) currentSpeed = maxAllowedSpeed;
            }

            transform.Translate(Vector3.up * currentSpeed * Time.deltaTime, Space.Self);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Accelerate the player
        /// </summary>
        /// <param name="accellerationModifier">0 to 1 value that controls the rate of acceleration</param>
        public void Accelerate(float accellerationModifier)
        {
            maxAllowedSpeed = maxSpeed * accellerationModifier;
        }

        public void Rotate(float rotationModifier)
        {
            transform.Rotate(0, 0, (rotationSpeed * rotationModifier) * Time.deltaTime, Space.World);
        }

        #endregion

        #endregion
    }
}