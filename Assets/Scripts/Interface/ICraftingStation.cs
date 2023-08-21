using System.Collections;
using UnityEngine;

namespace FrontierIsland
{
    public interface ICraftingStation : IViewable, IInventoryHolder, IUseable
    {
        /// <summary>
        /// Array of <see cref="ItemRecipe"/> availabe from this <see cref="ICraftingStation"/>
        /// </summary>
        ItemRecipe[] Recipes { get; }

        bool Functional { get; }

        /// <summary>
        /// <br>
        /// Consume ingredient and give result item to <see cref="IViewable.Viewer"/>
        /// </br>
        /// 
        /// </summary>
        /// <param name="recipeIndex"></param>
        /// <param name="count"></param>
        void Craft(int recipeIndex, int count);

        /// <summary>
        /// Coroutine to perform the crafting animation
        /// </summary>
        /// <param name="recipeIndex"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        IEnumerator Crafting(int recipeIndex, int count);

        /// <summary>
        /// Called when the craft button is clicked
        /// </summary>
        /// <param name="recipeIndex"></param>
        /// <param name="count"></param>
        void OnCraft(int recipeIndex, int count);
    }
}
