using System.Collections;
using Unity.VisualScripting;
using UnityEngine;


namespace FrontierIsland
{
    public abstract class CraftingBlock : Block, ICraftingStation
    {
        [SerializeField]
        private CraftingStationUI UIPrefab;
        private CraftingStationUI stationUI;

        public override MaterialType MaterialType { get { return MaterialType.None; } }

        public abstract ItemRecipe[] Recipes { get; }

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

        protected bool active = false;
        public bool Functional { get { return active; } }

        public Settler.AnimState UseState { get { return Settler.AnimState.Harvesting; } }

        DebugTracker tracker;
        protected override void Start()
        {
            tracker = new DebugTracker("CrafingBlock");
            base.Start();
            Inventory = CreateInventory();
            active = true;
        }

        protected virtual void OnDestroy()
        {
            // clean up possible ui
            Viewer?.CloseView();
            if (stationUI != null) CloseUI();
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

        public virtual void CloseUI()
        {
            if (stationUI != null)
            {
                Destroy(stationUI.gameObject);
                stationUI = null;
                Viewer = null;
            }
        }

        public virtual bool OpenUI(Settler viewer)
        {
            if (!CanView(viewer)) return false;

            if (stationUI == null)
            {
                stationUI = Instantiate(UIPrefab, GameController.Instance.WorldCanvas.transform);
                stationUI.transform.SetAsFirstSibling();

                stationUI.Init(this);
            }
            stationUI.Active = ShowUI;
            Viewer = viewer;

            // update position in the same frame to prevent UI correction during play
            stationUI.UpdatePosition();
            return true;
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

        public bool CanView(Settler viewer)
        {
            return Viewer == null;
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

        public virtual IEnumerator Crafting(int recipeIndex, int count)
        {
            if (!Functional || stationUI == null)
            {
                yield break;
            }

            ProgressUI progressUI = stationUI.ProgressUI;

            for (int i = 0; i < count; ++i)
            {
                float craftTime = Recipes[recipeIndex].CraftTime;
                float finishTime = Time.time + craftTime;
                int lastTimeDisplay = Mathf.CeilToInt(craftTime);

                // display has to be updated before (yield return null)
                // to avoid displaying previous state for a split second 
                progressUI.Title = $"{ItemAtlas.Get(Recipes[recipeIndex].ResultItem).name} {i}/{count}";
                progressUI.ProgressSlider.value = 0;
                progressUI.TimeDisplay = lastTimeDisplay;

                while (finishTime > Time.time)
                {
                    yield return null;

                    // check after yield return null to make sure
                    // for code in the bottom that all conditions 
                    // in the following is true
                    if (!Functional || stationUI == null)
                    {
                        yield break;
                    }

                    float timeLeft = finishTime - Time.time;
                    progressUI.ProgressSlider.value = (craftTime - timeLeft) / craftTime;

                    // optimization to reduce string garbage
                    int newTimeDisplay = Mathf.CeilToInt(timeLeft);
                    if (newTimeDisplay != lastTimeDisplay)
                    {
                        progressUI.TimeDisplay = newTimeDisplay;
                        lastTimeDisplay = newTimeDisplay;
                    }
                }
                Craft(recipeIndex, 1);
            }
            stationUI.OnFinishCrafting();
        }

        public virtual void OnCraft(int recipeIndex, int count)
        {
            if (Recipes[recipeIndex].Match(Inventory, count))
            {
                Viewer.StartUsing(Crafting(recipeIndex, count), this);
                stationUI.CraftingUI.Active = false;
                stationUI.ProgressUI.Active = true;
            }
        }

        public virtual void OnAbort()
        {
            Viewer.StopAction();
        }
    }
}
