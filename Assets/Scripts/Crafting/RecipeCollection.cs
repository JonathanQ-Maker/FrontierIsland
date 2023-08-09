using UnityEngine;
namespace FrontierIsland
{
    public static class RecipeCollections
    {
        public static readonly ItemRecipe[] CarpenterBench = new ItemRecipe[]
        {
            new ItemRecipe(ItemType.StoneAxe,
                new Ingredient[]
                {
                    new Ingredient(ItemType.Rocks, 1),
                    new Ingredient(ItemType.Grass, 2)
                }),

            new ItemRecipe(ItemType.WoodAxe,
                new Ingredient[]
                {
                    new Ingredient(ItemType.Grass, 1),
                    new Ingredient(ItemType.StoneAxe, 1),
                    new Ingredient(ItemType.Crate, 3)
                }),

            new ItemRecipe(ItemType.Mushrooms,
                new Ingredient[] {
                    new Ingredient(ItemType.Grass, 2)
                })
        };

        public static void SetUp()
        { 
            RecipeGraph.RegisterAll(CarpenterBench);
        }
    }
}
