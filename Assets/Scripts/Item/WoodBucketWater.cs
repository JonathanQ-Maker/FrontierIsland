namespace FrontierIsland
{
    public class WoodBucketWater : Bucket
    {
        public WoodBucketWater() : base(1, "Wooden Bucket - Water", "Bucket of water")
        {

        }

        public override ItemType ItemType { get { return ItemType.WoodBucketWater; } }

        public override ItemStack DeepClone()
        {
            WoodBucketWater clone = new WoodBucketWater();
            CopyTo(clone);
            return clone;
        }
    }
}
