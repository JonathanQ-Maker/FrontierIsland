namespace FrontierIsland
{
    public class ItemRecipe
    {
        public ItemType ResultItem { get; protected set; }
        public float CraftTime { get; protected set; }
        public Ingredient[] Ingredients { get; protected set; }

        public ItemRecipe(ItemType resultItem, Ingredient[] ingredients) : this(resultItem, ingredients, 3)
        { 
        
        }

        public ItemRecipe(ItemType resultItem, Ingredient[] ingredients, float craftTime)
        {
            ResultItem = resultItem;
            CraftTime = craftTime;
            Ingredients = ingredients;
        }

        /// <summary>
        /// Check if inventory has nessesary ingredients
        /// </summary>
        /// <param name="inventory"></param>
        /// <returns></returns>
        public bool Match(Inventory inventory, int count)
        {
            for (int i = 0; i < Ingredients.Length; ++i)
            {
                Ingredient ingredient = Ingredients[i];
                ItemStack item = inventory[i];
                if (item == null ||
                    ingredient.item != item.ItemType ||
                    ingredient.count * count > item.count)
                {
                    return false;
                }
            }
            return true;
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
