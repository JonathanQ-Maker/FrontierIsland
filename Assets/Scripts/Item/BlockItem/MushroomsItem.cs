namespace FrontierIsland
{
    public class MushroomsItem : BlockItem
    {
        public MushroomsItem(int count) : base(count, "Mushrooms", "like grass but not")
        {

        }

        public override BlockType BlockType { get { return BlockType.Mushrooms; } }

        public override ItemType ItemType { get { return ItemType.Mushrooms; } }

        public override ItemStack DeepClone()
        {
            MushroomsItem clone = new MushroomsItem(count);
            CopyTo(clone);
            return clone;
        }
    }
}