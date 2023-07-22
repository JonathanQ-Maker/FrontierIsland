using UnityEngine;

namespace FrontierIsland
{
    [CreateAssetMenu(fileName = "BlockPrefabs", menuName = "FrontierIsland/BlockPrefabs", order = 1)]

    public class BlockPrefabs : ScriptableObject
    {
        [SerializeField]
        private Block[] prefabs; // name is important for editor

        public Block this[BlockType type]
        {
            get
            {
                return prefabs[(int)type];
            }
        }
    }
}
