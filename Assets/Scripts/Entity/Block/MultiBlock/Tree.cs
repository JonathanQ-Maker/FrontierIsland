using NBT.Tags;
using System.Collections;
using UnityEngine;

namespace FrontierIsland
{
    public class Tree : Block
    {
        public enum TreeState : byte
        {
            Sapling,
            Normal,
            Grown
        }

        private bool sheared = false;
        public bool Sheared { get { return sheared; } }

        [SerializeField]
        private Mesh sapling;
        [SerializeField]
        private Mesh treeTrunk, treeLeaves;
        [SerializeField]
        private Mesh grownTreeTrunk, grownTreeLeaves;

        [SerializeField]
        private MeshFilter trunkMeshFilter, leavesMeshFilter;

        [SerializeField]
        private BoxCollider boxCollider;

        private TreeState state = TreeState.Normal;
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

        public override bool Solid { get { return State != TreeState.Sapling; } }

        public override MaterialType MaterialType
        {
            get
            {
                return MaterialType.Wood;
            }
        }

        private void SetState(TreeState state)
        {
            this.state = state;
            switch (state)
            {
                case TreeState.Sapling:
                    trunkMeshFilter.sharedMesh = sapling;
                    leavesMeshFilter.gameObject.SetActive(false);
                    break;
                case TreeState.Normal:
                    trunkMeshFilter.sharedMesh = treeTrunk;
                    leavesMeshFilter.sharedMesh = treeLeaves;
                    leavesMeshFilter.gameObject.SetActive(!Sheared);
                    break;
                case TreeState.Grown:
                    trunkMeshFilter.sharedMesh = grownTreeTrunk;
                    leavesMeshFilter.sharedMesh = grownTreeLeaves;
                    leavesMeshFilter.gameObject.SetActive(!Sheared);
                    break;
                default:
                    Debug.LogWarning($"Set unexpected state {state}");
                    break;
            }
            Bounds bounds = trunkMeshFilter.sharedMesh.bounds;
            if (State != TreeState.Sapling)
            {
                bounds.Encapsulate(leavesMeshFilter.sharedMesh.bounds);
            }
            boxCollider.size = bounds.size;
            boxCollider.center = bounds.center;
        }

        private void SetSheared(bool sheared)
        {
            this.sheared = sheared;
            leavesMeshFilter.gameObject.SetActive(!sheared && State != TreeState.Sapling);
        }

        public override void ReadFromNBT(CompoundTag nbt)
        {
            base.ReadFromNBT(nbt);
            
            // Left most bit encodes sheared,
            // rest encodes TreeState
            byte state = nbt.GetByte("state");
            TreeState treeState = (TreeState)(state & ~(1 << 7));
            bool sheared = (state & (1 << 7)) != 0;
            SetState(treeState);
            SetSheared(sheared);
        }

        public override void WriteToNBT(CompoundTag nbt)
        {
            base.WriteToNBT(nbt);
            int state = (int)State;
            if (Sheared)
            {
                state |= 1 << 7; // set bit
            }
            else
            { 
                state &= ~(1 << 7); // clear bit
            }
            nbt.PutByte("state", (byte)state);
        }

        protected override void Start()
        {
            base.Start();
            SetState(State);
            SetSheared(Sheared);
            StartCoroutine(GrowthTimer());
        }

        public override ItemStack[] GetItemDrops()
        {
            int amount = Random.Range(2, 6);
            if (State == TreeState.Grown)
            {
                amount *= 2;
            }
            
            return new ItemStack[] { new WoodLog(amount), 
                                     new TreeCone(Random.Range(1, 3)), 
                                     new Twig(Random.Range(1, 3))};
        }

        private IEnumerator GrowthTimer()
        {
            while((int)State < (int)TreeState.Grown)
            {
                yield return new WaitForSeconds(Random.Range(5, 5));

                // Grow leaves before growing tree
                if (State != TreeState.Sapling && Sheared)
                {
                    SetSheared(false);
                }
                else
                {
                    SetState((TreeState)((int)State + 1));
                }
            }
        }
    }
}

