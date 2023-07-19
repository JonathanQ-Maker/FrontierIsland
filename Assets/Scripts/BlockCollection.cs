using UnityEngine;

namespace FrontierIsland
{
    [CreateAssetMenu]
    public class BlockCollection : ScriptableObject
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
