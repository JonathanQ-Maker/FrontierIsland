
namespace FrontierIsland
{
    public class StoneAxe : AxeTool
    {
        public override ItemType ItemType
        {
            get { return ItemType.StoneAxe; }
        }

        protected override float BaseEfficiency
        {
            get { return 0.25f; }
        }
    }
}