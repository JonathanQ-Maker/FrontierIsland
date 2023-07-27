namespace FrontierIsland
{
    public class GrassItem : BlockItem
    {
        public GrassItem(int count) : base(count, "Grass", "nature's fur")
        {

        }

        public override BlockType BlockType { get { return BlockType.Grass; } }

        public override ItemType ItemType { get { return ItemType.Grass; } }

        public override ItemStack DeepClone()
        {
            GrassItem clone = new GrassItem(count);
            CopyTo(clone);
            return clone;
        }
    }
}