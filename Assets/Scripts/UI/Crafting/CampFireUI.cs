using UnityEngine;
using UnityEngine.UI;

namespace FrontierIsland
{
    public class CampFireUI : WorldUI
    {
        [SerializeField]
        private CampFireMenu menu;
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
            menu.Inventory = inventory;
        }



        public void OnClickAddFuel()
        {
            onAddFuel();
        }
    }
}
