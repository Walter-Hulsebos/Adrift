using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class SystemManager : MonoBehaviour
    {
        #region Variables

        #region Public

        public delegate void PlayerExitedSystemBounds();
        public PlayerExitedSystemBounds onPlayerExitedSystemBounds;

        public delegate void PlayerEnteredSystemBounds();
        public PlayerExitedSystemBounds onPlayerEnteredSystemBounds;

        #endregion

        #region Private


        #region Exposed

        [SerializeField] private int levelRadius = 50;

        #endregion

        private float thetaScale = 0.01f;
        private int size;
        private LineRenderer lineDrawer;
        private float theta = 0f;

        bool playerIsOutside;

        #endregion

        #endregion

        #region Methods

        #region Unity

        private void Start()
        {
            lineDrawer = GetComponent<LineRenderer>();
            GenerateLevel();
        }

        void Update()
        {
            //CheckIfOutside(PlayerController.Instance.transform.position);
            DrawCircle(levelRadius);
        }

        #endregion

        #region Public

        public void GenerateLevel()
        {

        }

        #endregion

        #region Private

        private void DrawCircle(int radius)
        {
            theta = 0f;
            size = (int)((1f / thetaScale) + 1f);
            lineDrawer.positionCount = size;
            for (int i = 0; i < size; i++)
            {
                theta += (2.0f * Mathf.PI * thetaScale);
                float x = radius * Mathf.Cos(theta);
                float y = radius * Mathf.Sin(theta);
                lineDrawer.SetPosition(i, new Vector3(x, 0, y));
            }
        }

        private void CheckIfOutside(Vector3 position)
        {
            if (Vector3.Distance(position, transform.position) > levelRadius && !playerIsOutside)
            {
                onPlayerExitedSystemBounds?.Invoke();
                playerIsOutside = true;
            }
            else if(playerIsOutside)
            {
                onPlayerEnteredSystemBounds?.Invoke();
                playerIsOutside = false;
            }
        }

        #endregion

        #endregion
    }
}
