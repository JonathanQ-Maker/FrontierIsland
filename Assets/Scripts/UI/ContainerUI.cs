using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using static UnityEngine.EventSystems.PointerEventData;

namespace FrontierIsland
{
    public abstract class ContainerUI : MonoBehaviour, IDropHandler
    {
        [SerializeField]
        private InventoryItem invItemPrefab;
        public InventoryItem InvItemPrefab { get { return invItemPrefab; } }

        [SerializeField]
        private ItemSlot itemSlotPrefab;
        public ItemSlot ItemSlotPrefab { get { return itemSlotPrefab; } }

        [SerializeField]
        private List<ItemSlot> itemSlots;

        public ItemSlot this[int slotIndex]
        {
            get { return itemSlots[slotIndex]; }
        }

        public int Count
        {
            get { return itemSlots.Count; }
        }

        private Inventory inventory;
        public Inventory Inventory
        {
            get
            {
                return inventory;
            }

            set
            {
                if (inventory != null)
                {
                    inventory.onInventoryChange -= UpdateContent;
                }
                inventory = value;
                if (inventory != null)
                {
                    inventory.onInventoryChange += UpdateContent;
                    LoadInventory(inventory);
                }
            }
        }

        public virtual bool Active
        {
            get { return gameObject.activeSelf; }
            set
            {
                gameObject.SetActive(value);
                if (Active)
                {
                    LoadInventory(Inventory);
                    LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
                }
            }
        }

        public delegate void ItemSlotClick(int slotIndex, PointerEventData eventData);
        public ItemSlotClick itemSlotClick;

        /// <summary>
        /// <br>
        /// Loads the given Inventory.
        /// </br>
        /// <br>
        /// Also serves as the initalization of <see cref="ContainerUI"/>
        /// </br>
        /// </summary>
        /// <param name="inventory"></param>
        protected virtual void LoadInventory(Inventory inventory)
        {
            UpdateContent();
        }

        /// <summary>
        /// Instantiate new <see cref="ItemSlot"/> as child of <paramref name="parent"/>
        /// </summary>
        /// <param name="parent"></param>
        /// <returns>
        /// <see cref="ItemSlot"/>'s slot index
        /// </returns>
        protected virtual int AddItemSlot(Transform parent)
        {
            ItemSlot slot = Instantiate(ItemSlotPrefab, parent);
            slot.Container = this;
            slot.SlotIndex = itemSlots.Count;
            itemSlots.Add(slot);
            return slot.SlotIndex;
        }

        protected virtual void RemoveInventorySlot()
        {
            ItemSlot slot = itemSlots[itemSlots.Count - 1];
            itemSlots.RemoveAt(itemSlots.Count - 1);
            Destroy(slot.gameObject);
        }

        protected virtual void Start()
        {
            // intentionally left blank
        }

        protected virtual void OnDestroy()
        {
            if (Inventory != null)
            {
                Inventory.onInventoryChange -= UpdateContent;
                Inventory = null;
                itemSlotClick = null;
            }
        }

        public virtual void UpdateContent()
        {
            int index = 0;
            foreach (ItemSlot itemSlot in itemSlots)
            {
                if (Inventory[index] != null)
                {
                    if (itemSlot.InventoryItem == null)
                    {
                        InventoryItem item = Instantiate(invItemPrefab, itemSlot.ItemHolder);
                        item.ItemSlot = itemSlot;
                        itemSlot.InventoryItem = item;
                    }
                }
                else
                {
                    if (itemSlot.InventoryItem != null)
                    {
                        Destroy(itemSlot.InventoryItem.gameObject);
                        itemSlot.InventoryItem = null;
                    }
                }
                itemSlot.UpdateContent();
                index++;
            }
        }

        /// <summary>
        /// Called when an <see cref="ItemSlot"/> UI is clicked
        /// </summary>
        /// <param name="slotIndex"></param>
        public virtual void SlotClicked(int slotIndex, PointerEventData eventData)
        {
            itemSlotClick?.Invoke(slotIndex, eventData);
        }

        public virtual void OnDrop(PointerEventData eventData)
        {
            if (eventData.button != InputButton.Left) return;
            if (eventData.pointerDrag != null)
            {
                if (eventData.pointerDrag.TryGetComponent(out InventoryItem other))
                {
                    // dropped on inventory window is InventoryItem,
                    // check if it is from a slot from this inventory.
                    if (ReferenceEquals(other.PrevSlot.Container, this))
                    {
                        // put InventoryItem back if it is empty
                        if (other.PrevSlot.InventoryItem == null)
                        {
                            ItemStack itemStack = other.ItemStack;
                            other.ItemSlot = other.PrevSlot;
                            other.PrevSlot.InventoryItem = other;
                            Inventory.SetItem(other.ItemSlot.SlotIndex, itemStack);
                        }
                    }
                }
            }
        }
    }
}
