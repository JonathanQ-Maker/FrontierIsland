using UnityEngine;
using NBT.Tags;
using System.Security.Authentication;

namespace FrontierIsland
{
    public abstract class ItemStack : INBTSerializable, ICopyable<ItemStack>
    {
        private static int itemCount = 0; // Total number of item instances

        public static int ItemCount { get { return itemCount; } }


        private CompoundTag nbt = null;
        public virtual CompoundTag NBT { get { return nbt; } set { nbt = value; } }

        public bool HasNBT { get { return NBT != null; } }

        public virtual int MaxStackSize { get { return 255; } }
        public abstract ItemType ItemType { get; }
        public int count;
        public string name, description;

        private ItemHandler handler;
        public virtual ItemHandler Handler 
        {
            get { return handler; }
            protected set { handler = value; }
        }

        public virtual ItemHandler InstantiateHandler(Vector3 pos, Transform parent)
        {
            ItemHandler prefab = GameController.Instance.ItemHandlerPrefabs[ItemType];

            // if spawning into world
            if (parent == null)
            {
                if (Handler == null || Handler.ToBeDestroyed)
                {
                    ItemHandler handler = Object.Instantiate(prefab, pos, Quaternion.identity);
                    handler.Item = this;
                    Handler = handler;
                }
                else
                {
                    Handler.transform.SetParent(null);
                    Handler.transform.position = pos;
                    Handler.transform.rotation = Quaternion.identity;
                }

                // model is offset to match item holder's pivot. Remove if spawned into world space
                if (Handler.Model != null)
                {
                    Handler.Model.localPosition = Vector3.zero;
                    Handler.Model.localRotation = Quaternion.identity;
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
                if (prefab.Model != null)
                {
                    Handler.Model.localPosition = prefab.Model.localPosition;
                    Handler.Model.localRotation = prefab.Model.localRotation;
                }
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

        public virtual Sprite GetIcon()
        { 
            return GameController.Instance.ItemHandlerPrefabs.GetIcon(ItemType);
        }

        /// <summary>
        /// Combines the stacks
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns><see langword="true"/> if successfully combined the stacks</returns>
        public bool CombineStack(ItemStack other)
        {
            if (Similar(other) && count < MaxStackSize)
            {
                int numAdded = Mathf.Min(MaxStackSize, count + other.count) - count;
                count += numAdded;
                other.count -= numAdded;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Create a new stack of with <paramref name="count"/> subtracted from this <see cref="ItemStack"/>
        /// </summary>
        /// <param name="count"></param>
        /// <returns><see langword="null"/> if cannot split stack</returns>
        public ItemStack SplitStack(int count)
        {
            if (count < this.count)
            {
                ItemStack newStack = DeepClone();
                newStack.count = count;
                this.count -= count;
                return newStack;
            }
            return null;
        }

        public ItemStack(int count, string name, string description)
        {
            this.name           = name;
            this.description    = description;
            this.count          = (byte)count;
            itemCount++;
        }

        ~ItemStack()
        {
            Debug.Log($"Deleting ItemStack {ItemType}");
            itemCount--;

            if (Handler != null && !Handler.ToBeDestroyed)
            {
                throw new System.Exception($"ItemStack finalizer is called but Handler still exists\n" +
                    $"ItemStack: {ToString()}, Handler: {Handler}@{Handler.transform.position}");
            }
        }

        public virtual bool Similar(ItemStack item)
        {
            return ItemType == item.ItemType;
        }

        public virtual string GetToolTip()
        {
            return $"<size=14><color=#C5D4E9>{name}<size=12>\r\n";
        }

        public void ReadFromNBT(CompoundTag nbt)
        {
            throw new System.NotImplementedException();
        }

        public void WriteToNBT(CompoundTag nbt)
        {
            throw new System.NotImplementedException();
        }

        public void CopyTo(ItemStack other)
        {
            other.count = count;
            other.name = name;
            other.description = description;

            if (HasNBT)
            {
                other.nbt = (CompoundTag)NBT.Clone();
            }
        }

        public abstract ItemStack DeepClone();
    }
}