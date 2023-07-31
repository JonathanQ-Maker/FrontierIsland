using System;
using UnityEngine;
using UnityEngine.UI;

namespace FrontierIsland
{
    public class HotBarUI : InventoryUI
    {
        [SerializeField]
        private Image selectionSquare;

        private int selectionIndex = 0;
        public int SelectionIndex
        {
            get { return selectionIndex; }
            set 
            { 
                selectionIndex = value;
                UpdateSelectionVisual();
            }
        }

        protected override void Awake()
        {
            base.Awake();
            if (gridLayout.constraint != CustomGridLayoutGroup.Constraint.FixedRowCount || gridLayout.constraintCount != 1)
            {
                throw new FormatException("Hotbar is not one row");
            }
        }

        protected override void LoadInventory(Inventory inventory)
        {
            if (inventory == null) return;

            // for adding slots
            for (int i = inventorySlots.Count; i < inventory.Columns; i = inventorySlots.Count)
            {
                AddInventorySlot();
            }


            // for removing slots
            for (int i = inventorySlots.Count; i > inventory.Columns; i = inventorySlots.Count)
            {
                RemoveInventorySlot();
            }


            UpdateContent();

            UpdateSelectionVisual();
        }

        protected void UpdateSelectionVisual()
        {
            inventorySlots[SelectionIndex].PlaceOverlay(selectionSquare.rectTransform);
        }
    }
}
