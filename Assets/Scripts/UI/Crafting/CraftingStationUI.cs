using System;
using UnityEngine;

namespace FrontierIsland
{
    public class CraftingStationUI : CraftingUI
    {
        [NonSerialized]
        public Transform focus;
        public float offset = 0.75f;

        public override bool Active 
        { 
            get => base.Active;
            set 
            {
                base.Active = value;

                // have to be called in the same frame as when enabled.
                // see note above UpdatePosition()
                UpdatePosition();
            }
        }

        public void Init(ICraftingStation station)
        {
            Init(station.Title, station.Recipes, station.Inventory);
            focus = station.transform;
            onCraft = station.Craft;
        }

        protected override void Update()
        {
            base.Update();
            UpdatePosition();
        }

        /*
         * NOTE: Although this UI will update its position each frame
         * when it is first enabled it will wait until next frame to
         * update position. This allows the viewer to see the UI at the
         * wrong position for a split frame. Solve this by updating 
         * position in the same frame as when this UI is enabled
         */
        public void UpdatePosition()
        {
            Vector3 position = Camera.main.WorldToScreenPoint(focus.position + new Vector3(0, offset, 0));
            rectTransform.position = new Vector3(position.x, position.y, rectTransform.position.z);
        }
    }
}
