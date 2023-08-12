namespace FrontierIsland
{
    public class Rocks : Block
    {
        public override MaterialType MaterialType 
        { 
            get 
            { 
                return MaterialType.Rock; 
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
            get { return BlockType.Rocks; }
        }

        public override ItemStack[] GetItemDrops()
        {
            return new ItemStack[] { new RocksItem(1) };
        }
    }
}