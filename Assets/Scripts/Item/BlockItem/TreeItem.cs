using NBT.Tags;
using UnityEngine;

namespace FrontierIsland
{
    public class TreeItem : BlockItem
    {
        public Tree.TreeState State 
        { 
            get { return (Tree.TreeState)NBT.GetByte("state"); } 
            set { NBT.PutByte("state", (byte)value); }

        }
        public TreeItem(int count, Tree.TreeState state) : base(count, "Tree", "a force of nature")
        {
            if (!HasNBT)
            {
                NBT = new CompoundTag(1);
                Debug.Log($"State: {state}");
                State = state;
            }
        }

        public override BlockType BlockType { get { return BlockType.Tree; } }

        public override ItemType ItemType { get { return ItemType.Tree; } }

        public override ItemStack DeepClone()
        {
            TreeItem clone = new TreeItem(count, State);
            CopyTo(clone);
            return clone;
        }
    }
}