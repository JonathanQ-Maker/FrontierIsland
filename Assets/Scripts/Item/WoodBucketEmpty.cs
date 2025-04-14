namespace FrontierIsland
{
    public class WoodBucketEmpty : Bucket
    {
        public WoodBucketEmpty() : base(1, "Wooden Bucket - Empty", "Big wood cup")
        {

        }

        public override ItemType ItemType { get { return ItemType.WoodBucketEmpty; } }

        public override ItemStack DeepClone()
        {
            WoodBucketEmpty clone = new WoodBucketEmpty();
            CopyTo(clone);
            return clone;
        }
    }
}
