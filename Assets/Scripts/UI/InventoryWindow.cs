using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace FrontierIsland
{
    public class InventoryWindow : MonoBehaviour
    {
        [SerializeField]
        private InventoryItem invItemPrefab;
        public InventoryItem InvItemPrefab { get { return invItemPrefab; } }

        [SerializeField]
        private ItemSlot itemSlotPrefab;
        [SerializeField]
        private RectTransform windowRect;
        public RectTransform WindowRect { get { return windowRect; } }

        public CustomGridLayoutGroup gridLayout;
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
                inventory = value;
                LoadInventory(inventory);
            }
        }

        public virtual bool Active
        {
            get { return gameObject.activeSelf;}
            set
            {
                gameObject.SetActive(value);
                LoadInventory(Inventory);
                LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
            }
        }

        protected virtual void LoadInventory(Inventory inventory)
        {
            if (inventory == null)
            {
                Debug.LogWarning($"Inventory set null");
                gameObject.SetActive(false);
            }
            else if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }

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
            UpdateContent();
        }

        public void UpdateContent()
        {
            int index = 0;
            foreach (ItemSlot itemSlot in inventorySlots)
            {
                if (Inventory[index] != null)
                {
                    if (itemSlot.inventoryItem == null)
                    {
                        InventoryItem item = Instantiate(invItemPrefab, itemSlot.ItemHolder);
                        item.ItemSlot = itemSlot;
                        itemSlot.inventoryItem = item;
                    }
                    itemSlot.inventoryItem.UpdateContent();
                }
                else
                {
                    if (itemSlot.inventoryItem != null)
                    {
                        Destroy(itemSlot.inventoryItem.gameObject);
                    }
                }
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
    }
}
