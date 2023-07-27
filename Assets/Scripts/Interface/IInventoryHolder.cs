namespace FrontierIsland
{
    public interface IInventoryHolder
    {
        Inventory Inventory { get; }

        /// <summary>
        /// Called if an item stack in holder's inventory is changed
        /// <br>
        /// NOTE: Is called after set is complete
        /// </br>
        /// </summary>
        /// <param name="index"></param>
        void OnInventoryChange(int index);

        /// <summary>
        /// Called if holder should drop 
        /// </summary>
        /// <param name="index"></param>
        void DropItem(ItemStack itemStack);
    }
}