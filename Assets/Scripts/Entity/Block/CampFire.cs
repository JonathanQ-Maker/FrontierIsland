using System.Collections;
using UnityEngine;

namespace FrontierIsland
{
    public class CampFire : CraftingBlock
    {
        [SerializeField]
        private Transform fireDisplay;
        [SerializeField]
        private Light fireLight;
        public float maxIntensity = 1.5f;
        public int maxProgress = 50, maxFuel = 500;
        protected ItemStack cookItem;

        public const int INPUT_INDEX = 0, OUTPUT_INDEX = 1, FUEL_INDEX = 2;

        public ItemStack InputItem 
        { 
            get { return Inventory[INPUT_INDEX]; }
            set { Inventory[INPUT_INDEX] = value; }
        }
        public ItemStack OutputItem 
        { 
            get { return Inventory[OUTPUT_INDEX]; } 
            set { Inventory[OUTPUT_INDEX] = value; }
        }
        public ItemStack FuelItem 
        { 
            get { return Inventory[FUEL_INDEX]; }
            set { Inventory[FUEL_INDEX] = value; }
        }

        protected CampFireUI UI { get { return (CampFireUI)ui; } }

        public bool Ignited 
        { 
            get { return fireDisplay.gameObject.activeSelf; } 
            set { fireDisplay.gameObject.SetActive(value); }
        }

        private int fuel;
        /// <summary>
        /// Amount of fuel left [0, 1]
        /// </summary>
        public int Fuel 
        { 
            get { return fuel; }
            set 
            {
                fuel = Mathf.Min(value, maxFuel);

                float ratio = (float)value / maxFuel;
                fireLight.intensity = ratio * maxIntensity;
                Ignited = Fuel > 0;
                if (UI != null) UI.FuelDisplay = ratio;

                // set fire scale
                Vector3 scale = fireDisplay.localScale;
                scale.y = ratio;
                fireDisplay.localScale = scale;
            }
        }

        // TODO: allow bar to be full
        private int progress;
        /// <summary>
        /// Current cook progress [0, 1]
        /// </summary>
        public int Progress
        {
            get { return progress; }
            set 
            {
                progress = Mathf.Min(value, maxProgress);
                if (UI != null) UI.ProgressDisplay = (float)value / maxProgress;
            }
        }

        public override MaterialType MaterialType
        {
            get { return MaterialType.Wood; }
        }

        public override BlockType BlockType
        {
            get { return BlockType.CampFire; }
        }

        public override string Title { get { return "Camp Fire"; } }

        public override ItemRecipe[] Recipes { get { return RecipeCollections.CampFire; } }

        public override ItemStack[] GetItemDrops()
        {
            return new ItemStack[] { new CampFireItem(1) };
        }

        protected override Inventory CreateInventory()
        {
            return new Inventory(3, 1, this);
        }

        protected override void InitUI()
        {
            UI.Init(AddFuel, transform, Inventory);
            Fuel = fuel;
            Progress = progress;
        }

        public void AddFuel()
        {
            // only allow add fuel if less than 90% of max fuel
            // to prevent too much over fuel
            if (FuelItem != null && FuelItem.FuelValue > 0 && Fuel < maxFuel * 0.9f)
            {
                Fuel += FuelItem.FuelValue;
                Inventory.ConsumeItem(FUEL_INDEX, 1);
                TryCook();
            }
        }

        /// <summary>
        /// Finds the <see cref="ItemRecipe"/> index that can be crafted
        /// </summary>
        /// <returns></returns>
        private int FindRecipeMatch()
        {
            for (int i = 0; i < Recipes.Length; ++i)
            {
                if (Recipes[i].Match(Inventory, 1)) return i;
            }
            return -1;
        }

        protected override void OnInventoryChange()
        {
            base.OnInventoryChange();
            if (!ReferenceEquals(cookItem, InputItem))
            {
                Progress = 0;
                ActionLoop = null;
            }
            TryCook();

            // IMPORTANT NOTE: cookItem must be
            // updated to avoid memory leak
            cookItem = InputItem;
            
            // DEBUG 
            //string item = cookItem == null ? "null" : cookItem.name;
            //Debug.Log($"cookItem: {item}");
        }

        protected bool CanCook(int recipeIndex, int count)
        {
            ItemRecipe recipe = Recipes[recipeIndex];
            return recipe.Match(Inventory, count) && (OutputItem == null 
                || (OutputItem.count + count <= OutputItem.MaxStackSize
                && OutputItem.Similar(ItemAtlas.Get(recipe.ResultItem))));
        }

        public void TryCook()
        { 
            if (ActionLoop == null && Fuel > 0)
            {
                int recipeIndex = FindRecipeMatch();
                if (recipeIndex > -1)
                {
                    if (CanCook(recipeIndex, 1))
                    {
                        ActionLoop = Cook(recipeIndex);
                        return;
                    }
                }
            }
        }

        protected IEnumerator Cook(int recipeIndex)
        {
            while (Fuel > 0)
            {
                ++Progress;
                --Fuel;
                if (Progress >= maxProgress)
                {
                    Progress = 0;
                    OnCookComplete();
                    if (!CanCook(recipeIndex, 1))
                    {
                        yield break;
                    }
                }
                yield return new WaitForSeconds(0.1f);
            }
        }

        public override void Craft(int recipeIndex, int count)
        {
            ItemRecipe recipe = Recipes[recipeIndex];
            if (!CanCook(recipeIndex, count))
            {
                return;
            }

            for (int i = 0; i < recipe.Ingredients.Length; ++i)
            {
                Ingredient ingredient = recipe.Ingredients[i];
                Inventory.ConsumeItem(i, ingredient.count * count);
            }

            if (OutputItem == null)
            {
                ItemStack result = ItemAtlas.Get(recipe.ResultItem).DeepClone();
                result.count = count;
                OutputItem = result;
            }
            else
            {
                // little cheat to not create
                // new ItemStack instances reducing
                // memory usage
                OutputItem.count += count;
                Inventory.InventoryChanged();
            }
        }

        protected virtual void OnCookComplete()
        {
            Craft(FindRecipeMatch(), 1);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            cookItem = null;
        }

        protected override void Start()
        {
            base.Start();
            Fuel = fuel;
        }
    }
}