using UnityEngine;

namespace FrontierIsland
{
    public class InventoryUI : ContainerUI
    {
        [SerializeField]
        protected CustomGridLayoutGroup gridLayout;

        protected override void LoadInventory(Inventory inventory)
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
            for (int i = Count; i < inventory.Capacity; i = Count)
            {
                AddItemSlot(gridLayout.gameObject.transform);
            }


            // for removing slots
            for (int i = Count; i > inventory.Capacity; i = Count)
            {
                RemoveInventorySlot();
            }

            UpdateContent();
        }
    }
}
