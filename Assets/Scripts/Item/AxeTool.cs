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

        public override string GetToolTip()
        {
            return base.GetToolTip() + $"<color=#898989><i>{description}</i></color>";
        }
    }
}
