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
            return $"<size=14><color=#C5D4E9>{name}<size=12>\n" +
                    $"Efficiency: <color=#F2CA6F>{Mathf.Round(EfficiencyEffective * 100)}%</color>\n" +
                   (FuelValue > 0 ? $"Fuel Value: {FuelValue}\n" : "") +
                   $"<color=#898989><i>{description}</i></color>\n";
        }

        public override bool IsEffective(Block block)
        {
            return block.MaterialType == MaterialType.Wood;
        }
    }
}
