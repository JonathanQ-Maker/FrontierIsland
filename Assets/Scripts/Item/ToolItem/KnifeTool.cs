namespace FrontierIsland
{
    public abstract class KnifeTool : ItemStack
    {
        public override int MaxStackSize { get { return 1; } }

        protected KnifeTool(string name, string description) : base(1, name, description)
        {

        }

        public override string GetToolTip()
        {
            return base.GetToolTip();
        }

        public override ItemStack[] HarvestBlock(Block block)
        {
            if (block is Tree tree)
            {
                if (tree.CanShear)
                {
                    return new ItemStack[] { tree.Shear() };
                }
            }
            return base.HarvestBlock(block);
        }
    }
}
