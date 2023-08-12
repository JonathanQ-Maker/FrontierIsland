namespace FrontierIsland
{
    public class Mushrooms : Foliage
    {
        public override BlockType BlockType
        {
            get { return BlockType.Mushrooms; }
        }

        public override ItemStack[] GetItemDrops()
        {
            return new ItemStack[] { new MushroomsItem(1) };
        }
    }
}
