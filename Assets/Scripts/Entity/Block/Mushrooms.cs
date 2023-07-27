namespace FrontierIsland
{
    public class Mushrooms : Foliage
    {
        public override BlockType BlockType
        {
            get { return BlockType.Mushrooms; }
        }

        public override ItemStack GetItemDrop()
        {
            return new MushroomsItem(1);
        }
    }
}
