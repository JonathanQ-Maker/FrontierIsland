using UnityEngine;

namespace FrontierIsland
{
    public abstract class ItemHandler : MonoBehaviour, ISelectable
    {
        public Vector3Int Position 
        {
            get { return Vector3Int.FloorToInt(transform.position); }
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
                    Destroy(gameObject);
                }
            }
        }

        private void Start()
        {
            if (Item == null)
            {
                Debug.LogError("ItemHandler cannot exit without ItemStack");
                Destroy(gameObject);
            }
        }

        public virtual void OnSelect()
        {
            
        }
    }
}