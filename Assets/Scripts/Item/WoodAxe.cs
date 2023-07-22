
namespace FrontierIsland
{
    public class WoodAxe : ItemStack
    {
        public override ItemType ItemType
        { 
            get { return ItemType.WoodAxe; }
        }

        public override int MaxStackSize { get { return 1; } }

        public WoodAxe(int count) : base(count)
        { 
        
        }

        public WoodAxe() : base(1)
        { 
        
        }
    }
}