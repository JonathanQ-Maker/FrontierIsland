namespace FrontierIsland
{
    public class Bin : Block
    {
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

        public override ItemStack GetItemDrop()
        {
            return new BinItem(1);
        }
    }
}