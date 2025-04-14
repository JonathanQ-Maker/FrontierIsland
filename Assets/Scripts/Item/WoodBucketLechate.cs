namespace FrontierIsland
{
    public class WoodBucketLechate : Bucket
    {
        public WoodBucketLechate() : base(1, "Wooden Bucket - Lechate", "Bucket of organic goo")
        {

        }

        public override ItemType ItemType { get { return ItemType.WoodBucketLechate; } }

        public override ItemStack DeepClone()
        {
            WoodBucketLechate clone = new WoodBucketLechate();
            CopyTo(clone);
            return clone;
        }
    }
}
