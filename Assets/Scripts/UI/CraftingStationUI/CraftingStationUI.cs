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
        private TextMeshProUGUI title;
        [SerializeField]
        private RectTransform selectOverlay, recipeContent;
        [SerializeField]
        private Slider countSldier;

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

            ItemType[] ingredients = selectedRecipe.Ingredients;
            for (int i = 0; i < ingredients.Length; ++i)
            {
                // TODO: get actual count
                SetRequiredItem(i, ingredients[i], UnityEngine.Random.Range(1, 6));
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

        public void UpdateSlider()
        { 
            //TODO: update max count
        }

        public void OnClickCraft()
        {
            Debug.Log("Craft");
        }

        public void OnCountChange()
        {
            Debug.Log($"Slider Value: {countSldier.value}");
        }
    }
}
