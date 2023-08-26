using NBT.Tags;
using System.Collections;
using UnityEngine;

namespace FrontierIsland
{
    public class TreeBlock : Block
    {
        public static int TREE_STATE_MASK = 3;
        public static int NUM_FRUIT_MASK = 124;
        public static int HAS_LEAVES_MASK = 128;

        // TODO: Generalize for all trees
        public enum TreeStage : byte
        {
            Sapling,
            Normal,
            Grown
        }

        public virtual bool HasLeaves 
        { 
            get { return leavesMeshFilter.gameObject.activeSelf; }
            set 
            { 
                leavesMeshFilter.gameObject.SetActive(value && Stage != TreeStage.Sapling);
                UpdateBounds();
            }
        }

        private int numFruits = 0;
        public virtual int NumFruits 
        { 
            // NOTE: current encoding only allows NumFruits <= 31
            get { return numFruits; }
            set 
            { 
                numFruits = value;

                // update visuals
                for (int i = 0; i < fruitSpawnPoints.Length; ++i)
                {
                    // only show fruit (tree sap) if is normal tree
                    fruitSpawnPoints[i].gameObject.SetActive(Stage == TreeStage.Normal && i < numFruits);
                }
            }
        }

        [SerializeField]
        private Mesh sapling;
        [SerializeField]
        private Mesh treeTrunk, treeLeaves;
        [SerializeField]
        private Mesh grownTreeTrunk, grownTreeLeaves;

        [SerializeField]
        private MeshFilter trunkMeshFilter, leavesMeshFilter;
        [SerializeField]
        private Transform[] fruitSpawnPoints;

        [SerializeField]
        private BoxCollider boxCollider;

        private TreeStage stage = TreeStage.Normal;
        public TreeStage Stage
        {
            get { return stage; }
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

        public override bool Solid { get { return Stage != TreeStage.Sapling; } }

        public override MaterialType MaterialType
        {
            get
            {
                return MaterialType.Wood;
            }
        }

        public virtual bool CanShear
        { 
            get { return Stage != TreeStage.Sapling && (HasLeaves || NumFruits > 0);} 
        }

        protected virtual void SetStage(TreeStage stage)
        {
            this.stage = stage;
            switch (stage)
            {
                case TreeStage.Sapling:
                    trunkMeshFilter.sharedMesh = sapling;
                    break;
                case TreeStage.Normal:
                    trunkMeshFilter.sharedMesh = treeTrunk;
                    leavesMeshFilter.sharedMesh = treeLeaves;
                    HasLeaves = true;
                    break;
                case TreeStage.Grown:
                    trunkMeshFilter.sharedMesh = grownTreeTrunk;
                    leavesMeshFilter.sharedMesh = grownTreeLeaves;
                    HasLeaves = true;
                    break;
                default:
                    Debug.LogWarning($"Set unexpected stage {stage}");
                    break;
            }
            UpdateBounds();
        }

        protected void UpdateBounds()
        { 
            Bounds bounds = trunkMeshFilter.sharedMesh.bounds;
            if (HasLeaves)
            {
                bounds.Encapsulate(leavesMeshFilter.sharedMesh.bounds);
            }
            boxCollider.size = bounds.size;
            boxCollider.center = bounds.center;
        }

        public ItemStack Shear()
        {
            if (!CanShear)
            {
                return null;
            }


            if (HasLeaves)
            {
                HasLeaves = false;
                return new Leaves(3);
            }
            else if (NumFruits > 0)
            {
                int fruits = NumFruits;
                NumFruits = 0;
                return new TreeSap(fruits);
            }

            // if it reacehs here it probably
            // means that tree has been sheared
            // just does not give anything for shearing
            return null;
        }

        public override void ReadFromNBT(CompoundTag nbt)
        {
            /* 
             * Left most bit encodes !HasLeaves, 
             * 2 right most bit encodes TreeState, 
             * rest encodes NumFruit.
             * 
             * NOTE: HasLeaves is encoded as !HasLeaves
             * because this allows the binary state 0000_0000
             * to represent HasLeaves=true as the default
             * 
             * *=============================*
             * |!HasLeaves|Numfruit|TreeStage|
             * *=============================*
             */
            base.ReadFromNBT(nbt);
            byte state = nbt.GetByte("state");
            SetStage((TreeStage)(state & TREE_STATE_MASK));
            HasLeaves = (state & HAS_LEAVES_MASK) == 0;
            NumFruits = (state & NUM_FRUIT_MASK) >> 2;
        }

        public override void WriteToNBT(CompoundTag nbt)
        {
            base.WriteToNBT(nbt);
            int state = (int)Stage;
            if (!HasLeaves)
            {
                state |= 1 << 7; // set bit
            }
            else
            { 
                state &= ~(1 << 7); // clear bit
            }
            state &= ~NUM_FRUIT_MASK;
            state |= NumFruits << 2;
            nbt.PutByte("state", (byte)state);
        }

        protected override void Start()
        {
            base.Start();
            SetStage(Stage);
            StartCoroutine(GrowthCycle());
        }

        public override ItemStack[] GetItemDrops()
        {
            int amount = Random.Range(2, 6);
            if (Stage == TreeStage.Grown)
            {
                amount *= 2;
            }
            
            return new ItemStack[] { new WoodLog(amount), 
                                     new TreeCone(Random.Range(1, 3)), 
                                     new Twig(Random.Range(1, 3))};
        }

        private IEnumerator GrowthCycle()
        {
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(30, 120));

                if (Stage == TreeStage.Normal && !HasLeaves)
                {
                    if (NumFruits < fruitSpawnPoints.Length)
                        ++NumFruits;
                }
                else if (Stage <= TreeStage.Grown)
                {
                    if (Stage != TreeStage.Sapling && !HasLeaves)
                    {
                        HasLeaves = true;
                    }
                    else if (Stage < TreeStage.Grown)
                    {
                        SetStage((TreeStage)((byte)Stage + 1));
                    }
                }
            }
        }
    }
}

