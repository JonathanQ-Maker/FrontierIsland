namespace FrontierIsland
{
    public abstract class AxeTool : ToolItem
    {
        protected virtual float BaseEfficiency { get; }

        public override float GetEfficiency(Block block)
        {
            if (block.MaterialType == MaterialType.Wood)
            {
                return BaseEfficiency;
            }
            return base.GetEfficiency(block);
        }
    }
}
