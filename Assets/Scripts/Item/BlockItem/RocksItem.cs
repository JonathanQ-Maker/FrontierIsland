namespace FrontierIsland
{
    public class RocksItem : BlockItem
    {
        public override ItemType ItemType { get { return ItemType.Rocks; } }

        public override BlockType BlockType { get { return BlockType.Rocks; } }

        public RocksItem(int count) : base(count, "Rocks", "a collection of rocks")
        {

        }

        public override ItemStack DeepClone()
        {
            RocksItem clone = new RocksItem(count);
            CopyTo(clone);
            return clone;
        }
    }
}
