using UnityEngine;


namespace FrontierIsland
{
    public abstract class CraftingBlock : Block, ICraftingStation
    {
        [SerializeField]
        private CraftingStationUI stationUIPrefab;
        private CraftingStationUI stationUI;

        public override MaterialType MaterialType { get { return MaterialType.None; } }

        public abstract ItemRecipe[] Recipes { get; }

        public bool IsViewed
        {
            get { return Viewer != null; }
        }

        public abstract string Title { get; }

        public Settler Viewer { get; protected set; }

        private bool showUI = true;
        public bool ShowUI 
        { 
            get { return showUI; }
            set 
            { 
                showUI = value;
                if (stationUI != null)
                    stationUI.Active = ShowUI;
            }
        }

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

        private void Start()
        {
            Inventory = new Inventory(4, 2, this);
        }

        private void OnDestroy()
        {
            // clean up possible ui
            if (stationUI != null) CloseUI();
            Inventory.onInventoryChange -= OnInventoryChange;
        }

        public virtual void CloseUI()
        {
            if (stationUI != null)
            {
                Destroy(stationUI.gameObject);
                stationUI = null;
                Viewer = null;
            }
        }

        public bool OpenUI(Settler viewer)
        {
            if (IsViewed) return false;

            if (stationUI == null)
            {
                stationUI = Instantiate(stationUIPrefab, GameController.Instance.WorldCanvas.transform);
                stationUI.transform.SetAsFirstSibling();
                stationUI.Station = this;
                stationUI.Inventory = Inventory;
            }
            stationUI.Active = ShowUI;
            Viewer = viewer;
            // update position in the same frame to prevent UI correction during play
            stationUI.UpdatePosition();
            return true;
        }

        public void OnInventoryChange()
        {
            
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

        public virtual void Assemble(int recipeIndex, int count)
        {
            ItemRecipe recipe = Recipes[recipeIndex];
            for (int i = 0; i < recipe.Ingredients.Length; ++i)
            {
                Ingredient ingredient = recipe.Ingredients[i];
                if (inventory[i] == null || 
                    ingredient.item != inventory[i].ItemType || 
                    ingredient.count * count > inventory[i].count)
                {
                    return;
                }
            }

            for (int i = 0; i < recipe.Ingredients.Length; ++i)
            {
                Ingredient ingredient = recipe.Ingredients[i];
                inventory.ConsumeItem(i, ingredient.count * count);
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
