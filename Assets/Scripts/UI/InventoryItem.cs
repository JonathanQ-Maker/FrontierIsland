using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using static UnityEngine.EventSystems.PointerEventData;
using System;

namespace FrontierIsland
{
    public class InventoryItem : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        private Image image;
        [SerializeField]
        private TextMeshProUGUI countDisplay;

        [NonSerialized]
        public InventoryWindow window;

        [SerializeField]
        private int slotIndex = -1;
        
        /// <summary>
        /// The index that gets the ItemStack this InventoryItem represents
        /// </summary>
        public int SlotIndex
        {
            get
            {
                return slotIndex;
            }

            set
            {
                slotIndex = value;
                if (slotIndex != -1)
                { 
                    UpdateContent();
                    ResetPosition();
                }
            }
        }

        public ItemStack ItemStack
        {
            get
            {
                return window.Inventory[SlotIndex];
            }
        }

        private Canvas canvas;

        private void Start()
        {
            Canvas[] canvases = GetComponentsInParent<Canvas>();
            canvas = canvases[canvases.Length - 1]; // topmost canvas
        }

        public void UpdateContent()
        {
            ItemStack item = window.Inventory[SlotIndex];
            image.sprite = item.GetIcon();
            if (item.count == 1)
            {
                countDisplay.text = string.Empty;
            }
            else
            {
                countDisplay.text = $"{item.count}";
            }
        }

        public void ResetPosition()
        {
            transform.SetParent(null);
            transform.SetParent(window[SlotIndex].ItemHolder);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            transform.SetParent(canvas.transform);
            transform.SetAsLastSibling();
            image.raycastTarget = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (eventData.button == InputButton.Left)
            {
                transform.position = eventData.position;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!RectTransformUtility.RectangleContainsScreenPoint(window.WindowRect, Input.mousePosition))
            {
                if (window.Inventory.Holder != null)
                {
                    window.Inventory.Holder.DropItem(SlotIndex);
                }
                else
                {
                    window.Inventory.RemoveStack(SlotIndex);
                }
                Destroy(gameObject);
            }
            else
            {
                ResetPosition();
            }
            image.raycastTarget = true;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            GameController.Instance.ToolTipWindow.Active = true;
            GameController.Instance.ToolTipWindow.LoadItemTip(ItemStack);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            GameController.Instance.ToolTipWindow.Active = false;
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
