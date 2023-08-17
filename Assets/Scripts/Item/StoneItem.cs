namespace FrontierIsland
{
    public class StoneItem : ItemStack
    {
        public override ItemType ItemType { get { return ItemType.Stone; } }

        public StoneItem(int count) : base(count, "Stone", "small and hard")
        {

        }

        public override ItemStack DeepClone()
        {
            StoneItem clone = new StoneItem(count);
            CopyTo(clone);
            return clone;
        }
    }
}
