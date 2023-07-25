using UnityEngine;

namespace FrontierIsland
{
    public abstract class ItemHandler : MonoBehaviour, ISelectable
    {
        /// <summary>
        /// <see cref="ItemHandler"/> exists as a dropped form in the world
        /// </summary>
        public bool Dropped { get { return transform.parent == null; } }

        private bool toBeDestroyed = false;
        public bool ToBeDestroyed { get { return toBeDestroyed; } }


        public Vector3Int Position 
        {
            get { return Vector3Int.RoundToInt(transform.position); }
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
        private Transform model;
        public Transform Model { get { return model; } protected set { model = value; } }

        [SerializeField]
        private new Collider collider;
        public Collider Collider { get { return collider; } }

        private void Start()
        {
            if (Item == null)
            {
                Debug.LogError("ItemHandler cannot exist without ItemStack");
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