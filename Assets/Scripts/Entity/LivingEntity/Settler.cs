using System;
using System.Collections;
using UnityEngine;

namespace FrontierIsland
{
    public class Settler : LivingEntity, IInventoryHolder
    {
        public enum AnimState
        {
            Idle = 0,
            Walking = 1,
            Harvesting = 2
        }

        [SerializeField]
        private Transform rightHandPivot;
        protected Transform RightHandPivot { get { return rightHandPivot; } }

        [SerializeField]
        private Transform leftHandPivot;
        protected Transform LeftHandPivot { get { return leftHandPivot; } }

        [SerializeField]
        private Animator animator;
        public Animator Animator
        {
            get { return animator; }
        }

        public override LivingEntityType EntityType
        {
            get { return LivingEntityType.Settler; }
        }

        public virtual AnimState State
        {
            get { return (AnimState)animator.GetInteger("state"); }
            set { animator.SetInteger("state", (int)value); }
        }

        protected override IEnumerator ActionLoop
        {
            get => base.ActionLoop;
            set
            {
                State = AnimState.Idle;
                base.ActionLoop = value;
            }
        }

        private Inventory inventory;
        public virtual Inventory Inventory 
        {
            get { return inventory; }
            set { inventory = value; }
        }

        private int heldItemIndex;
        public virtual int HeldItemIndex
        {
            get { return heldItemIndex; }
            set 
            {
                heldItemIndex = value;
                UpdateHeldItem();
            }
        }

        public ItemStack HeldItem { get { return Inventory[HeldItemIndex]; } }

        private ItemHandler heldItemhandler;


        public void Start()
        {
            if (animator == null)
                Debug.LogError("Missing animator");

            inventory = new Inventory(9, 1, this);
            inventory[0, 0] = new RocksItem(255);
            inventory[1, 0] = new WoodAxe();
            inventory[2, 0] = new StoneAxe();
            inventory[3, 0] = new TreeItem(255, Tree.TreeState.Normal);
            inventory[4, 0] = new BinItem(255);
            inventory[5, 0] = new GrassItem(255);
            inventory[6, 0] = new CampFireItem(255);
            inventory[7, 0] = new MushroomsItem(255);
            inventory[8, 0] = new CrateItem(255);
        }

        protected void UpdateHeldItem()
        {
            ItemStack heldItem = Inventory[HeldItemIndex];
            if (heldItemhandler != null)
            {
                if (heldItem == null || !ReferenceEquals(heldItemhandler, heldItem.Handler))
                {
                    heldItemhandler.Destruct();

                    // although Destroy() will set this to null, maunally setting to null allows GC to collect it
                    heldItemhandler = null;
                }
            }

            if (heldItem != null)
            {
                heldItemhandler = heldItem.InstantiateHandler(Vector3.zero, RightHandPivot);
            }
        }

        protected override IEnumerator MoveTo(Vector3Int targetPos, float maxSpeed)
        {
            Vector3 delta = new Vector3(targetPos.x, transform.position.y, targetPos.z) - transform.position;
            if (delta.magnitude < 0.1f) yield break;
            State = AnimState.Walking;
            yield return base.MoveTo(targetPos, maxSpeed);
            State = AnimState.Idle;
        }

        #region Inspect
        protected virtual IEnumerator Inspect(Vector3Int[] path, Vector3 target)
        {
            yield return TraversePath(path, false);
            yield return LookAt(target);
        }

        public virtual void StartInspect(Vector3 target)
        {
            if (pathRequest != null)
            {
                pathRequest.Cancel();
            }

            Action<Vector3Int[], bool> callback = (Vector3Int[] path, bool success) =>
            {
                if (path.Length > 0)
                {
                    this.path = path;
                    ActionLoop = Inspect(path, target);
                }
            };

            PathRequest newRequest = new PathRequest(Vector3Int.RoundToInt(transform.position), Vector3Int.RoundToInt(target), 32, callback);
            pathRequest = newRequest;
            PathRequestManager.RequestPath(newRequest);
        }
        #endregion

        #region HarvestBlock
        protected virtual float GetHarvestTime(Block block)
        {
            float efficiency = 1f;
            if (HeldItem != null && HeldItem is ToolItem)
            {
                efficiency = ((ToolItem)HeldItem).GetEfficiency(block);
            }
            return block.Hardness / efficiency;
        }

