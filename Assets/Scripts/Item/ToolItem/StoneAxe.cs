
namespace FrontierIsland
{
    public class StoneAxe : AxeTool
    {

        public StoneAxe() : this("Stone Axe", "An axe made \r\nwith a stone head")
        {

        }
        public StoneAxe(string name, string description) : base(name, description)
        {

        }

        public override ItemType ItemType
        {
            get { return ItemType.StoneAxe; }
        }

        public override float EfficiencyEffective
        {
            get { return 1.5f; }
        }

        public override ItemStack DeepClone()
        {
            StoneAxe clone = new StoneAxe();
            CopyTo(clone);
            return clone;
        }
    }
}