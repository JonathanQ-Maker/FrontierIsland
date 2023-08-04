namespace FrontierIsland
{
    public class CarpenterBenchItem : BlockItem
    {
        public CarpenterBenchItem(int count) : base(count, "Carpenter Bench", "makes wood stuff")
        {

        }

        public override BlockType BlockType { get { return BlockType.CarpenterBench; } }

        public override ItemType ItemType { get { return ItemType.CarpenterBench; } }

        public override ItemStack DeepClone()
        {
            CarpenterBenchItem item = new CarpenterBenchItem(count);
            CopyTo(item);
            return item;
        }
    }
}
