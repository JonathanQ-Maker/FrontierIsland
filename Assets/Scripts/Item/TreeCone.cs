using NBT.Tags;

namespace FrontierIsland
{
    public class TreeCone : BlockItem
    {
        public TreeCone(int count) : base(count, "Tree Cone", "Tree fruit")
        {
            if (!HasNBT)
            {
                NBT = new CompoundTag(1);    
            }

            // TreeState: sapling, Sheared: false.
            NBT.PutByte("state", 0);
        }

        public override ItemType ItemType { get { return ItemType.TreeCone; } }

        public override BlockType BlockType { get { return BlockType.Tree; } }

        public override ItemStack DeepClone()
        {
            TreeCone clone = new TreeCone(count);
            CopyTo(clone);
            return clone;
        }
    }
}
