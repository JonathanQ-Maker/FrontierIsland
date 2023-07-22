
using UnityEngine;
using NBT.Tags;

namespace FrontierIsland
{
    public abstract class ItemStack : INBTSerializable<CompoundTag>
    {
        private static int itemCount = 0; // Total number of item instances

        public static int ItemCount { get { return itemCount; } }


        public virtual int MaxStackSize { get { return 255; } }
        public virtual ItemType ItemType { get; }
        public byte count;

        private ItemHandler handler;
        public virtual ItemHandler Handler 
        {
            get { return handler; }
            protected set { handler = value; }
        }

        public ItemHandler InstantiateHandler(Vector3 pos, Transform parent)
        {
            if (Handler == null)
            {
                ItemHandler prefab = GameController.Instance.ItemHandlerPrefabs[ItemType];
                ItemHandler handler = Object.Instantiate(prefab, pos, Quaternion.identity, parent);
                handler.Item = this;
                Handler = handler;
                return handler;
            }
            Handler.transform.position = pos;
            Handler.transform.SetParent(parent);
            return Handler;
        }

        public bool RemoveHandler()
        {
            if (Handler != null)
            {
                Object.Destroy(handler.gameObject);
                return true;
            }
            return false;
        }

        public Sprite GetIcon()
        { 
            return GameController.Instance.ItemHandlerPrefabs.GetIcon(ItemType);
        }

        public ItemStack(int count)
        {
            this.count = (byte)count;
            itemCount++;
        }

        ~ItemStack()
        {
            itemCount--;
        }

        public virtual bool Similar(ItemStack item)
        {
            return ItemType == item.ItemType;
        }

        /// <summary>
        /// Places <see cref="ItemStack"/> in first available slot in inventory and return index.
        /// 
        /// <br>
        /// NOTE: If no avialable slot is found, return -1
        /// </br>
        /// </summary>
        /// <param name="item"></param>
        /// <returns>index where the item is added, -1 if no slot is available</returns>
        public int AddToInventory(Inventory inventory)
        {
            RemoveHandler();
            return inventory.AddItem(this);
        }

        public void DeserializeNBT(CompoundTag tag)
        {
            throw new System.NotImplementedException();
        }

        public CompoundTag SerializeNBT()
        {
            throw new System.NotImplementedException();
        }
    }
}