using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using static UnityEngine.EventSystems.PointerEventData;

namespace FrontierIsland
{
    public class InventoryUI : MonoBehaviour, IDropHandler
    {
        [SerializeField]
        private InventoryItem invItemPrefab;
        public InventoryItem InvItemPrefab { get { return invItemPrefab; } }

        [SerializeField]
        private ItemSlot itemSlotPrefab;
        [SerializeField]
        protected CustomGridLayoutGroup gridLayout;
        protected List<ItemSlot> inventorySlots = new List<ItemSlot>();

        public ItemSlot this[int slotIndex]
        {
            get { return inventorySlots[slotIndex]; }
        }

        public Canvas Canvas { get; private set; }
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
            get { return gameObject.activeSelf;}
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

        protected virtual void LoadInventory(Inventory inventory)
        {

            if (gridLayout.constraint == CustomGridLayoutGroup.Constraint.FixedColumnCount)
            {
                gridLayout.constraintCount = inventory.Columns;
            }
            else
            {
                gridLayout.constraintCount = inventory.Rows;
            }

            // for adding slots
            for (int i = inventorySlots.Count; i < inventory.Capacity; i = inventorySlots.Count)
            {
                AddInventorySlot();
            }


            // for removing slots
            for (int i = inventorySlots.Count; i > inventory.Capacity; i = inventorySlots.Count)
            {
                RemoveInventorySlot();
            }

            UpdateContent();
        }

        protected virtual void Awake()
        {
            Canvas = GetComponentInParent<Canvas>();
        }

        protected virtual void Update()
        {
            //UpdateContent();
        }

        protected virtual void OnDestroy()
        {
            if (Inventory != null)
            {
                Inventory inv = Inventory;
                Inventory.onInventoryChange -= UpdateContent;
                Inventory = null;
            }
        }

        public virtual void UpdateContent()
        {
            int index = 0;
            foreach (ItemSlot itemSlot in inventorySlots)
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

        protected void AddInventorySlot()
        {
            ItemSlot slot = Instantiate(itemSlotPrefab, gridLayout.gameObject.transform);
            slot.Window = this;
            slot.SlotIndex = inventorySlots.Count;
            inventorySlots.Add(slot);
        }

        protected void RemoveInventorySlot()
        {
            ItemSlot slot = inventorySlots[inventorySlots.Count - 1];
            inventorySlots.RemoveAt(inventorySlots.Count - 1);
            Destroy(slot.gameObject);
        }

        /// <summary>
        /// Called when an <see cref="ItemSlot"/> UI is clicked
        /// </summary>
        /// <param name="slotIndex"></param>
        public void SlotClicked(int slotIndex, PointerEventData eventData)
        {
            itemSlotClick?.Invoke(slotIndex, eventData);
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (eventData.button != InputButton.Left) return;
            if (eventData.pointerDrag != null)
            {
                if (eventData.pointerDrag.TryGetComponent(out InventoryItem other))
                {
                    // dropped on inventory window is InventoryItem,
                    // check if it is from a slot from this inventory.
                    if (ReferenceEquals(other.PrevSlot.Window, this))
                    {
                        // put InventoryItem back if it is empty
                        if (other.PrevSlot.InventoryItem == null)
                        {
                            Inventory.SetItem(other.PrevSlot.SlotIndex, other.ItemStack);
                            other.ItemSlot = other.PrevSlot;
                            other.PrevSlot.InventoryItem = other;
                        }
                        else
                        {
                            // otherwise add it to inventory, remaining items get dropped 
                            Inventory.AddItem(other.ItemStack);
                        }
                    }
                    else
                    {
                        // otherwise add it to inventory, remaining items get dropped 
                        Inventory.AddItem(other.ItemStack);
                    }
                }
            }
        }
    }
}
