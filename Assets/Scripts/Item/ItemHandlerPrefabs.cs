using UnityEngine;

namespace FrontierIsland
{
    [CreateAssetMenu(fileName = "ItemHandlerPrefabs", menuName = "FrontierIsland/ItemHandlerPrefabs", order = 1)]
    public class ItemHandlerPrefabs : ScriptableObject
    {
        // vairable name prefabs is use to link to editor

        [SerializeField]
        private ItemHandler[] prefabs;
        [SerializeField]
        private Sprite[] icons;

        public ItemHandler this[ItemType type]
        {
            get
            {
                return prefabs[(int)type];
            }
        }

        public Sprite GetIcon(ItemType type)
        { 
            return icons[(int)type];
        }
    }
}