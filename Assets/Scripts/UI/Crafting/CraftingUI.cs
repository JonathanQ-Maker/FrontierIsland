using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FrontierIsland
{
    public class CraftingUI : InventoryUI
    {
        [SerializeField]
        private ItemSelectionNode selectionNodePrefab;
        [SerializeField]
        private TextMeshProUGUI title, countDisplay;
        [SerializeField]
        private RectTransform selectOverlay, recipeContent;
        [SerializeField]
        private Slider countSlider;

        public RectTransform rectTransform { get { return transform as RectTransform; } }

        private ItemSelectionNode[] selectionNodes;

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

        public int CraftCount { get { return (int)countSlider.value; } }

        public const int CAMERA_CONTROL_MASK = 1 << 1;

        private ItemRecipe[] itemRecipes;

        public delegate void OnCraft(int selectIndex, int count);
        public OnCraft onCraft;

        DebugTracker tracker;
        protected override void Start()
        {
            base.Start();
            if (ItemSlotPrefab is not CraftingSlot)
            {
                throw new Exception("ItemSlot is not CraftingSlot");
            }
            tracker = new DebugTracker("CraftingUI");
            UpdateSelectRecipe();
        }

        /// <summary>
        /// <br>
        /// Updates the UI for the selected recipe
        /// </br>
        /// <br>
        /// NOTE: can only be called after Load()
        /// </br>
        /// </summary>
        public void UpdateSelectRecipe()
        {
            if (selectionNodes == null) return; // Load() will fill selectionNodes
            ItemRecipe selectedRecipe = itemRecipes[SelectIndex];
            selectOverlay.SetParent(selectionNodes[SelectIndex].transform, false);

            for (int i = 0; i < Count; ++i)
            {
                // Clear required item by setting 0
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
            Ingredient[] ingredients = itemRecipes[SelectIndex].Ingredients;
            for (int i = 0; i < ingredients.Length; ++i)
            {
                SetRequiredItem(i, ingredients[i].item, ingredients[i].count * count);
            }
        }

        public void Init(string title, ItemRecipe[] recipes, Inventory inventory)
        {
            Inventory = inventory;
            itemRecipes = recipes;
            if (selectionNodes != null) ClearStationItems();
            this.title.text = title;

            if (selectionNodes == null || selectionNodes.Length != itemRecipes.Length)
            {
                selectionNodes = new ItemSelectionNode[itemRecipes.Length];
            }

            for (int i = 0; i < itemRecipes.Length; ++i)
            {
                ItemSelectionNode itemDisplay = Instantiate(selectionNodePrefab,
                                                      Vector3.zero,
                                                      Quaternion.identity,
                                                      recipeContent);
                itemDisplay.Load(itemRecipes[i].ResultItem, i);
                itemDisplay.onSelect = OnSelect;
                selectionNodes[i] = itemDisplay;
            }

            selectOverlay.gameObject.SetActive(selectionNodes.Length > 0);
        }

        private void OnSelect(int index)
        {
            SelectIndex = index;
        }

        private void ClearStationItems()
        {
            if (selectionNodes == null) return;

            for (int i = 0; i < selectionNodes.Length; ++i)
            {
                selectionNodes[i].onSelect = null;
                Destroy(selectionNodes[i].gameObject);
            }
        }

        private void SetRequiredItem(int slotIndex, ItemType item, int count)
        {
            CraftingSlot slot = (CraftingSlot)this[slotIndex];
            slot.RequiredItem = item;
            slot.RequiredCount = count;
        }

        public virtual void OnClickCraft()
        {
            // did not use onCraft?.Invoke() because
            // we want it to report error when onCraft
            // is empty.
            onCraft(SelectIndex, CraftCount);
        }

        public void OnCountChange()
        {
            countDisplay.text = $"{CraftCount}";
            UpdateRequiredItems(CraftCount);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            onCraft = null;
        }
    }
}
