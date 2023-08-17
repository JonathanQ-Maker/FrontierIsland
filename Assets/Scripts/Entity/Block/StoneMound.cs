namespace FrontierIsland
{
    public class StoneMound : Block
    {
        public override MaterialType MaterialType 
        { 
            get 
            { 
                return MaterialType.Stone; 
            } 
        }


        public override float Hardness
        { 
            get
            {
                return 5f;
            }    
        }

        public override BlockType BlockType
        {
            get { return BlockType.StoneMound; }
        }

        public override ItemStack[] GetItemDrops()
        {
            return new ItemStack[] { new StoneItem(1) };
        }
    }
}