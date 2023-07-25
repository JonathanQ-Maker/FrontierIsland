namespace FrontierIsland
{
    public class CrateItem : BlockItem
    {
        public CrateItem(int count) : base(count, "Crate", "Box made of wood planks")
        {

        }

        public override BlockType BlockType { get { return BlockType.Crate; } }

        public override ItemType ItemType { get { return ItemType.Crate; } }

        public override ItemStack DeepClone()
        {
            CrateItem clone = new CrateItem(count);
            CopyTo(clone);
            return clone;
        }
    }
}