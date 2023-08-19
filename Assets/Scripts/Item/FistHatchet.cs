namespace FrontierIsland
{
    public class FistHatchet : KnifeTool
    {
        public FistHatchet() : base("Fist Hatchet", "Sharpened stone perfect\nfor cutting small branches")
        {

        }

        public override ItemType ItemType { get { return ItemType.FistHatchet; } }

        public override ItemStack DeepClone()
        {
            FistHatchet clone = new FistHatchet();
            CopyTo(clone);
            return clone;
        }
    }
}
