using UnityEngine;


namespace FrontierIsland
{
    [CreateAssetMenu(fileName = "ItemIcons", menuName = "FrontierIsland/ItemIcons", order = 1)]
    public class ItemIcons : ScriptableObject
    {
        [SerializeField]
        private Sprite[] icons;

        public Sprite this[ItemType type]
        {
            get
            {
                return icons[(int)type];
            }
        }
    }
}
