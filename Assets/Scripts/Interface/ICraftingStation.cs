using UnityEngine;

namespace FrontierIsland
{
    public interface ICraftingStation : IViewable, IInventoryHolder
    {
        /// <summary>
        /// Array of <see cref="ItemRecipe"/> availabe from this <see cref="ICraftingStation"/>
        /// </summary>
        ItemRecipe[] Recipes { get; }

        /// <summary>
        /// The display title for <see cref="CraftingStationUI"/>
        /// </summary>
        string Title { get; }

        /// <summary>
        /// The <see cref="Transform"/> that <see cref="CraftingStationUI"/> will follow
        /// </summary>
        Transform transform { get; }

        /// <summary>
        /// <br>
        /// Consume ingredient and give result item to <see cref="IViewable.Viewer"/>
        /// </br>
        /// 
        /// </summary>
        /// <param name="recipeIndex"></param>
        /// <param name="count"></param>
        void Craft(int recipeIndex, int count)
        {
            ItemRecipe recipe = Recipes[recipeIndex];
            for (int i = 0; i < recipe.Ingredients.Length; ++i)
            {
                Ingredient ingredient = recipe.Ingredients[i];
                if (Inventory[i] == null ||
                    ingredient.item != Inventory[i].ItemType ||
                    ingredient.count * count > Inventory[i].count)
                {
                    return;
                }
            }

            for (int i = 0; i < recipe.Ingredients.Length; ++i)
            {
                Ingredient ingredient = recipe.Ingredients[i];
                Inventory.ConsumeItem(i, ingredient.count * count);
            }

            ItemStack result = ItemAtlas.Get(recipe.ResultItem).DeepClone();
            result.count = count;
            if (!Viewer.Inventory.AddItem(result))
            {
                DropItem(result);
            }
        }
    }
}
