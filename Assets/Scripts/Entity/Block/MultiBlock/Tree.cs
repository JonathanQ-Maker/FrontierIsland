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

        [SerializeField]
        private Mesh sapling, normalTree, grownTree;

        [SerializeField]
        private MeshFilter meshFilter;

        [SerializeField]
        private BoxCollider boxCollider;

        [SerializeField]
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

        private void TrySetState(TreeState state)
        {
            switch (state)
            {
                case TreeState.Sapling:
                    meshFilter.sharedMesh = sapling;
                    break;
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
            this.state = state;
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

        private void Start()
        {
            TrySetState(State);
            StartCoroutine(GrowthTimer());
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                TrySetState(TreeState.Grown);
            }
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
            for (int i = (int)State + 1; i <= (int)TreeState.Grown; ++i)
            {
                yield return new WaitForSeconds(Random.Range(10, 60));
                TrySetState((TreeState)i);
            }
        }
    }
}

