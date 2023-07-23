
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
            ItemHandler prefab = GameController.Instance.ItemHandlerPrefabs[ItemType];

            // if spawning into world
            if (parent == null)
            {
                pos -= prefab.ModelTransform.position;
                Quaternion quaternion = Quaternion.Euler(-prefab.ModelTransform.eulerAngles);

                if (Handler == null || Handler.ToBeDestroyed)
                {
                    ItemHandler handler = Object.Instantiate(prefab, pos, quaternion);
                    handler.Item = this;
                    Handler = handler;
                }
                else
                {
                    Handler.transform.SetParent(null);
                    Handler.transform.position = pos;
                    Handler.transform.rotation = quaternion;
                }
                Handler.Collider.enabled = true;
                return Handler;
            }

            // if spawning as a child
            if (Handler == null || Handler.ToBeDestroyed)
            {
                ItemHandler handler = Object.Instantiate(prefab, parent, false);
                handler.Item = this;
                Handler = handler;
            }
            else
            {
                Handler.transform.SetParent(parent);
                Handler.transform.localPosition = pos;
                Handler.transform.localRotation = Quaternion.identity;
            }
            Handler.Collider.enabled = false;
            return Handler;
        }

        public void RemoveHandler()
        {
            if (Handler != null)
            {
                Handler.Destruct();
            }
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
        /// <br>
        /// NOTE: Destroys handler
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