namespace FrontierIsland
{
    public class Bin : Block
    {
        // TODO: make functional
        public override MaterialType MaterialType
        {
            get
            {
                return MaterialType.Wood;
            }
        }

        public override BlockType BlockType
        {
            get { return BlockType.Bin; }
        }

        public override ItemStack[] GetItemDrops()
        {
            return new ItemStack[] { new BinItem(1) };
        }
    }
}