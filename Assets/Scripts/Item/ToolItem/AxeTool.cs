namespace FrontierIsland
{
    public abstract class AxeTool : ToolItem
    {
        public AxeTool(string name, string description) : base(name, description)
        {

        }

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
