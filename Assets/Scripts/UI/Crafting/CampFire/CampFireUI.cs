using UnityEngine;
using UnityEngine.UI;

namespace FrontierIsland
{
    public class CampFireUI : WorldUI
    {
        [SerializeField]
        private ContainerUI menu;
        [SerializeField]
        private Slider fuelSlider, progressSlider;

        public delegate void OnAddFuel();
        public OnAddFuel onAddFuel;

        public float FuelDisplay 
        { 
            get { return fuelSlider.value; }
            set { fuelSlider.value = value; }
        }

        public float ProgressDisplay
        {
            get { return progressSlider.value; }
            set { progressSlider.value = value; }
        }

        public void Init(OnAddFuel onAddFuel, Transform focus, Inventory inventory)
        {
            this.onAddFuel = onAddFuel;
            this.focus = focus;

            if (inventory.Columns != 3 || inventory.Capacity != 3)
            {
                throw new System.Exception($"{GetType()} can only load inventory with 3 slots");
            }
            menu.Inventory = inventory;
        }



        public void OnClickAddFuel()
        {
            onAddFuel();
        }
    }
}
