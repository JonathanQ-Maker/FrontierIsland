namespace FrontierIsland
{
    public class Water : Fluid
    {
        public Water(int amount) : base(amount, "H2O")
        {

        }

        public override string Name { get { return "Water"; } }

        public override FluidType FluidType { get { return FluidType.Water; } }

        public override Fluid DeepClone()
        {
            Water clone = new Water(amount);
            CopyTo(clone);
            return clone;
        }
    }
}
