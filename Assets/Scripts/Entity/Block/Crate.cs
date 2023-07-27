namespace FrontierIsland
{
    public class Crate : Block
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
            get { return BlockType.Crate; }
        }

        public override ItemStack GetItemDrop()
        {
            return new CrateItem(1);
        }
    }
}

