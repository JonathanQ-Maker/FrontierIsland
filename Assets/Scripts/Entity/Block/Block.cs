using UnityEngine;
using NBT.Tags;

namespace FrontierIsland
{
    public abstract class Block : MonoBehaviour, INBTSerializable<CompoundTag>, ISelectable
    {
        public virtual BlockType BlockType { get; }
        public virtual float Hardness { get { return 1f; } }

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