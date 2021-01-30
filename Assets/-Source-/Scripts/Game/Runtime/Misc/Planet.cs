using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Planet : MonoBehaviour
    {
        #region Variables

        #region Private

        #region Exposed

        [SerializeField] private Color[] colourOptions;

        #endregion

        private float size;
        private SpriteRenderer spriteRenderer;

        private const float minSize = 3f, maxSize = 5f;
        private const int minRot = 0, maxRot = 360;

        #endregion

        #endregion

        public void Start()
        {
            spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

            int colourOption = Random.Range(0, colourOptions.Length);
            spriteRenderer.color = colourOptions[colourOption];

            float size = Random.Range(minSize, maxSize);

            transform.localScale = Vector3.one * size;
            transform.localRotation = new Quaternion(0, 0, Random.Range(minRot, maxRot), 0);
        }
    }
}