using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FrontierIsland
{
    public class CraftingStationUI : InventoryUI
    {
        [SerializeField]
        private CraftingStationItem stationItemPrefab;
        [SerializeField]
        private TextMeshProUGUI title, countDisplay;
        [SerializeField]
        private RectTransform selectOverlay, recipeContent;
        [SerializeField]
        private Slider countSlider;

        [NonSerialized]
        public Transform focus;

        public float offset = 0.75f;

        public RectTransform rectTransform { get { return transform as RectTransform; } }

        private ICraftingStation station;
        public ICraftingStation Station 
        {
            get { return station; }
            set
            {
                station = value;
                focus = station.transform;
            }
        }

        private CraftingStationItem[] stationItems;

        private int selectIndex;
        public int SelectIndex 
        { 
            get { return selectIndex; }
            set 
            { 
                selectIndex = value;
                UpdateSelectRecipe();
            }
        }

        public int Count { get { return (int)countSlider.value; } }

        public const int CAMERA_CONTROL_MASK = 1 << 1;

        private void Start()
        {
            Render();
            UpdateSelectRecipe();
        }

        protected override void Update()
        {
            UpdatePosition();
            base.Update();
        }

        public void UpdateSelectRecipe()
        {
            if (stationItems == null) return;
            ItemRecipe selectedRecipe = station.Recipes[SelectIndex];
            selectOverlay.SetParent(stationItems[SelectIndex].transform, false);

            for (int i = 0; i < inventorySlots.Count; ++i)
            {
                // Clear required item
                SetRequiredItem(i, ItemType.Tree, 0);
            }

            int maxCount = ItemAtlas.Get(selectedRecipe.Ingredients[0].item).MaxStackSize / selectedRecipe.Ingredients[0].count;
            for (int i = 1; i < selectedRecipe.Ingredients.Length; ++i)
            { 
                int current = ItemAtlas.Get(selectedRecipe.Ingredients[i].item).MaxStackSize / selectedRecipe.Ingredients[i].count;
                if (current < maxCount)
                {
                    maxCount = current;
                }
            }
            maxCount = Mathf.Min(maxCount, ItemAtlas.Get(selectedRecipe.ResultItem).MaxStackSize);
            
            // if maxCount 1 hide slider, otherwise show slider
            if (maxCount == 1)
            {
                countSlider.gameObject.SetActive(false);
                countDisplay.gameObject.SetActive(false);
                countSlider.value = 1;
            }
            else
            {
                countSlider.gameObject.SetActive(true);
                countDisplay.gameObject.SetActive(true);
                countSlider.maxValue = maxCount;
            }
            UpdateRequiredItems(1);
        }

        /// <summary>
        /// Renders Required Crafting Slot
        /// </summary>
        /// <param name="count">number of result items</param>
        private void UpdateRequiredItems(int count)
        {
            Ingredient[] ingredients = station.Recipes[SelectIndex].Ingredients;
            for (int i = 0; i < ingredients.Length; ++i)
            {
                SetRequiredItem(i, ingredients[i].item, ingredients[i].count * count);
            }
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


        /// <summary>
        /// Renders the <see cref="ICraftingStation"/>
        /// </summary>
        private void Render()
        {
            if (stationItems != null) ClearStationItems();
            title.text = station.Title;

            if (stationItems == null || stationItems.Length != station.Recipes.Length)
            {
                stationItems = new CraftingStationItem[station.Recipes.Length];
            }

            for (int i = 0; i < station.Recipes.Length; ++i)
            {
                CraftingStationItem stationItem = Instantiate(stationItemPrefab,
                                                      Vector3.zero,
                                                      Quaternion.identity,
                                                      recipeContent);
                stationItem.stationUI = this;
                stationItem.Load(i);
                stationItems[i] = stationItem;
            }

            selectOverlay.gameObject.SetActive(stationItems.Length > 0);
        }

        private void ClearStationItems()
        {
            if (stationItems == null) return;

            for (int i = 0; i < stationItems.Length; ++i)
            {
                Destroy(stationItems[i].gameObject);
            }
        }

        private void SetRequiredItem(int slotIndex, ItemType item, int count)
        {
            CraftingSlot slot = (CraftingSlot)this[slotIndex];
            slot.RequiredItem = item;
            slot.RequiredCount = count;
        }

        public void OnClickCraft()
        {
            station.Assemble(SelectIndex, Count);
        }

        public void OnCountChange()
        {
            countDisplay.text = $"{Count}";
            UpdateRequiredItems(Count);
        }
    }
}
