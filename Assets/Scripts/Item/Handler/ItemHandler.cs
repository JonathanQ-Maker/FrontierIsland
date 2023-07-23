using UnityEngine;

namespace FrontierIsland
{
    public abstract class ItemHandler : MonoBehaviour, ISelectable
    {
        private bool toBeDestroyed = false;
        public bool ToBeDestroyed { get { return toBeDestroyed; } }

        public Vector3Int Position 
        {
            get { return Vector3Int.FloorToInt(transform.position + modelTransform.localPosition); }
        }

        private ItemStack item;
        public virtual ItemStack Item
        {
            get { return item; }
            set 
            {
                item = value;
                if (value == null)
                {
                    Destruct();
                }
            }
        }

        [SerializeField]
        private Transform modelTransform;
        public Transform ModelTransform { get { return modelTransform; } }

        [SerializeField]
        private new Collider collider;
        public Collider Collider { get { return collider; } }

        private void Start()
        {
            if (Item == null)
            {
                Debug.LogError("ItemHandler cannot exit without ItemStack");
                Destruct();
            }
        }

        public virtual void OnSelect()
        {
            
        }

        /// <summary>
        /// Sets ToBeDestroyed flag when Destroying this handler, prevents race conditions
        /// </summary>
        public void Destruct()
        {
            toBeDestroyed = true;
            Destroy(gameObject);
        }
    }
}