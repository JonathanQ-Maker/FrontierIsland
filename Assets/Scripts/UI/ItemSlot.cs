using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.EventSystems.PointerEventData;

namespace FrontierIsland
{
    /// <summary>
    /// Item Slot for handling reciving dropped InventoryItem
    /// 
    /// Prefab Hierarchy:
    ///     ItemSlot (script goes here, slot sprite here, drop hitbox)
    ///         ItemHolder (InventoryItem's parent, assures InventoryItem is 
    ///                     smaller than slot by using VerticalLayoutGroup 
    ///                     and control child width/heigh true, full stretch)
    ///             InventoryItem (script, handles dragging and item icon display, item sprite)
    ///         ItemSlot Overlay (full stretch, overlays of the item slot goes here, e.g. selection square)
    /// </summary>
    public class ItemSlot : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public readonly static Color HoverColor = new Color(0.8f, 0.8f, 0.8f);

        [SerializeField]
        private Image image;
        [SerializeField]
        private RectTransform itemHolder;

        public RectTransform ItemHolder { get { return itemHolder; } }

        private InventoryUI window;
        public InventoryUI Window
        {
            get
            {
                return window;
            }

            set
            {
                if (window == null)
                {
                    window = value;
                    return;
                }
                Debug.LogWarning("Cannot re-assign window");
            }
        }

        private int slotIndex = -1;

        /// <summary>
        /// The index of the slot in inventory this ItemSlot represents
        /// </summary>
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

        public ItemStack ItemStack { get { return Window.Inventory[SlotIndex]; } }


        private InventoryItem inventoryItem = null;

        public virtual InventoryItem InventoryItem 
        {
            get { return inventoryItem; }
            set 
            {
                inventoryItem = value;
            } 
        }

        public virtual void OnItemChange()
        { 
        
        }

        public virtual void OnDrop(PointerEventData eventData)
        {
            if (eventData.button != InputButton.Left) return;
            if (eventData.pointerDrag != null)
            {
                if (eventData.pointerDrag.TryGetComponent(out InventoryItem other))
                {

                    if (InventoryItem == null)
                    {

                        // empty slot
                        Window.Inventory.SetItem(SlotIndex, other.ItemStack);
                        other.ItemSlot = this;
                        InventoryItem = other;
                    }
                    else if (InventoryItem.ItemStack.Similar(other.ItemStack))
                    {
                        // slot with similar items
                        InventoryItem.ItemStack.CombineStack(other.ItemStack);
                        if (other.ItemStack.count > 0)
                        {
                            // still have left overs
                            if (other.PrevSlot.ItemStack == null)
                            {
                                // previous slot is empty, put item back
                                ItemSlot prevSlot = other.PrevSlot;
                                Window.Inventory.SetItem(prevSlot.SlotIndex, other.ItemStack);
                                other.ItemSlot = prevSlot;
                                prevSlot.InventoryItem = other;
                            }
                            else
                            {
                                Window.Inventory.AddItem(other.ItemStack);
                            }
                        }
                    }
                    else if (other.PrevSlot.ItemStack == null)
                    {
                        // different items and our previous slot is empty, swap it
                        other.PrevSlot.Window.Inventory.SetItem(other.PrevSlot.SlotIndex, InventoryItem.ItemStack);
                        Window.Inventory.SetItem(SlotIndex, other.ItemStack);

                        InventoryItem.ItemSlot = other.PrevSlot;
                        InventoryItem.ItemSlot.InventoryItem = InventoryItem;
                        InventoryItem.ResetPosition();

                        other.ItemSlot = this;
                        InventoryItem = other;
                    }
                    else if (other.PrevSlot.ItemStack.Similar(other.ItemStack))
                    {
                        // different items, previous slot is not empty
                        // and previous item is similar, combine them
                        other.PrevSlot.ItemStack.CombineStack(other.ItemStack);
                        if (other.ItemStack.count > 0)
                        {
                            // still have left overs
                            Window.Inventory.AddItem(other.ItemStack);
                        }
                    }
                }
            }
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            image.color = HoverColor;
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            image.color = Color.white;
        }

        // TODO: remove
        public void AssignInventoryItem(InventoryItem inventoryItem)
        {
            this.InventoryItem = inventoryItem;
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

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            window.SlotClicked(SlotIndex, eventData);
        }
    }
}
