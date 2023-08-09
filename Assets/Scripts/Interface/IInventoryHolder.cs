namespace FrontierIsland
{
    public interface IInventoryHolder
    {
        Inventory Inventory { get; }

        /// <summary>
        /// Called if holder should drop 
        /// </summary>
        /// <param name="index"></param>
        void DropItem(ItemStack itemStack);
    }
}