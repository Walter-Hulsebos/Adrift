using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class CircleDrawer : MonoBehaviour
    {
        private float thetaScale = 0.01f;
        private int size;
        [SerializeField] private LineRenderer lineDrawer;
        private float theta = 0f;

        public void Setup(float width, Color color)
        {
            lineDrawer.startWidth = width;
            lineDrawer.endWidth = width;
            lineDrawer.startColor = color;
            lineDrawer.endColor = color;
        }

        public void DrawCircle(int radius)
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
    }
}
