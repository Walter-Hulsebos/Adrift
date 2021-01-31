using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CGTK.Utilities.Singletons;

namespace Game
{
    using Actors.Health;

    public class PlayerController : Singleton<PlayerController>
    {
        #region Variables

        #region Public

        /// <summary>
        /// Event that will be triggered when the player collides with an object
        /// </summary>
        /// <param name="force">The speed the player was going at at the moment of impact</param>
        public delegate void HitObstacle(float force);
        public HitObstacle onHitObstacle;
        
        

        #endregion

        #region Private

        #region Exposed

        [Range(-1, 1)]
        public float rotationSlider = 0;
        [Range(0, 1)]
        public float accelerationSlider = 0;

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

        private void OnTriggerEnter2D(Collider2D collision)
        {
            Debug.Log("Hit object!");
            if(TryGetComponent(component: out Health __health))
            {
                __health -= Mathf.RoundToInt(25 * (currentSpeed / maxSpeed));
            }

            onHitObstacle?.Invoke(currentSpeed/maxSpeed);
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
