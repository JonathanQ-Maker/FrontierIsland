using UnityEngine;
using NBT.Tags;

namespace FrontierIsland
{
    public abstract class Block : MonoBehaviour, INBTSerializable, ISelectable
    {
        public virtual Vector3Int Size { get { return Vector3Int.one; } }

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
        /// Returns new <see cref="ItemStack"/> representing the 
        /// content dropped from breaking this <see cref="Block"/>
        /// </summary>
        /// <returns><see langword="null"/> if no drops </returns>
        public abstract ItemStack[] GetItemDrops();


        protected virtual void OnDrawGizmosSelected()
        {
            Vector3Int origin = Position;
            float x = origin.x - 0.5f, y = origin.y, z = origin.z - 0.5f;
            Gizmos.DrawLine(new Vector3(x, y, z), new Vector3(x, y + Size.y, z));
            Gizmos.DrawLine(new Vector3(x, y + Size.y, z + Size.z), new Vector3(x, y + Size.y, z));
            Gizmos.DrawLine(new Vector3(x, y + Size.y, z + Size.z), new Vector3(x, y, z + Size.z));
            Gizmos.DrawLine(new Vector3(x, y, z + Size.z), new Vector3(x, y, z));

            Gizmos.DrawLine(new Vector3(x, y, z), new Vector3(x + Size.x, y, z));
            Gizmos.DrawLine(new Vector3(x, y + Size.y, z), new Vector3(x + Size.x, y + Size.y, z));
            Gizmos.DrawLine(new Vector3(x, y, z + Size.z), new Vector3(x + Size.x, y, z + Size.z));
            Gizmos.DrawLine(new Vector3(x, y + Size.y, z + Size.z), new Vector3(x + Size.x, y + Size.y, z + Size.z));

            x += Size.x;
            Gizmos.DrawLine(new Vector3(x, y, z), new Vector3(x, y + Size.y, z));
            Gizmos.DrawLine(new Vector3(x, y + Size.y, z + Size.z), new Vector3(x, y + Size.y, z));
            Gizmos.DrawLine(new Vector3(x, y + Size.y, z + Size.z), new Vector3(x, y, z + Size.z));
            Gizmos.DrawLine(new Vector3(x, y, z + Size.z), new Vector3(x, y, z));
        }

        protected virtual void Start()
        {
            // intentionally left blank
        }
    }

    public enum BlockFace
    {
        Top,
        North,
        East,
        South,
        West
    }
}