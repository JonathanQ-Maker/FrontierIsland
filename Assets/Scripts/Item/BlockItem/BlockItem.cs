namespace FrontierIsland
{
    public abstract class BlockItem : ItemStack
    {
        public abstract BlockType BlockType { get; }

        public BlockItem(int count, string name, string description) : base(count, name, description)
        {
        }

        public Block GetBlockPrefab()
        { 
            return GameController.Instance.BlockPrefabs[BlockType];
        }

        public override string GetToolTip()
        {
            return base.GetToolTip() + $"<color=#898989><i>{description}</i></color>";
        }
    }
}