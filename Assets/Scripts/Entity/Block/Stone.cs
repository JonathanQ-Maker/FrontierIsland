namespace FrontierIsland
{
    public class Stone : Foliage
    {
        public override MaterialType MaterialType { get { return MaterialType.None; } }

        public override BlockType BlockType { get { return BlockType.Stone; } }

        public override ItemStack[] GetItemDrops()
        {
            return new ItemStack[] { new StoneItem(1) };
        }
    }
}
