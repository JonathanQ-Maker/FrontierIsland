namespace FrontierIsland
{
    public class Grass : Foliage
    {
        public override BlockType BlockType
        {
            get { return BlockType.Grass; }
        }

        public override ItemStack GetItemDrop()
        {
            return new GrassItem(1);
        }
    }
}