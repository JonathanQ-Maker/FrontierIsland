using System;

namespace FrontierIsland
{
    public abstract class ToolItem : ItemStack
    {
        protected virtual float BaseEfficiency { get; }
        public override int MaxStackSize { get { return 1; } }

        protected ToolItem(string name, string description) : base(1, name, description)
        {

        }

        public virtual float GetEfficiency(Block block)
        {
            return 1f;
        }

        public override string GetToolTip()
        {
            return base.GetToolTip() 
                + $"Efficiency: <color=#F2CA6F>{Math.Round(BaseEfficiency * 100)}%</color>\n";
        }
    }
}