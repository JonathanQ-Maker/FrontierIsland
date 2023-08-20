namespace FrontierIsland
{
    public class TreeSap : ItemStack
    {
        public TreeSap(int count) : base(count, "Tree Sap", "Blood of a tree")
        {

        }

        public override ItemType ItemType { get { return ItemType.TreeSap; } }

        public override ItemStack DeepClone()
        {
            TreeSap clone = new TreeSap(count);
            CopyTo(clone);
            return clone;
        }
    }
}
