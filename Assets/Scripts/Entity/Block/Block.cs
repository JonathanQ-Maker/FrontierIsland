using UnityEngine;
using NBT.Tags;

namespace FrontierIsland
{
    public abstract class Block : MonoBehaviour, INBTSerializable, ISelectable
    {
        [SerializeField]
        private Transform model;

        public Transform Model { get { return model; } }

        /// <summary>
        /// Block material identifier for tool efficiency calculation
        /// </summary>
        public abstract MaterialType MaterialType { get; }

        /// <summary>
        /// Type identifier
        /// </summary>
        public abstract BlockType BlockType { get; }

        /// <summary>
        /// How long it takes in seconds to break this block
        /// </summary>
        public virtual float Hardness { get { return 3f; } }

        /// <summary>
        /// Can <see cref="LivingEntity"/> move through this block
        /// </summary>
        public virtual bool Solid { get { return true; } }

        public Vector3Int Position 
        {
            get 
            {
                return Vector3Int.RoundToInt(transform.position);
            }
        }

        public virtual void OnSelect()
        {
            
        }

        public virtual void ReadFromNBT(CompoundTag nbt)
        {
            
        }

        public virtual void WriteToNBT(CompoundTag nbt)
        {

        }

        /// <summary>
        /// Returns a new <see cref="ItemStack"/> representing the 
        /// content dropped from breaking this <see cref="Block"/>
        /// </summary>
        /// <returns><see langword="null"/> if no drops </returns>
        public abstract ItemStack GetItemDrop();
    }
}