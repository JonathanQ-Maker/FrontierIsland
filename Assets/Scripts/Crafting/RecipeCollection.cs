using UnityEngine;
namespace FrontierIsland
{
    public static class RecipeCollections
    {
        public static readonly ItemRecipe[] CarpenterBench = new ItemRecipe[]
        {
            new ItemRecipe(ItemType.WoodAxe,
                new Ingredient[]
                {
                    new Ingredient(ItemType.WoodLog, 5),
                    new Ingredient(ItemType.Twig, 3),
                }),

            new ItemRecipe(ItemType.StoneAxe,
                new Ingredient[]
                {
                    new Ingredient(ItemType.Stone, 3),
                    new Ingredient(ItemType.Twig, 3)
                }),

            new ItemRecipe(ItemType.CampFire,
                new Ingredient[]
                {
                    new Ingredient(ItemType.Twig, 6)
                }),

            new ItemRecipe(ItemType.Bin,
                new Ingredient[]
                {
                    new Ingredient(ItemType.WoodLog, 8)
                })
        };

        public static readonly ItemRecipe[] Settler = new ItemRecipe[]
        {
            new ItemRecipe(ItemType.CarpenterBench,
                new Ingredient[]
                {
                    new Ingredient(ItemType.WoodLog, 5),
                    new Ingredient(ItemType.Stone, 2)
                }),

            new ItemRecipe(ItemType.FistHatchet,
                new Ingredient[]
                { 
                    new Ingredient(ItemType.Stone, 2)
                })
        };

        public static void SetUp()
        { 
            RecipeGraph.RegisterAll(CarpenterBench);
            RecipeGraph.RegisterAll(Settler);
        }
    }
}
