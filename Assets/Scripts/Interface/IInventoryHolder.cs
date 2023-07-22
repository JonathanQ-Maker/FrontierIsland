namespace FrontierIsland
{
    public interface IInventoryHolder
    {
        Inventory Inventory { get; }

        /// <summary>
        /// Called if an item stack in holder's inventory is set
        /// 
        /// <br>
        /// NOTE: Is called after set is complete
        /// </br>
        /// </summary>
        /// <param name="newStack"></param>
        /// <param name="oldStack"></param>
        /// <param name="index"></param>
        void OnSetItem(ItemStack newStack, ItemStack oldStack, int index);

        /// <summary>
        /// called if holder should drop the item at index
        /// </summary>
        /// <param name="index"></param>
        void DropItem(int index);
    }
}