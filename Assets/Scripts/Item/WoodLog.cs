namespace FrontierIsland
{
    public class WoodLog : ItemStack
    {
        public override bool IsFuel
        {
            get { return true; }
        }

        public WoodLog(int count) : base(count, "Wood Log", "Big pieces of wood")
        {

        }

        public override ItemType ItemType { get { return ItemType.WoodLog; } }

        public override ItemStack DeepClone()
        {
            WoodLog clone = new WoodLog(count);
            CopyTo(clone);
            return clone;
        }
    }
}
