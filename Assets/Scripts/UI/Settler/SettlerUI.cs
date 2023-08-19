using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FrontierIsland
{
    public class SettlerUI : MonoBehaviour
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

        [NonSerialized]
        public Transform focus;
        public float offset = 2f;

        public RectTransform rectTransform { get { return transform as RectTransform; } }

        public virtual bool Active
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

        protected virtual void Update()
        {
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

        protected void Start()
        {
            craftingUI.Active = false;
        }
    }
}
