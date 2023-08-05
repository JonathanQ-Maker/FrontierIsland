using UnityEngine;

namespace FrontierIsland
{
    public interface ICraftingStation : IViewable
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
    }
}
