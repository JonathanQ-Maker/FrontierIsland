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
                    new Ingredient(ItemType.Rocks, 3),
                    new Ingredient(ItemType.Twig, 3)
                }),

            new ItemRecipe(ItemType.CampFire,
                new Ingredient[]
                { 
                    new Ingredient(ItemType.Twig, 6)
                })
        };

        public static void SetUp()
        { 
            RecipeGraph.RegisterAll(CarpenterBench);
        }
    }
}
