using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FrontierIsland
{
    public class CraftingSlot : ItemSlot
    {
        [SerializeField]
        private TextMeshProUGUI countDisplay;
        public TextMeshProUGUI CountDisplay { get { return countDisplay; } }

        [SerializeField]
        private Image requiredItemImage;

        public ItemType RequiredItem { get; set; }

        private int requiredCount;
        public int RequiredCount
        {
            get { return requiredCount; }
            set 
            {
                requiredCount = Mathf.Max(value, 0);
                UpdateDisplay();
            } 
        }

        public override InventoryItem InventoryItem 
        { 
            get => base.InventoryItem;
            set 
            {
                base.InventoryItem = value;
                UpdateDisplay();
            } 
        }

        public void UpdateDisplay()
        {
            if (InventoryItem == null)
            {
                if (requiredCount > 0)
                {
                    requiredItemImage.gameObject.SetActive(true);
                    requiredItemImage.sprite = GameController.Instance.ItemIcons[RequiredItem];
                }
                else
                {
                    requiredItemImage.gameObject.SetActive(false);
                }
            }
            else
            {
                requiredItemImage.gameObject.SetActive(false);
            }

            // set text color
            if (requiredCount > 0)
            {
                if (InventoryItem != null)
                {
                    if (InventoryItem.ItemStack.ItemType == RequiredItem)
                    {
                        if (InventoryItem.ItemStack.count >= RequiredCount)
                        {
                            CountDisplay.text = $"<color=#528D1C>{InventoryItem.ItemStack.count}/{RequiredCount}</color>";
                        }
                        else
                        {
                            CountDisplay.text = $"<color=#9F0909>{InventoryItem.ItemStack.count}/{RequiredCount}</color>";
                        }
                    }
                    else
                    {
                        CountDisplay.text = $"<color=#9F0909>0/{RequiredCount}</color>";
                    }
                }
                else
                {
                    CountDisplay.text = $"<color=#9F0909>0/{RequiredCount}</color>";
                }
            }
            else
            {
                CountDisplay.text = $"<color=#3D4045>0/0</color>";
            }
        }

        public override void OnItemChange()
        {
            UpdateDisplay();
        }

        public override void OnDrop(PointerEventData eventData)
        {
            base.OnDrop(eventData);
            UpdateDisplay();
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            if (InventoryItem == null && RequiredCount > 0)
            {
                GameController.Instance.ToolTipWindow.Active = true;
                GameController.Instance.ToolTipWindow.LoadItemTip(ItemAtlas.Get(RequiredItem));
            }
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            GameController.Instance.ToolTipWindow.Active = false;
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
        }

        private void OnDestroy()
        {
            GameController.Instance.ToolTipWindow.Active = false;
        }

        private void OnDisable()
        {
            GameController.Instance.ToolTipWindow.Active = false;
        }
    }
}
