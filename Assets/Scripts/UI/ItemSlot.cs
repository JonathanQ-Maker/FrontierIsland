using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FrontierIsland
{
    /// <summary>
    /// Item Slot for handling reciving dropped InventoryItem
    /// 
    /// Prefab Hierarchy:
    ///     ItemSlot (script goes here, no visuals, drop hitbox)
    ///         ItemHolder (InventoryItem's parent, assures InventoryItem is 
    ///                     smaller than slot by using VerticalLayoutGroup, full stretch, slot sprite here)
    ///             InventoryItem (script, handles dragging and item icon display, item sprite)
    ///         ItemSlot Overlay (full stretch, overlays of the item slot goes here, e.g. selection square)
    /// </summary>
    public class ItemSlot : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public readonly static Color HoverColor = new Color(0.8f, 0.8f, 0.8f);

        [SerializeField]
        private Image image;
        [SerializeField]
        private RectTransform itemHolder;

        public RectTransform ItemHolder { get { return itemHolder; } }

        private InventoryWindow inventoryWindow;
        public InventoryWindow InventoryWindow
        {
            get
            {
                return inventoryWindow;
            }

            set
            {
                if (inventoryWindow == null)
                {
                    inventoryWindow = value;
                    return;
                }
                Debug.LogWarning("Cannot re-assign inventoryWindow");
            }
        }

        private int slotIndex = -1;
        public int SlotIndex
        {
            get
            {
                return slotIndex;
            }

            set
            {
                if (slotIndex == -1)
                {
                    slotIndex = value;
                    return;
                }
                Debug.LogWarning("Cannot re-assign slotIndex");
            }
        }

        [NonSerialized]
        public InventoryItem inventoryItem = null;

        public void OnDrop(PointerEventData eventData)
        {
            if (eventData.pointerDrag != null)
            {
                InventoryItem inventoryItem = eventData.pointerDrag.GetComponent<InventoryItem>();
                if (inventoryItem != null)
                {
                    if (this.inventoryItem == null)
                    {
                        inventoryItem.Slot = this;
                    }
                }
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            image.color = HoverColor;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            image.color = Color.white;
        }

        public void AssignInventoryItem(InventoryItem inventoryItem)
        {
            this.inventoryItem = inventoryItem;
            inventoryItem.transform.SetParent(ItemHolder);
        }

        public void PlaceOverlay(RectTransform overlay)
        {
            overlay.SetParent(transform);
            overlay.anchorMax = new Vector2(1, 1);
            overlay.anchorMin = new Vector2(0, 0);
            overlay.offsetMax = Vector2.zero;
            overlay.offsetMin = Vector2.zero;
        }
    }
}
