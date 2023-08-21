using UnityEngine;

namespace FrontierIsland
{
    public class CampFire : Block
    {
        // TODO: make functional

        [SerializeField]
        private Transform fireDisplay;

        public bool Ignited 
        { 
            get { return fireDisplay.gameObject.activeSelf; } 
            set { fireDisplay.gameObject.SetActive(value); }
        }

        public override MaterialType MaterialType
        {
            get { return MaterialType.Wood; }
        }

        public override BlockType BlockType
        {
            get { return BlockType.CampFire; }
        }

        public override ItemStack[] GetItemDrops()
        {
            return new ItemStack[] { new CampFireItem(1) };
        }
    }
}