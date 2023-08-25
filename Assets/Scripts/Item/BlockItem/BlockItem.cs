using UnityEngine;

namespace FrontierIsland
{
    public abstract class BlockItem : ItemStack
    {
        public abstract BlockType BlockType { get; }

        public override bool CanPlaceBlock { get { return true; } }

        public BlockItem(int count, string name, string description) : base(count, name, description)
        {
        }

        /// <summary>
        /// Tries to instantiate <see cref="Block"/> of <see cref="BlockType"/> at <paramref name="pos"/> by <paramref name="settler"/>
        /// </summary>
        /// <param name="item"></param>
        /// <param name="pos"></param>
        /// <returns>
        /// <br>
        /// <see langword="null"/> if <paramref name="pos"/> does not have space for the <see cref="Block"/>
        /// </br>
        /// <br>
        /// Otherwise, return the <see cref="Block"/> placed
        /// </br>
        /// </returns>
        public virtual Block PlaceBlock(Settler settler, Vector3Int pos)
        {
            Block block = Terrain.Instance.PlaceBlock(BlockType, pos);

            if (block != null && HasNBT)
            {
                block.ReadFromNBT(NBT);
            }
            return block;
        }

        public override void OnUsePlaceBlock(Settler user, Vector3Int position, BlockFace blockFace)
        {
            if (PlaceBlock(user, position) != null)
            {
                user.Inventory.ConsumeItem(user.HeldItemIndex, 1);
            }
        }
    }
}