namespace FrontierIsland
{
    public class WoodenBarrelItem : BlockItem
    {
        public WoodenBarrelItem(int count) : base(count, "Bin", "an open roof box")
        {

        }

        public override BlockType BlockType { get { return BlockType.WoodenBarrel; } }

        public override ItemType ItemType { get { return ItemType.WoodenBarrel; } }

        public override ItemStack DeepClone()
        {
            WoodenBarrelItem clone = new WoodenBarrelItem(count);
            CopyTo(clone);
            return clone;
        }
    }
}