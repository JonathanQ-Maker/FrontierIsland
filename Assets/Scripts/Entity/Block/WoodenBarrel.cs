using System.Collections;
using UnityEngine;

namespace FrontierIsland
{
    public class WoodenBarrel : CraftingBlock
    {
        public override MaterialType MaterialType
        {
            get
            {
                return MaterialType.Wood;
            }
        }

        public override BlockType BlockType
        {
            get { return BlockType.WoodenBarrel; }
        }

        public const int INPUT_INDEX = 0, OUTPUT_INDEX = 1, FLUID_OUTPUT_INDEX = 2;
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
        public ItemStack FluidOutputItem
        {
            get { return Inventory[FLUID_OUTPUT_INDEX]; }
            set { Inventory[FLUID_OUTPUT_INDEX] = value; }
        }

        public override ItemRecipe[] Recipes { get { return RecipeCollections.WoodenBarrel; } }

        public override string Title { get { return "Wooden Barrel"; } }

        public WoodenBarrelUI UI { get { return (WoodenBarrelUI)ui; } }

        public int maxProgress = 50, fluidCapacity = 2000;
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
                if (UI != null) UI.ProgressDisplay = (float)progress / maxProgress;
            }
        }

        protected Fluid fluid;

        protected ItemStack processItem;

        [SerializeField]
        protected MeshRenderer contentDisplay;



        public void UpdateFluid()
        {
            if (fluid != null && fluid.amount <= 0)
            {
                fluid = null;
            }

            if (fluid != null)
            {
                Vector3 scale = contentDisplay.transform.localScale;
                scale.y = (float)fluid.amount / fluidCapacity;
                contentDisplay.transform.localScale = scale;
                contentDisplay.sharedMaterial = GameController.Instance.FluidMaterials.GetSceneMaterial(fluid.FluidType);
            }
            else
            {
                Vector3 scale = contentDisplay.transform.localScale;
                scale.y = 0;
                contentDisplay.transform.localScale = scale;
            }

            // update UI
            if (UI != null)
            {
                if (fluid != null)
                {
                    UI.FluidDisplay = (float)fluid.amount / fluidCapacity;
                    UI.FluidType = fluid.FluidType;
                }
                else
                { 
                    UI.FluidDisplay = 0;
                }
            }
        }

        protected override Inventory CreateInventory()
        {
            return new Inventory(3, 1, this);
        }

        public override ItemStack[] GetItemDrops()
        {
            return new ItemStack[] { new WoodenBarrelItem(1) };
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

        protected override void InitUI()
        {
            UI.Init(transform, Inventory);
            Progress = progress;
            UpdateFluid();
        }

        protected override void OnInventoryChange()
        {
            base.OnInventoryChange();
            if (!ReferenceEquals(processItem, InputItem))
            {
                Progress = 0;
                ActionLoop = null;
            }
            TryProcess();


            processItem = InputItem;
        }

        public void TryProcess()
        {
            if (ActionLoop == null)
            {
                int recipeIndex = FindRecipeMatch();
                if (recipeIndex > -1)
                {
                    if (CanProcess(recipeIndex, 1))
                    {
                        ActionLoop = Process(recipeIndex);
                        return;
                    }
                }
            }
        }

        protected bool CanProcess(int recipeIndex, int count)
        {
            ItemRecipe recipe = Recipes[recipeIndex];
            return (OutputItem == null
                || (OutputItem.count + count <= OutputItem.MaxStackSize
                && OutputItem.Similar(ItemAtlas.Get(recipe.ResultItem))))
                && recipe.Match(Inventory, count);
        }

        protected IEnumerator Process(int recipeIndex)
        {
            while (true)
            {
                ++Progress;
                if (Progress >= maxProgress)
                {
                    Progress = 0;
                    OnProcessComplete();
                    if (!CanProcess(recipeIndex, 1))
                    {
                        yield break;
                    }
                }
                yield return new WaitForSeconds(0.1f);
            }
        }

        protected virtual void OnProcessComplete()
        {
            Craft(FindRecipeMatch(), 1);
        }

        public override void Craft(int recipeIndex, int count)
        {
            ItemRecipe recipe = Recipes[recipeIndex];
            if (!CanProcess(recipeIndex, count))
            {
                return;
            }

            // generate fluid
            if (fluid == null)
            {
                fluid = new Lechate(100);
            }
            else if (fluid.amount < fluidCapacity)
            {
                fluid.amount = Mathf.Min(fluid.amount + 100, fluidCapacity);
            }
            UpdateFluid();

            // consume and make item
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

        protected override void Start()
        {
            base.Start();
            UpdateFluid();

        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            processItem = null;
        }
    }
}