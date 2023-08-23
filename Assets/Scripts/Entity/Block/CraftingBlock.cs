
namespace FrontierIsland
{
    public abstract class CraftingBlock : ViewableBlock, ICraftingStation
    {
        public override MaterialType MaterialType { get { return MaterialType.None; } }

        public abstract ItemRecipe[] Recipes { get; }

        private Inventory inventory;
        public virtual Inventory Inventory
        {
            get { return inventory; }
            set
            {
                if (inventory != null)
                {
                    inventory.onInventoryChange -= OnInventoryChange;
                }
                inventory = value;
                inventory.onInventoryChange += OnInventoryChange;
            }
        }

        protected bool active = false;
        public bool Functional { get { return active; } }

        DebugTracker tracker;
        protected override void Start()
        {
            tracker = new DebugTracker("CrafingBlock");
            base.Start();
            Inventory = CreateInventory();
            active = true;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Inventory.onInventoryChange -= OnInventoryChange;
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

        protected virtual Inventory CreateInventory()
        { 
            return new Inventory(4, 2, this);
        }

        protected override void InitUI()
        {
            ((CraftingStationUI)ui).Init(this);
        }

        public virtual void OnInventoryChange()
        {
            // intentionally left empty
        }

        public void DropItem(ItemStack itemStack)
        {
            if (Viewer == null)
            {
                itemStack.InstantiateHandler(Position, null);
            }
            else
            {
                Viewer.DropItem(itemStack);
            }
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
