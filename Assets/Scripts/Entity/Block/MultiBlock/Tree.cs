using UnityEngine;

namespace FrontierIsland
{
    public class Tree : MultiBlock
    {
        public enum TreeState : byte
        {
            Normal,
            Tall
        }

        [SerializeField]
        private Mesh normalTree, tallTree;

        [SerializeField]
        private MeshFilter meshFilter;

        [SerializeField]
        private BoxCollider boxCollider;

        private TreeState state;
        public TreeState State
        {
            get { return state; }
            protected set
            {
                state = value;
                SetState(state);
            }
        }

        public override BlockType BlockType
        {
            get { return BlockType.Tree; }
        }

        public override Vector3Int Size
        {
            get
            {
                if (State == TreeState.Normal)
                    return new Vector3Int(1, 2, 1);
                return new Vector3Int(1, 3, 1);
            }
        }

        private void SetState(TreeState state)
        {
            switch (state)
            {
                case TreeState.Normal:
                    meshFilter.sharedMesh = normalTree;
                    break;
                case TreeState.Tall:
                    meshFilter.sharedMesh = tallTree;
                    break;
                default:
                    Debug.LogWarning($"Set unexpected state {state}");
                    break;
            }
            boxCollider.size = meshFilter.mesh.bounds.size;
            boxCollider.center = meshFilter.mesh.bounds.center;
        }
    }
}

