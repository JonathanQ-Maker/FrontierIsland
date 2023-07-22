using NBT.Tags;
using System.Numerics;
using UnityEngine;

namespace FrontierIsland
{
    public class Inventory : INBTSerializable<CompoundTag>
    {
        private ItemStack[] items;

        private int columns, rows;
        public int Columns { get { return columns; } }

        public int Rows { get { return rows;} }

        public int Capacity
        {
            get { return items.Length; }
        }

        private IInventoryHolder holder;
        public IInventoryHolder Holder
        {
            get { return holder; }
            set { holder = value; }
        }


        public ItemStack this[int index]
        {
            get { return GetItem(index); }
            set { SetItem(index, value); }
        }

        public ItemStack this[int x, int y]
        {
            get { return GetItem(y * Rows + x); }

            set { SetItem(y * Rows + x, value); }
        }

        public Inventory(int columns, int rows, IInventoryHolder holder)
        {
            this.columns = columns;
            this.rows = rows;
            Holder = holder;
            items = new ItemStack[columns * rows];
        }

        public virtual ItemStack GetItem(int index)
        {
            return items[index];
        }

        public virtual void SetItem(int index, ItemStack itemStack)
        {
            ItemStack oldItem = items[index];
            items[index] = itemStack;
            if (Holder != null)
                Holder.OnSetItem(itemStack, oldItem, index);
        }

        public ItemStack RemoveItem(int index)
        { 
            ItemStack item = items[index];
            items[index] = null;
            if (Holder != null)
                Holder.OnSetItem(null, item, index);
            return item;
        }

        public ItemHandler DropItem(int index, Vector3Int position)
        {
            ItemStack item = RemoveItem(index);
            if (item != null)
            {
                ItemHandler handler = item.Handler;
                if (handler == null)
                {
                    return item.InstantiateHandler(position, null);
                }
                handler.transform.SetParent(null);
                handler.transform.position = position;
                return handler;
            }
            return null;
        }

        /// <summary>
        /// Places <see cref="ItemStack"/> in first available slot and return index.
        /// 
        /// <br>
        /// NOTE: If no avialable slot is found, return -1
        /// </br>
        /// </summary>
        /// <param name="item"></param>
        /// <returns>index where the item is added, -1 if no slot is available</returns>
        public int AddItem(ItemStack item)
        {
            for (int i = 0; i < items.Length; ++i)
            {
                ItemStack currentItem = items[i];
                if (currentItem == null)
                {
                    SetItem(i, item);
                    return i;
                }

                if (currentItem.Similar(item) && currentItem.count + item.count <= currentItem.MaxStackSize)
                {
                    currentItem.count += item.count;
                    return i;
                }
            }
            return -1;
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
