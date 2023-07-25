using UnityEngine;

namespace FrontierIsland
{
    public abstract class MultiBlock : Block
    { 
        public abstract Vector3Int Size { get; }

        protected virtual void OnDrawGizmosSelected()
        {
            Vector3Int origin = Vector3Int.RoundToInt(transform.position);
            float x = origin.x - 0.5f, y = origin.y, z = origin.z - 0.5f;
            Gizmos.DrawLine(new Vector3(x, y, z),                    new Vector3(x, y + Size.y, z));
            Gizmos.DrawLine(new Vector3(x, y + Size.y, z + Size.z),  new Vector3(x, y + Size.y, z));
            Gizmos.DrawLine(new Vector3(x, y + Size.y, z + Size.z),  new Vector3(x, y, z + Size.z));
            Gizmos.DrawLine(new Vector3(x, y, z + Size.z),           new Vector3(x, y, z));

            Gizmos.DrawLine(new Vector3(x, y, z), new Vector3(x + Size.x, y, z));
            Gizmos.DrawLine(new Vector3(x, y + Size.y, z), new Vector3(x + Size.x, y + Size.y, z));
            Gizmos.DrawLine(new Vector3(x, y, z + Size.z), new Vector3(x + Size.x, y, z + Size.z));
            Gizmos.DrawLine(new Vector3(x, y + Size.y, z + Size.z), new Vector3(x + Size.x, y + Size.y, z + Size.z));

            x += Size.x;
            Gizmos.DrawLine(new Vector3(x, y, z),                    new Vector3(x, y + Size.y, z));
            Gizmos.DrawLine(new Vector3(x, y + Size.y, z + Size.z),  new Vector3(x, y + Size.y, z));
            Gizmos.DrawLine(new Vector3(x, y + Size.y, z + Size.z),  new Vector3(x, y, z + Size.z));
            Gizmos.DrawLine(new Vector3(x, y, z + Size.z),           new Vector3(x, y, z));
        }
    }
}
