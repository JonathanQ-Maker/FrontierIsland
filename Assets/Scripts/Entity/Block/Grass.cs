namespace FrontierIsland
{
    public class Grass : Foliage
    {
        public override BlockType BlockType
        {
            get { return BlockType.Grass; }
        }

        public override ItemStack[] GetItemDrops()
        {
            return new ItemStack[] { new GrassItem(1) };
        }
    }
}