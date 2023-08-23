namespace FrontierIsland
{
    public interface ICraftingStation : IViewable, IInventoryHolder
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
    }
}
