namespace FrontierIsland
{
    public class TreeTar : ItemStack
    {
        public override int FuelValue { get { return 25; } }

        public TreeTar(int count) : base(count, "Tree Tar", "Organic adhesive")
        {

        }

        public override ItemType ItemType { get { return ItemType.TreeTar; } }

        public override ItemStack DeepClone()
        {
            TreeTar clone = new TreeTar(count);
            CopyTo(clone);
            return clone;
        }
    }
}
