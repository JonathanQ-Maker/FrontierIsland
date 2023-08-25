namespace FrontierIsland
{
    public class CampFireMenu : ContainerUI
    {
        protected ItemSlot InputSlot { get { return this[0]; } }
        protected ItemSlot OutputSlot { get { return this[1]; } }
        protected ItemSlot FuelSlot { get { return this[2]; } }

        protected override void LoadInventory(Inventory inventory)
        {
            if (inventory.Columns != 3 || inventory.Capacity != 3)
            {
                throw new System.Exception($"{GetType()} can only load inventory with 3 slots");
            }

            base.LoadInventory(inventory);
        }

        DebugTracker tracker;
        protected override void Start()
        {
            base.Start();
            tracker = new DebugTracker("CampFireMenu");
        }
    }
}
