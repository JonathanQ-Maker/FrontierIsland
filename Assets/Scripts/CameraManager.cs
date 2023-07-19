using System;
using UnityEngine;

namespace FrontierIsland
{
    public class CameraManager : MonoBehaviour
    {
        private static CameraManager instance;
        public static CameraManager Instance
        {
            get { return instance; }
        }

        #region public_field
        public float maxCameraSize = 10;
        #endregion

        Plane plane = new Plane(Vector3.up, Vector3.zero);
        Vector3 dragStartPosition;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this);
            }
            else
            {
                instance = this;
            }
        }


        void Update()
        {
            HandleMovement();
            HandleScroll();
        }

        private void HandleMovement()
        {
            // Drag code from https://youtu.be/rnqF6S7PfFA?t=756

            if (Input.GetMouseButtonDown(1))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                float entry;
                if (plane.Raycast(ray, out entry))
                {
                    dragStartPosition = ray.GetPoint(entry);
                }
            }

            if (Input.GetMouseButton(1))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                float entry;
                if (plane.Raycast(ray, out entry))
                {
                    transform.position += dragStartPosition - ray.GetPoint(entry);
                }
            }
        }

        private void HandleScroll()
        {
            if (Input.mouseScrollDelta.y != 0)
            {
                // subtract because controls are inverted
                float size = Camera.main.orthographicSize - Input.mouseScrollDelta.y;
                Camera.main.orthographicSize = Mathf.Clamp(size, 1, maxCameraSize);
            }
        }
    }
}
