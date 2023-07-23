
namespace FrontierIsland
{
    public class StoneAxe : ItemStack
    {
        public override ItemType ItemType
        {
            get { return ItemType.StoneAxe; }
        }

        public override int MaxStackSize { get { return 2; } }

        public StoneAxe(int count) : base(count)
        {

        }

        public StoneAxe() : base(1)
        {

        }
    }
}