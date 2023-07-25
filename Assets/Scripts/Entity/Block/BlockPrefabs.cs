using UnityEngine;

namespace FrontierIsland
{
    [CreateAssetMenu(fileName = "BlockPrefabs", menuName = "FrontierIsland/BlockPrefabs", order = 1)]

    public class BlockPrefabs : ScriptableObject
    {
        // the vairable names "prefabs" and "icons" is use to link to editor
        [SerializeField]
        private Block[] prefabs;

        public Block this[BlockType type]
        {
            get
            {
                return prefabs[(int)type];
            }
        }
    }
}
