using UnityEngine;
using UnityEngine.UI;

namespace FrontierIsland
{
    public class WoodenBarrelUI : WorldUI
    {
        [SerializeField]
        private ContainerUI menu;
        [SerializeField]
        private Slider fluidSlider, progressSlider;
        [SerializeField]
        private Image fluidBar;

        public float FluidDisplay
        {
            get { return fluidSlider.value; }
            set { fluidSlider.value = value; }
        }

        public float ProgressDisplay
        {
            get { return progressSlider.value; }
            set { progressSlider.value = value; }
        }

        private FluidType fluidType;
        public FluidType FluidType
        {
            get { return fluidType; }
            set 
            {
                fluidType = value;
                fluidBar.material = GameController.Instance.FluidMaterials.GetUIMaterial(fluidType);
            }
        }



        public void Init(Transform focus, Inventory inventory)
        {
            this.focus = focus;

            if (inventory.Columns != 3 || inventory.Capacity != 3)
            {
                throw new System.Exception($"{GetType()} can only load inventory with 3 slots");
            }
            menu.Inventory = inventory;
        }
    }
}
