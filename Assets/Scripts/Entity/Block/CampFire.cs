using System.Security.Authentication.ExtendedProtection;
using UnityEngine;

namespace FrontierIsland
{
    public class CampFire : CraftingBlock
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

        public override ItemRecipe[] Recipes { get { return RecipeCollections.CampFire; } }

        public override string Title { get { return "Camp Fire"; } }

        public override ItemStack[] GetItemDrops()
        {
            return new ItemStack[] { new CampFireItem(1) };
        }

        protected override Inventory CreateInventory()
        {
            return new Inventory(4, 1, this);
        }
    }
}