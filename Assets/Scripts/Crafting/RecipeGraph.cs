using System;
using System.Collections.Generic;

namespace FrontierIsland
{
    public static class RecipeGraph
    {
        public class RecipeGraphNode
        {
            public HashSet<ItemType> requiredBy = new HashSet<ItemType>(1);
            public List<ItemType[]> ingredients = new List<ItemType[]>(1);

            internal RecipeGraphNode()
            { 
                
            }
        }

        private static Dictionary<ItemType, RecipeGraphNode> nodes 
            = new Dictionary<ItemType, RecipeGraphNode>(Enum.GetValues(typeof(ItemType)).Length);

        public static bool TryGetNode(ItemType itemType, out RecipeGraphNode node)
        {
            return nodes.TryGetValue(itemType, out node);
        }

        public static Dictionary<ItemType, RecipeGraphNode>.KeyCollection Keys 
        { 
            get { return nodes.Keys; } 
        }

        private static void RegisterRecipe(ItemRecipe recipe)
        {
            if (nodes.TryGetValue(recipe.ResultItem, out RecipeGraphNode node))
            {
                node.ingredients.Add(recipe.Ingredients);
            }
            else
            {
                RecipeGraphNode newNode = new RecipeGraphNode();
                newNode.ingredients.Add(recipe.Ingredients);
                nodes.Add(recipe.ResultItem, newNode);
            }
            UpdateRequiredBy(recipe.ResultItem, recipe.Ingredients);
        }

        private static void UpdateRequiredBy(ItemType requiredBy, ItemType[] ingredients)
        {
            for (int i = 0; i < ingredients.Length; ++i)
            {
                if (nodes.TryGetValue(ingredients[i], out RecipeGraphNode node)) 
                {
                    node.requiredBy.Add(requiredBy);
                }
            }
        }

        public static void RegisterAll(ItemRecipe[] recipes)
        {
            for (int i = 0; i < recipes.Length; ++i)
            {
                RegisterRecipe(recipes[i]);
            }
        }
    }
}
