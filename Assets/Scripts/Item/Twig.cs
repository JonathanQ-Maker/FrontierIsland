namespace FrontierIsland
{
    public class Twig : ItemStack
    {
        public override int FuelValue { get { return 50; } }

        public Twig(int count) : base(count, "Twig", "sticks from trees")
        {

        }

        public override ItemType ItemType { get { return ItemType.Twig; } }

        public override ItemStack DeepClone()
        {
            Twig clone = new Twig(count);
            CopyTo(clone);
            return clone;
        }
    }
}
