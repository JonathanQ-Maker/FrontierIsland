namespace FrontierIsland
{
    public class CarpenterBench : CraftingBlock
    {

        public override BlockType BlockType { get { return BlockType.CarpenterBench; } }

        public override ItemRecipe[] Recipes { get { return RecipeCollections.CarpenterBench; } }

        public override string Title { get { return "Carpenter Bench"; } }

        public override ItemStack GetItemDrop()
        {
            return new CarpenterBenchItem(1);
        }
    }
}
