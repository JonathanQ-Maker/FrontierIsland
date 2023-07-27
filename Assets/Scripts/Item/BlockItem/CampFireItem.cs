namespace FrontierIsland
{
    public class CampFireItem : BlockItem
    {
        public CampFireItem(int count) : base(count, "Camp Fire", "a collection of wood\r\nwith burning passion")
        {

        }

        public override BlockType BlockType { get { return BlockType.CampFire; } }

        public override ItemType ItemType { get { return ItemType.CampFire; } }

        public override ItemStack DeepClone()
        {
            CampFireItem clone = new CampFireItem(count);
            CopyTo(clone);
            return clone;
        }
    }
}