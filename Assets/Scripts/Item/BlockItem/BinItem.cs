namespace FrontierIsland
{
    public class BinItem : BlockItem
    {
        public BinItem(int count) : base(count, "Bin", "an open roof box")
        {

        }

        public override BlockType BlockType { get { return BlockType.Bin; } }

        public override ItemType ItemType { get { return ItemType.Bin; } }

        public override ItemStack DeepClone()
        {
            BinItem clone = new BinItem(count);
            CopyTo(clone);
            return clone;
        }
    }
}