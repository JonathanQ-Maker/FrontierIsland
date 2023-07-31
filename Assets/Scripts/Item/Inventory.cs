using NBT.Tags;
using System;
using System.Collections;
using UnityEngine;

namespace FrontierIsland
{
    public class Inventory : INBTSerializable
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
            items[index] = itemStack;
            if (Holder != null)
            {
                Holder.OnInventoryChange(index);
            }
        }

        public ItemStack RemoveStack(int index)
        { 
            ItemStack item = items[index];
            items[index] = null;
            item.RemoveHandler();
            if (Holder != null)
            {
                Holder.OnInventoryChange(index);
            }
            return item;
        }

        /// <summary>
        /// Places <see cref="ItemStack"/> in first available slot and return index.
        /// </summary>
        /// <param name="item"></param>
        /// <returns><see langword="true"/> if successfully added all items to inventory</returns>
        public bool AddItem(ItemStack item)
        {
            int firstEmptyIndex = -1;
            for (int i = 0; i < items.Length; ++i)
            {
                ItemStack currentItem = items[i];
                if (firstEmptyIndex < 0 && currentItem == null)
                {
                    firstEmptyIndex = i;
                }

                if (currentItem != null && currentItem.CombineStack(item))
                {
                    if (Holder != null)
                        Holder.OnInventoryChange(i);
                    if (item.count <= 0)
                    {
                        return true;
                    }
                }
            }

            if (firstEmptyIndex >= 0)
            {
                SetItem(firstEmptyIndex, item);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Adds <paramref name="item"/> to inventory and removes <see cref="ItemHandler"/> if it is in <see cref="ItemHandler.Dropped"/> state
        /// </summary>
        /// <param name="item"></param>
        public void CollectItem(ItemStack item)
        {
            if (AddItem(item)) // succesfully added all items
            {
                // handler exists in the world as a dropped form
                if (item.Handler != null && item.Handler.Dropped)
                {
                    item.RemoveHandler();
                }
            }
        }

        /// <summary>
        /// Removes <see cref="ItemStack"/> from <paramref name="index"/> and 
        /// instantiate an <see cref="ItemHandler"/> at <paramref name="position"/>
        /// </summary>
        /// <param name="index"></param>
        /// <param name="position"></param>
        /// <returns><see langword="null"/> if target item is <see langword="null"/>, 
        /// otherwise the instantiated <see cref="ItemHandler"/></returns>
        public ItemHandler DropStack(int index, Vector3Int position)
        {
            ItemStack item = RemoveStack(index);
            if (item != null)
            {
                return item.InstantiateHandler(position, null);
            }
            return null;
        }

        /// <summary>
        /// Consumes <paramref name="count"/> items from <see cref="ItemStack"/> at <paramref name="index"/>
        /// 
        /// <br>
        /// NOTE1: if resulting item count equals 0 removes <see cref="ItemStack"/> from inventory and removes <see cref="ItemHandler"/>
        /// </br>
        /// 
        /// <br>
        /// NOTE2: can result in negative count 
        /// </br>
        /// </summary>
        /// <param name="index"></param>
        /// <param name="count"></param>
        public void ConsumeItem(int index, int count)
        {
            ItemStack item = this[index];
            item.count -= count;

            if (item.count <= 0)
            {
                item.RemoveHandler();
                RemoveStack(index);
            }
        }

        public void ReadFromNBT(CompoundTag nbt)
        {
            throw new NotImplementedException();
        }

        public void WriteToNBT(CompoundTag nbt)
        {
            throw new NotImplementedException();
        }
    }
}
