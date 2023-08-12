namespace FrontierIsland
{
    public class CampFire : Block
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
            get { return BlockType.CampFire; }
        }

        public override ItemStack[] GetItemDrops()
        {
            return new ItemStack[] { new CampFireItem(1) };
        }
    }
}