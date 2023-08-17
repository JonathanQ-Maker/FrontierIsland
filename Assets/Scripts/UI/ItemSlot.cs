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

        [SerializeField] // for debug
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

        public virtual void UpdateContent()
        { 
            InventoryItem?.UpdateContent();
        }

        public virtual void OnDrop(PointerEventData eventData)
        {
            /*
             * When an InventoryItem is dropped on this ItemSlot UI
             * the ItemSlot must handle the ItemStack stored in the InventoryItem.
             * In general, the ItemStack should be moved to the Inventory slot this 
             * ItemSlot UI represents.
             * 
             * NOTE: Any unhandled/leftover ItemStack in InventoryItem after OnDrop() exits 
             * will be dropped through the InventoryHolder
             * 
             * NOTE: Inventory moddification functions are called after the bidirectional
             * accociation of ItemStack and InventoryItem are updated.
             * This is because the InventoryUI is subscribed 
             * to the Inventory.onInventoryChange delegate with InventoryUI.UpdateContent()
             * which will update all visual UI components and assumes that the bidirectional
             * accociation is correct.
             */
            if (eventData.button != InputButton.Left) return;
            if (eventData.pointerDrag != null)
            {
                if (eventData.pointerDrag.TryGetComponent(out InventoryItem other))
                {

                    if (InventoryItem == null)
                    {
                        // empty slot
                        ItemStack otherStack = other.ItemStack;
                        other.ItemSlot = this;
                        InventoryItem = other;
                        Window.Inventory.SetItem(SlotIndex, otherStack);
                    }
                    else if (InventoryItem.ItemStack.Similar(other.ItemStack))
                    {
                        // slot with similar items
                        InventoryItem.ItemStack.CombineStack(other.ItemStack);
                        //Window.Inventory.onInventoryChange?.Invoke();
                        if (other.ItemStack.count > 0)
                        {
                            // still have left overs
                            if (other.PrevSlot.ItemStack == null)
                            {
                                // previous slot is empty, put item back
                                ItemSlot prevSlot = other.PrevSlot;
                                ItemStack otherStack = other.ItemStack;
                                other.ItemSlot = prevSlot;
                                prevSlot.InventoryItem = other;
                                prevSlot.Window.Inventory.SetItem(prevSlot.SlotIndex, otherStack);
                            }
                            else
                            {
                                // previous slot is occupied, add to origin inventory.
                                // Any left overs is dropped
                                other.PrevSlot.Window.Inventory.AddItem(other.ItemStack);
                            }
                        }
                    }
                    else if (other.PrevSlot.ItemStack == null)
                    {
                        // different items and our previous slot is empty, swap it
                        ItemStack thisStack = InventoryItem.ItemStack;
                        ItemStack otherStack = other.ItemStack;

                        InventoryItem.ItemSlot = other.PrevSlot;
                        InventoryItem.ItemSlot.InventoryItem = InventoryItem;
                        InventoryItem.ResetPosition();
                        other.PrevSlot.Window.Inventory.SetItem(other.PrevSlot.SlotIndex, thisStack);

                        other.ItemSlot = this;
                        InventoryItem = other;
                        Window.Inventory.SetItem(SlotIndex, otherStack);
                    }
                    else if (other.PrevSlot.ItemStack.Similar(other.ItemStack))
                    {
                        // different items, previous slot is not empty
                        // and previous item is similar, combine them
                        other.PrevSlot.ItemStack.CombineStack(other.ItemStack);
                        //Window.Inventory.onInventoryChange?.Invoke();
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

        protected virtual void OnDestroy()
        {
            window = null;
        }
    }
}
