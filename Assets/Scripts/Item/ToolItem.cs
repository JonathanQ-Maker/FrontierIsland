namespace FrontierIsland
{
    public abstract class ToolItem : ItemStack
    {
        public override int MaxStackSize { get { return 1; } }

        protected ToolItem() : base(1)
        {

        }

        public virtual float GetEfficiency(Block block)
        {
            return 0.8f;
        }
    }
}