        protected virtual IEnumerator HarvestBlock(Vector3Int[] path, Block block)
        {
            yield return Inspect(path, block.transform.position);
            if (block == null) yield break;

            Vector3Int delta = Position - block.Position;
            delta.y = 0;
            if (delta.magnitude > 2)
            {
                Debug.LogError("error cannot harvest blocks this far away");
            }
            State = AnimState.Harvesting;

            float finishTime = GetHarvestTime(block) + Time.time;
            ItemStack heldItem = HeldItem;
            while (finishTime > Time.time)
            {
                if (!ReferenceEquals(HeldItem, heldItem) || block == null)
                {
                    State = AnimState.Idle;
                    yield break;
                }
                yield return null;
            }

            if (block != null) // is null if destroyed by another
            {
                // Break block and handle item drops from block
                ItemStack drops = block.GetItemDrop();
                if (drops != null)
                {
                    if (!Inventory.AddItem(drops))
                    {
                        drops.InstantiateHandler(block.Position, null);
                    }
                }
                Terrain.Instance.DestroyBlock(block);
            }
            State = AnimState.Idle;
        }

        public virtual void StartHarvestBlock(Block block)
        {
            if (pathRequest != null)
            {
                pathRequest.Cancel();
            }

            Action<Vector3Int[], bool> callback = (Vector3Int[] path, bool success) =>
            {
                if (path.Length > 0)
                {
                    this.path = path;
                    if (success)
                    {
                        ActionLoop = HarvestBlock(path, block);
                    }
                    else
                    {
                        ActionLoop = TraversePath(path, false);
                    }
                }
            };

            PathRequest newRequest = new PathRequest(Vector3Int.RoundToInt(transform.position), block.Position, 32, callback);
            pathRequest = newRequest;
            PathRequestManager.RequestPath(newRequest);
        }
        #endregion

        #region PlaceBlock
        protected virtual IEnumerator PlaceBlock(Vector3Int[] path)
        {
            Vector3Int targetPos = path[path.Length - 1];
            yield return Inspect(path, targetPos);

            State = AnimState.Harvesting;
            yield return new WaitForSeconds(0.5f);

            BlockItem blockItem = HeldItem as BlockItem;
            if (blockItem == null || blockItem.count <= 0)
            {
                State = AnimState.Idle;
                yield break;
            }
            if (Terrain.Instance.PlaceBlockItem(blockItem, targetPos) != null)
            {
                Inventory.ConsumeItem(HeldItemIndex, 1);
            }
            State = AnimState.Idle;
        }

        public virtual void StartPlaceBlock(Vector3Int targetPos)
        {

            if (pathRequest != null)
            {
                pathRequest.Cancel();
            }

            Action<Vector3Int[], bool> callback = (Vector3Int[] path, bool success) =>
            {
                if (path.Length > 0)
                {
                    this.path = path;
                    if (success)
                    {
                        ActionLoop = PlaceBlock(path);
                    }
                    else
                    {
                        ActionLoop = TraversePath(path, false);
                    }
                }
            };

            PathRequest newRequest = new PathRequest(Vector3Int.RoundToInt(transform.position), targetPos, 32, callback);
            pathRequest = newRequest;
            PathRequestManager.RequestPath(newRequest);
        }
        #endregion

        #region CollectItem
        protected virtual IEnumerator CollectItem(Vector3Int[] path, ItemHandler handler)
        {
            yield return Inspect(path, handler.transform.position);

            if (handler == null || handler.ToBeDestroyed) yield break; // check if collected by another

            if ((Vector3Int.RoundToInt(transform.position) - handler.transform.position).magnitude > 2)
            {
                Debug.LogError("error");
            }
            State = AnimState.Harvesting;
            yield return new WaitForSeconds(0.25f);

            if (handler != null && !handler.ToBeDestroyed && handler.transform.parent == null) // check if picked up by another
            {
                inventory.CollectItem(handler.Item);
            }
            State = AnimState.Idle;
        }

        public virtual void StartCollectItem(ItemHandler handler)
        {
            if (pathRequest != null)
            {
                pathRequest.Cancel();
            }

            Action<Vector3Int[], bool> callback = (Vector3Int[] path, bool success) =>
            {
                this.path = path;
                if (success)
                {
                    ActionLoop = CollectItem(path, handler);
                }
                else
                {
                    ActionLoop = TraversePath(path, false);
                }
            };

            PathRequest newRequest = new PathRequest(Vector3Int.RoundToInt(transform.position), handler.Position, 32, callback);
            pathRequest = newRequest;
            PathRequestManager.RequestPath(newRequest);
        }
        #endregion

        public virtual void DropItem(ItemStack item)
        {
            item.InstantiateHandler(Position, null);
        }

        public void OnInventoryChange(int index)
        {
            UpdateHeldItem();
        }
    }
}

