namespace FrontierIsland
{
    public class CarpenterBench : CraftingBlock
    {
        // TODO: display first item in crafting inventory
        public override BlockType BlockType { get { return BlockType.CarpenterBench; } }

        public override ItemRecipe[] Recipes { get { return RecipeCollections.CarpenterBench; } }

        public override string Title { get { return "Carpenter Bench"; } }

        public override ItemStack[] GetItemDrops()
        {
            return new ItemStack[] { new CarpenterBenchItem(1) };
        }
    }
}
