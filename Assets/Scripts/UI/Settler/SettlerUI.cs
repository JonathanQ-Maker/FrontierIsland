using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FrontierIsland
{
    public class SettlerUI : WorldUI
    {
        [SerializeField]
        private CraftingUI craftingUI;

        [SerializeField]
        private Transform menu;
        [SerializeField]
        private TextMeshProUGUI menuTitle;

        [SerializeField]
        private Slider healthSlider;

        public int HealthDisplay 
        { 
            get { return (int)healthSlider.value; }
            set { healthSlider.value = value; } 
        }


        public void Init(string name, ItemRecipe[] recipes, Inventory craftingInv, 
            Transform focus, 
            int health, 
            int maxHealth, 
            CraftingUI.OnCraft onCraft)
        {
            menuTitle.text = name;
            craftingUI.Init(name, recipes, craftingInv);
            this.focus = focus;
            healthSlider.maxValue = maxHealth;
            HealthDisplay = health;
            craftingUI.onCraft = onCraft;
        }

        public void OnClickCrafting()
        {
            menu.gameObject.SetActive(false);
            craftingUI.Active = true;
        }


        protected void Start()
        {
            craftingUI.Active = false;
        }
    }
}
