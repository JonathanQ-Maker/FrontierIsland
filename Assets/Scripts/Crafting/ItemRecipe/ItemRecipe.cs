namespace FrontierIsland
{
    public class ItemRecipe
    {
        public ItemType ResultItem { get; protected set; }
        private ItemType[] ingredients;
        public ItemType[] Ingredients { get { return ingredients; } }

        public ItemRecipe(ItemType resultItem, ItemType[] ingredients)
        {
            ResultItem = resultItem;
            this.ingredients = ingredients;
        }

        /// <summary>
        /// Check if inventory has nessesary ingredients
        /// </summary>
        /// <param name="inventory"></param>
        /// <returns></returns>
        public bool Matches(Inventory inventory)
        {
            throw new System.NotImplementedException();
        }
    }
}
