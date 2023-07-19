namespace FrontierIsland
{
    public class Rocks : Block
    {
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
    }
}