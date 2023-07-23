using UnityEngine;
using NBT.Tags;

namespace FrontierIsland
{
    public abstract class Block : MonoBehaviour, INBTSerializable<CompoundTag>, ISelectable
    {
        /// <summary>
        /// Block material identifier for tool efficiency calculation
        /// </summary>
        public virtual MaterialType MaterialType { get; }

        /// <summary>
        /// Type identifier
        /// </summary>
        public virtual BlockType BlockType { get; }

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
                return Vector3Int.FloorToInt(transform.position);
            }
        }

        public virtual void DeserializeNBT(CompoundTag tag)
        {

        }

        public virtual void OnSelect()
        {
            //throw new System.NotImplementedException();
        }

        public virtual CompoundTag SerializeNBT()
        {
            CompoundTag tag = new CompoundTag();
            tag.PutByte("type", (byte)BlockType);
            return tag;
        }
    }
}