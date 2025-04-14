using UnityEngine;
using NBT.Tags;

namespace FrontierIsland
{
    /*
     * =======================================
     *  How to make an item in FrontierIsland
     * =======================================
     * 
     * 1. Add item type to ItemType enum
     * 
     * 2. Create a concrete class inheriting from ItemStack
     * 
     * 3. Create a concrete class inheriting from ItemHandler (optional)
     * 
     * 4. Create a prefab in Editor with ItemHandler attached to the 
     * top-most gameobject and a child gameobject called "Model" which 
     * holds the mesh of the ItemHandler. Make sure the top-most gameobject 
     * has zero position, identity rotation
     * and the prefab has a collider
     * 
     * 5. Adjust ItemHandler's Model transform such that it displays
     * properly when attached to settler's hand pivots 
     * 
     * 6. link ItemHandler prefab to ItemHandlerPrefabs
     * ScriptibleObject and link icon to ItemIcons ScriptibleObject
     * 
     * 7. Add master ItemStack instance to ItemAtlas
     */

    public abstract class ItemStack : INBTSerializable, ICopyable<ItemStack>
    {
        private CompoundTag nbt = null;
        public virtual CompoundTag NBT { get { return nbt; } set { nbt = value; } }

        public bool HasNBT { get { return NBT != null; } }

        public virtual int MaxStackSize { get { return 256; } }
        public abstract ItemType ItemType { get; }

        public int count;
        public string name, description;

        private ItemHandler handler;
        public virtual ItemHandler Handler 
        {
            get { return handler; }
            protected set { handler = value; }
        }

        /// <summary>
        /// Can this <see cref="ItemStack"/> be used to place block
        /// </summary>
        /// <param name="on"></param>
        /// <returns></returns>
        public virtual bool CanPlaceBlock { get { return false; } }

        public virtual int FuelValue { get { return 0; } }

        #region InstanceTracker
        private static int instanceCount;
        public static int InstanceCount { get { return instanceCount; } }

        ~ItemStack()
        {
            --instanceCount;
            Debug.Log($"[ItemStack]: Deleted \"{name}\", {InstanceCount} left");
        }
        #endregion
        public ItemStack(int count, string name, string description)
        {
            this.name           = name;
            this.description    = description;
            this.count          = count;

            ++instanceCount;
        }

        /// <summary>
        /// Instantiate the corresponding <see cref="ItemHandler"/> to this <see cref="ItemStack"/> to <paramref name="pos"/>
        /// <br></br>
        /// <br>
        /// NOTE1: If an <see cref="ItemHandler"/> already exists will instead move to <paramref name="pos"/>
        /// </br>
        /// <br>
        /// NOTE2: If <paramref name="parent"/> is null, will reset <see cref="ItemHandler.Model"/> transform. For the offset on 
        /// </br>
        /// <br>
        /// <see cref="ItemHandler.Model"/> is intended for when held by <see cref="Settler"/>
        /// </br>
        /// <br>NOTE3: If <paramref name="parent"/> is not null, <paramref name="pos"/> is in 
        /// localspace and <see cref="ItemHandler.Collider"/> is disabled</br>
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
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
            Handler.transform.localPosition = pos;
            Handler.transform.localRotation = Quaternion.identity;
            Handler.Collider.enabled = false;
            return Handler;
        }

        /// <summary>
        /// Destroy the <see cref="ItemHandler"/> using <see cref="ItemHandler.Destruct()"/>
        /// </summary>
        public void RemoveHandler()
        {
            if (Handler != null)
            {
                // Sets ToBeDestroyed flag when Destroying this handler, prevents race conditions
                Handler.Destruct();
                handler = null;
            }
        }

        /// <summary>
        /// Get the Icon image corresponding to this <see cref="ItemStack"/>
        /// </summary>
        /// <returns></returns>
        public virtual Sprite GetIcon()
        { 
            return GameController.Instance.ItemIcons[ItemType];
        }

        /// <summary>
        /// Combines the stacks
        /// </summary>
        /// <param name="other"></param>
        /// <returns><see langword="true"/> if successfully combined at least one item from <paramref name="other"/></returns>
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
        /// Add <paramref name="count"/> number of items to this stack from <paramref name="other"/>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns><see langword="true"/> if successfully added <paramref name="count"/> to this stack</returns>
        public bool AddFrom(ItemStack other, int count)
        {
            if (Similar(other) && (this.count + count) <= MaxStackSize && other.count >= count)
            {
                other.count -= count;
                this.count += count;
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

        

        public virtual bool Similar(ItemStack other)
        {
            if (HasNBT)
            {
                if (other.HasNBT)
                { 
                    return ItemType == other.ItemType && other.NBT.Equals(NBT);
                }
                return false;
            }
            return other != null && ItemType == other.ItemType;
        }

        public virtual string GetToolTip()
        {
            return $"<size=14><color=#C5D4E9>{name}<size=12>\n" +
                   (FuelValue > 0 ? $"Fuel Value: {FuelValue}\n" : "") +
                   $"<color=#898989><i>{description}</i></color>\n";
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

        /// <summary>
        /// <br>
        /// Efficiency of breaking blocks that the <see cref="ItemStack"/> is effective against.
        /// </br>
        /// <br>
        /// harvest time = hardness / efficiency
        /// </br>
        /// </summary>
        public virtual float EfficiencyEffective { get { return 1; } }

        /// <summary>
        /// Is <see cref="ItemStack"/> effective at breaking <paramref name="block"/>
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        public virtual bool IsEffective(Block block)
        {
            return false;
        }

        /// <summary>
        /// Called when used to place block
        /// </summary>
        /// <param name="user"></param>
        /// <param name="position"></param>
        /// <param name="blockFace"></param>
        public virtual void OnUsePlaceBlock(Settler user, Vector3Int position, BlockFace blockFace)
        {
            // intentionally left blank
        }

        public virtual ItemStack[] HarvestBlock(Block block)
        {
            // safe to destroy first then call GetItemDrops()
            // because destroy happens at end of frame
            Terrain.Instance.DestroyBlock(block);
            return block.GetItemDrops();
        }
    }
}