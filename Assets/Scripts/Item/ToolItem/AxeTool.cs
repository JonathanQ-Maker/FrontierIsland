using UnityEngine;

namespace FrontierIsland
{
    public abstract class AxeTool : ItemStack
    {
        public override int MaxStackSize { get { return 1; } }
        public AxeTool(string name, string description) : base(1, name, description)
        {

        }

        public override string GetToolTip()
        {
            return base.GetToolTip()
                + $"Efficiency: <color=#F2CA6F>{Mathf.Round(EfficiencyEffective * 100)}%</color>\n";
        }

        public override bool IsEffective(Block block)
        {
            return block.MaterialType == MaterialType.Wood;
        }
    }
}
