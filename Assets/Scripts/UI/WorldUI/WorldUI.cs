using System;
using UnityEngine;

namespace FrontierIsland
{
    public class WorldUI : MonoBehaviour
    {
        [NonSerialized]
        public Transform focus;

        public float offset = 0.75f;

        public RectTransform rectTransform { get { return transform as RectTransform; } }

        public bool Active
        {
            get { return gameObject.activeSelf; }
            set 
            {
                gameObject.SetActive(value);

                // have to be called in the same frame as when enabled.
                // see note above UpdatePosition()
                UpdatePosition();
            }
        }

        private void Update()
        {
            UpdatePosition();
        }


        /*
         * NOTE: Although this UI will update its position each frame
         * when it is first enabled it will wait until next frame to
         * update position. This allows the viewer to see the UI at the
         * wrong position for a split second. Solve this by updating 
         * position in the same frame as when this UI is enabled
         */

        public void UpdatePosition()
        {
            Vector3 position = Camera.main.WorldToScreenPoint(focus.position + new Vector3(0, offset, 0));
            rectTransform.position = new Vector3(position.x, position.y, rectTransform.position.z);
        }
    }
}
