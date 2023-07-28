using NBT.Tags;
using UnityEngine;

namespace FrontierIsland
{
    public class Tree : MultiBlock
    {
        public enum TreeState : byte
        {
            Normal,
            Grown
        }

        [SerializeField]
        private Mesh normalTree, grownTree;

        [SerializeField]
        private MeshFilter meshFilter;

        [SerializeField]
        private BoxCollider boxCollider;

        private TreeState state;
        public TreeState State
        {
            get { return state; }
        }

        public override BlockType BlockType
        {
            get { return BlockType.Tree; }
        }

        public override Vector3Int Size
        {
            get
            {
                return new Vector3Int(1, 2, 1);
            }
        }

        public override MaterialType MaterialType
        {
            get
            {
                return MaterialType.Wood;
            }
        }

        private void TrySetState(TreeState state)
        {
            TreeState prevState = State;
            switch (state)
            {
                case TreeState.Normal:
                    meshFilter.sharedMesh = normalTree;
                    break;
                case TreeState.Grown:
                    meshFilter.sharedMesh = grownTree;
                    break;
                default:
                    Debug.LogWarning($"Set unexpected state {state}");
                    break;
            }
            boxCollider.size = meshFilter.mesh.bounds.size;
            boxCollider.center = meshFilter.mesh.bounds.center;
        }

        public override void ReadFromNBT(CompoundTag nbt)
        {
            base.ReadFromNBT(nbt);
            TrySetState((TreeState)nbt.GetByte("state"));
        }

        public override void WriteToNBT(CompoundTag nbt)
        {
            base.WriteToNBT(nbt);
            nbt.PutByte("state", (byte)State);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                TrySetState(TreeState.Grown);
            }
        }

        public override ItemStack GetItemDrop()
        {
            return new TreeItem(1, State);
        }
    }
}

