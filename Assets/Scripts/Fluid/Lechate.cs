
namespace FrontierIsland
{
    public class Lechate : Fluid
    {
        public Lechate(int amount) : base(amount, "organic goo")
        {
        }

        public override string Name { get { return "Lechate"; } }

        public override FluidType FluidType { get { return FluidType.Lechate; } }

        public override Fluid DeepClone()
        {
            Lechate clone = new Lechate(amount);
            clone.amount = amount;
            return clone;
        }
    }
}
