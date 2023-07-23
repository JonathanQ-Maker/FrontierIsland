
namespace FrontierIsland
{
    public class WoodAxe : AxeTool
    {
        public override ItemType ItemType
        { 
            get { return ItemType.WoodAxe; }
        }

        protected override float BaseEfficiency
        {
            get { return 0.5f; }
        }
    }
}