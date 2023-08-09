namespace FrontierIsland
{
    public class ItemRecipe
    {
        public ItemType ResultItem { get; protected set; }
        private Ingredient[] ingredients;
        public Ingredient[] Ingredients { get { return ingredients; } }

        public ItemRecipe(ItemType resultItem, Ingredient[] ingredients)
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

    public class Ingredient
    {
        public readonly ItemType item;

        public readonly int count;


        public Ingredient(ItemType item, int count)
        {
            this.item = item;
            this.count = count;
        }
    }
}
