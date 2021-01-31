using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Asteroid : MonoBehaviour, IHittable
    {
        #region Variables

        #region Public

        public int amountOfNeutronium = 0;
        

        public delegate void AsteroidDestroyed(int resources);
        public AsteroidDestroyed onAsteroidDestroyed;

        #endregion

        #region Private

        #region Exposed

        [SerializeField] private Color emptyColor, fullColor;

        #endregion

        private bool hasNeutronium;
        private bool isTurning;
        private float turnSpeed;
        private float health;

        private int minNeutronium = 10, maxNeutronium = 150;
        private const float minSize = 0.5f, maxSize = 1.5f;
        private const int minRot = 0, maxRot = 360;
        private const int minTurnSpeed = 25, maxTurnSpeed = 60;
        private const int minHealth = 100, maxHealth = 120;

        private SpriteRenderer spriteRenderer;
        private Color currentColor;

        #endregion

        #endregion

        #region Methods

        #region Unity Methods

        public void Initialize(bool shouldHaveNeutronium)
        {
            hasNeutronium = shouldHaveNeutronium;

            onAsteroidDestroyed += ResourceManager.Instance.AddNeutronium;

            spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            currentColor = emptyColor;

            if (hasNeutronium)
            {
                amountOfNeutronium = Random.Range(minNeutronium, maxNeutronium);
                currentColor = Color.Lerp(emptyColor, fullColor, (float)amountOfNeutronium / maxNeutronium);
            }

            spriteRenderer.color = currentColor;

            float size = Random.Range(minSize, maxSize);

            transform.localScale = Vector3.one * size;
            transform.localRotation = new Quaternion(0, 0, Random.Range(minRot, maxRot), 0);

            isTurning = (Random.Range(0, 2) != 0);

            if(isTurning) turnSpeed = Random.Range(minTurnSpeed, maxTurnSpeed);

            int sign = Random.Range(0, 2);
            if (sign == 0) sign = -1;

            turnSpeed *= sign;

            health = Random.Range(minHealth, maxHealth) * size;
        }

        private void Update()
        {
            if(isTurning)
            {
                transform.Rotate(0, 0, turnSpeed * Time.deltaTime);
            }
        }

        private void OnDestroy()
        {
            onAsteroidDestroyed -= ResourceManager.Instance.AddNeutronium;
        }

        #endregion

        #region Public

        public void Hit(float damage)
        {
            health -= damage;

            if(health < 0)
            {
                spriteRenderer.enabled = false;
                EffectManager.Instance.SpawnExplosion(transform.position, currentColor);
                onAsteroidDestroyed?.Invoke(amountOfNeutronium);
                Destroy(gameObject);
            }
        }

        #endregion

        #endregion
    }
}
