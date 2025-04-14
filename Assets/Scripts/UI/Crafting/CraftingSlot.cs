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

        private int requiredCount;
        public int RequiredCount
        {
            get { return requiredCount; }
            set 
            {
                requiredCount = Mathf.Max(value, 0);
                UpdateContent();
            } 
        }

        public override InventoryItem InventoryItem 
        { 
            get => base.InventoryItem;
            set 
            {
                base.InventoryItem = value;
            } 
        }

        public override void UpdateContent()
        {
            hasHintItem = requiredCount > 0;
            base.UpdateContent();

            // set text color
            if (requiredCount > 0)
            {
                if (InventoryItem != null)
                {
                    if (InventoryItem.SlotItemStack.ItemType == HintItemType)
                    {
                        if (InventoryItem.SlotItemStack.count >= RequiredCount)
                        {
                            CountDisplay.text = $"<color=#528D1C>{InventoryItem.SlotItemStack.count}/{RequiredCount}</color>";
                        }
                        else
                        {
                            CountDisplay.text = $"<color=#9F0909>{InventoryItem.SlotItemStack.count}/{RequiredCount}</color>";
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

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            if (InventoryItem == null && RequiredCount > 0)
            {
                GameController.Instance.ToolTipWindow.Active = true;
                GameController.Instance.ToolTipWindow.LoadItemTip(ItemAtlas.Get(HintItemType));
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

        protected override void OnDestroy()
        {
            base.OnDestroy();
            GameController.Instance.ToolTipWindow.Active = false;
        }

        private void OnDisable()
        {
            GameController.Instance.ToolTipWindow.Active = false;
        }
    }
}
