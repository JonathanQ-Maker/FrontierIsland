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
            base.UpdateContent();
            if (InventoryItem == null)
            {
                if (requiredCount > 0)
                {
                    if (!requiredItemImage.gameObject.activeSelf)
                        requiredItemImage.gameObject.SetActive(true);
                    requiredItemImage.sprite = GameController.Instance.ItemIcons[RequiredItem];
                }
                else if (requiredItemImage.gameObject.activeSelf)
                {
                    requiredItemImage.gameObject.SetActive(false);
                }
            }
            else if (requiredItemImage.gameObject.activeSelf)
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
