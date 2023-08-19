namespace FrontierIsland
{
    public class WoodAxe : AxeTool
    {
        public WoodAxe() : this("Wood Axe", "An axe made \r\nentirely out of wood")
        { 
        
        }
        public WoodAxe(string name, string description) : base(name, description)
        {
        }

        public override ItemType ItemType
        { 
            get { return ItemType.WoodAxe; }
        }

        public override float EfficiencyEffective
        {
            get { return 1.25f; }
        }

        public override ItemStack DeepClone()
        {
            WoodAxe clone = new WoodAxe();
            CopyTo(clone);
            return clone;
        }
    }
}