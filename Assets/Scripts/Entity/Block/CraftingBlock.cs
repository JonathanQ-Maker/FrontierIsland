
namespace FrontierIsland
{
    public abstract class CraftingBlock : ContainerBlock, ICraftingStation
    {
        public override MaterialType MaterialType { get { return MaterialType.None; } }

        public abstract ItemRecipe[] Recipes { get; }

        protected bool active = false;
        public bool Functional { get { return active; } }

        protected override void Start()
        {
            base.Start();
            active = true;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            active = false;
        }


        protected virtual void OnEnable()
        {
            active = true;
        }

        protected virtual void OnDisable()
        {
            active = false;
        }

        protected override Inventory CreateInventory()
        { 
            return new Inventory(4, 1, this);
        }

        protected override void InitUI()
        {
            ((CraftingStationUI)ui).Init(this);
        }

        public virtual void Craft(int recipeIndex, int count)
        {
            ItemRecipe recipe = Recipes[recipeIndex];
            if (!recipe.Match(Inventory, count)) return;

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
