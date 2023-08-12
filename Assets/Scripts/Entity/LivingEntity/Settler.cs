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
            set 
            {
                if (inventory != null)
                {
                    inventory.onInventoryChange -= OnInventoryChange;
                }
                inventory = value;
                inventory.onInventoryChange += OnInventoryChange;
                UpdateHeldItem();
            }
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

        private bool showView = true;
        public bool ShowView 
        {
            get { return showView; }
            set 
            {
                showView = value;
                if (viewable != null) viewable.ShowUI = value; 
            } 
        }
        public ItemStack HeldItem { get { return Inventory[HeldItemIndex]; } }

        private ItemHandler heldItemhandler;
        private IViewable viewable;


        public void Start()
        {
            GameController.Instance.settlers.Add(this);
            Inventory = new Inventory(9, 1, this);

            inventory[0] = new TreeCone(99);
            inventory[1] = new WoodLog(99);
            inventory[2] = new Twig(99);
            inventory[3] = new RocksItem(99);


            UpdateHeldItem();
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

            PathRequest newRequest = new PathRequest(Position, Vector3Int.RoundToInt(target), 32, callback);
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
                ItemStack[] drops = block.GetItemDrops();
                if (drops != null)
                {
                    for (int i = 0; i < drops.Length; ++i)
                    {
                        if (!Inventory.AddItem(drops[i]))
                        {
                            DropItem(drops[i]);
                        }
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

            PathRequest newRequest = new PathRequest(Position, block.Position, 32, callback);
            pathRequest = newRequest;
            PathRequestManager.RequestPath(newRequest);
        }
        #endregion

        #region PlaceBlock
        protected virtual IEnumerator PlaceBlock(Vector3Int[] path, Vector3Int targetPos)
        {
            yield return Inspect(path, targetPos);

            if (!Terrain.Instance.CanPlaceBlock(targetPos)) yield break;

            State = AnimState.Harvesting;
            yield return new WaitForSeconds(0.5f);

            if (HeldItem is not BlockItem blockItem || blockItem.count <= 0)
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

            Vector3Int currentPos = Position;
            if (targetPos.x == currentPos.x && targetPos.z == currentPos.z)
            {
                // trying to place block at where Settler is standing
                if (Terrain.Instance.Walkable(currentPos.x, currentPos.z + 1))
                {
                    // can walk north
                    ActionLoop = PlaceBlock(new Vector3Int[] { currentPos + Vector3Int.forward, currentPos}, targetPos);
                }
                else if (Terrain.Instance.Walkable(currentPos.x + 1, currentPos.z))
                {
                    // can walk east
                    ActionLoop = PlaceBlock(new Vector3Int[] { currentPos + Vector3Int.right, currentPos }, targetPos);
                }
                else if (Terrain.Instance.Walkable(currentPos.x, currentPos.z - 1))
                {
                    // can walk south
                    ActionLoop = PlaceBlock(new Vector3Int[] { currentPos + Vector3Int.back, currentPos }, targetPos);
                }
                else if (Terrain.Instance.Walkable(currentPos.x - 1, currentPos.z))
                {
                    // can walk west
                    ActionLoop = PlaceBlock(new Vector3Int[] { currentPos + Vector3Int.left, currentPos }, targetPos);
                }
                return; // cannot find an open spot to move out of the way, exit
            }

            Action<Vector3Int[], bool> callback = (Vector3Int[] path, bool success) =>
            {
                if (path.Length > 0)
                {
                    this.path = path;
                    if (success)
                    {
                        ActionLoop = PlaceBlock(path, targetPos);
                    }
                    else
                    {
                        ActionLoop = TraversePath(path, false);
                    }
                }
            };

            PathRequest newRequest = new PathRequest(Position, targetPos, 32, callback);
            pathRequest = newRequest;
            PathRequestManager.RequestPath(newRequest);
        }
        #endregion

        #region CollectItem
        protected virtual IEnumerator CollectItem(Vector3Int[] path, ItemHandler handler)
        {
            yield return Inspect(path, handler.transform.position);

            if (handler == null || handler.ToBeDestroyed) yield break; // check if collected by another

            if ((Position - handler.transform.position).magnitude > 2)
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

            PathRequest newRequest = new PathRequest(Position, handler.Position, 32, callback);
            pathRequest = newRequest;
            PathRequestManager.RequestPath(newRequest);
        }
        #endregion

        #region UseCraftingBlock
        protected virtual IEnumerator UseCraftingBlock(Vector3Int[] path, CraftingBlock block)
        {
            yield return Inspect(path, block.transform.position);
            if (block == null) yield break;

            Vector3Int delta = Position - block.Position;
            delta.y = 0;
            if (delta.magnitude > 2)
            {
                Debug.LogError("error cannot use blocks this far away");
            }
            View(block);
        }

        public virtual void StartUseCraftingBlock(CraftingBlock block)
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
                    ActionLoop = UseCraftingBlock(path, block);
                }
                else
                {
                    ActionLoop = TraversePath(path, false);
                }
            };

            PathRequest newRequest = new PathRequest(Position, block.Position, 32, callback);
            pathRequest = newRequest;
            PathRequestManager.RequestPath(newRequest);
        }
        #endregion

        public virtual void DropItem(ItemStack itemStack)
        {
            itemStack.InstantiateHandler(Position, null);
        }

        public void OnInventoryChange()
        {
            UpdateHeldItem();
        }

        protected virtual void View(IViewable viewable)
        {
            if (!ReferenceEquals(viewable, this.viewable))
            {
                if (this.viewable != null)
                {
                    this.viewable.CloseUI();
                    this.viewable = null;
                }

                if (viewable.OpenUI(this))
                {
                    this.viewable = viewable;
                    this.viewable.ShowUI = ShowView;
                }
            }
        }

        public virtual void CloseView()
        {
            if (viewable != null)
            {
                viewable.CloseUI();
                viewable = null;
            }
        }

        private void OnDestroy()
        {
            CloseView();
            // unsubscribe from event
            if (Inventory != null)
            {
                Inventory.onInventoryChange -= OnInventoryChange;
            }
        }
    }
}

