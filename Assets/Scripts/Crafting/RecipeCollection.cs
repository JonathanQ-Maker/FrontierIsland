using UnityEngine;
namespace FrontierIsland
{
    public static class RecipeCollections
    {
        public static readonly ItemRecipe[] CarpenterBench = new ItemRecipe[]
        {
            new ItemRecipe(ItemType.StoneAxe,
                new ItemType[]
                {
                    ItemType.Rocks, ItemType.Grass
                }),

            new ItemRecipe(ItemType.WoodAxe,
                new ItemType[]
                {
                    ItemType.Grass, ItemType.StoneAxe, ItemType.Crate
                })
        };

        public static void SetUp()
        { 
            RecipeGraph.RegisterAll(CarpenterBench);
        }
    }
}
