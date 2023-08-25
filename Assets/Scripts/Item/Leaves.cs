namespace FrontierIsland
{
    public class Leaves : ItemStack
    {
        public override int FuelValue { get { return 50; } }
        public Leaves(int count) : base(count, "Leaves", "Organic material from a tree")
        {

        }

        public override ItemType ItemType { get { return ItemType.Leaves; } }

        public override ItemStack DeepClone()
        {
            Leaves clone = new Leaves(count);
            CopyTo(clone);
            return clone;
        }
    }
}
