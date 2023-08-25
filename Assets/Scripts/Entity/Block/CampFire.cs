using System.Collections;
using UnityEditor.MPE;
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
        public int cookStages = 50;
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

        private float fuel = 1;
        /// <summary>
        /// Amount of fuel left [0, 1]
        /// </summary>
        public float Fuel 
        { 
            get { return fuel; }
            set 
            {
                fuel = value;
                fireLight.intensity = value * maxIntensity;
                Ignited = Fuel > 0;
                if (UI != null) UI.FuelDisplay = value;
            }
        }

        // TODO: allow bar to be full
        private float progress;
        /// <summary>
        /// Current cook progress [0, 1]
        /// </summary>
        public float Progress
        {
            get { return progress; }
            set 
            {
                progress = value;
                if (UI != null) UI.ProgressDisplay = value;
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
            UI.FuelDisplay = Fuel;
            UI.ProgressDisplay = Progress;
        }

        public void AddFuel()
        {
            if (FuelItem != null && FuelItem.IsFuel && Fuel < 0.9f)
            {
                Inventory.ConsumeItem(FUEL_INDEX, 1);
                Fuel += 0.5f;
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
                    }
                }
            }
        }

        protected IEnumerator Cook(int recipeIndex)
        {
            while (Fuel > 0)
            {
                Progress += 1f / cookStages;
                Fuel -= 0.005f;
                if (Progress >= 1)
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

            ItemStack result = ItemAtlas.Get(recipe.ResultItem).DeepClone();
            result.count = count;
            if (OutputItem == null)
            {
                OutputItem = result;
            }
            else
            {
                OutputItem.CombineStack(result);
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

        DebugTracker tracker;
        protected override void Start()
        {
            base.Start();
            tracker = new DebugTracker("Camp Fire");
        }
    }
}