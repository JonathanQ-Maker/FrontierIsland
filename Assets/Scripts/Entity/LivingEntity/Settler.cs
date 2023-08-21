using System;
using System.Collections;
using UnityEngine;

namespace FrontierIsland
{
    public class Settler : LivingEntity, IInventoryHolder
    {
        public enum AnimState
        {
            Idle        = 0,
            Walking     = 1,
            Harvesting  = 2
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
                if (inventory != null)
                {
                    inventory.onInventoryChange += OnInventoryChange;
                    UpdateHeldItem();
                }
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
                if (settlerUI != null) settlerUI.Active = value;
            } 
        }
        public ItemStack HeldItem { get { return Inventory[HeldItemIndex]; } }

        private ItemHandler heldItemhandler;
        private IViewable viewable;

        [SerializeField]
        private SettlerUI UIPrefab;
        public SettlerUI settlerUI;

        public override int Health
        {
            get => base.Health;
            set
            {
                base.Health = value;
                if (settlerUI != null) 
                    settlerUI.HealthDisplay = Health;
            }
        }

        protected Inventory craftingInventory;

        public void Start()
        {
            GameController.Instance.settlers.Add(this);
            Inventory = new Inventory(9, 1, this);
            craftingInventory = new Inventory(4, 1, this);

            inventory[0] = new TreeCone(99);
            inventory[1] = new WoodLog(99);
            inventory[2] = new Twig(99);
            inventory[3] = new StoneItem(99);
            inventory[4] = new CarpenterBenchItem(1);
            inventory[5] = new FistHatchet();
            inventory[6] = new TreeSap(10);


            UpdateHeldItem();
        }

        protected void UpdateHeldItem()
        {
            ItemStack heldItem = Inventory[HeldItemIndex];
            if (heldItemhandler != null)
            {
                if (heldItem == null || !ReferenceEquals(heldItemhandler, heldItem.Handler))
                {
                    heldItemhandler.Item.RemoveHandler();

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

        public virtual void DropItem(ItemStack itemStack)
        {
            itemStack.InstantiateHandler(Position, null);
        }

        public void OnInventoryChange()
        {
            UpdateHeldItem();
        }

        private void OnDestroy()
        {
            GameController.Instance.settlers.Remove(this);
            CloseView();
            // unsubscribe from event
            if (Inventory != null)
            {
                Inventory.onInventoryChange -= OnInventoryChange;
                Inventory = null;
            }
            craftingInventory = null;
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
            if (HeldItem != null && HeldItem.IsEffective(block))
            {
                return block.Hardness / HeldItem.EfficiencyEffective;
            }
            return block.Hardness;
        }

        protected virtual ItemStack[] HarvestWithHand(Block block)
        {
            // safe to destroy first then call GetItemDrops()
            // because destroy happens at end of frame
            Terrain.Instance.DestroyBlock(block);
            return block.GetItemDrops();
        }

        protected virtual IEnumerator HarvestBlock(Vector3Int[] path, Vector3Int targetPos)
        {
            Block block = Terrain.Instance.GetBlock(targetPos);
            if (block == null) yield break;
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
                yield return null;

                // check after yield return null to make sure
                // for code in the bottom that all conditions 
                // in the following is true
                if (!ReferenceEquals(HeldItem, heldItem) || block == null)
                {
                    State = AnimState.Idle;
                    yield break;
                }
            }

            if (block != null) // is null if destroyed by another
            {
                // Break block and handle item drops from block
                ItemStack[] drops = HeldItem == null ? HarvestWithHand(block) : HeldItem.HarvestBlock(block);
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
            }

            //TODO: deal with instant block harvest animation
            State = AnimState.Idle;
        }

        public virtual void StartHarvestBlock(Vector3Int targetPos)
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
                        ActionLoop = HarvestBlock(path, targetPos);
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

        #region UseHeldItem
        protected virtual IEnumerator UsePlaceBlock(Vector3Int[] path, Vector3Int targetPos, BlockFace blockFace)
        {
            yield return Inspect(path, targetPos);

            if (!Terrain.Instance.CanPlaceBlock(targetPos)) yield break;

            State = AnimState.Harvesting;
            yield return new WaitForSeconds(0.5f);

            if (HeldItem == null)
            {
                State = AnimState.Idle;
                yield break;
            }
            HeldItem.OnUsePlaceBlock(this, targetPos, blockFace);
            State = AnimState.Idle;
        }

        public virtual void StartUsePlaceBlock(Vector3Int targetPos, BlockFace blockFace)
        {
            if (pathRequest != null)
            {
                pathRequest.Cancel();
            }

            Vector3Int currentPos = Position;
            if (targetPos.x == currentPos.x && targetPos.z == currentPos.z)
            {
                // trying to use item at where Settler is standing
                if (Terrain.Instance.Walkable(currentPos.x, currentPos.z + 1))
                {
                    // can walk north
                    ActionLoop = UsePlaceBlock(new Vector3Int[] { currentPos + Vector3Int.forward, currentPos}, targetPos, blockFace);
                }
                else if (Terrain.Instance.Walkable(currentPos.x + 1, currentPos.z))
                {
                    // can walk east
                    ActionLoop = UsePlaceBlock(new Vector3Int[] { currentPos + Vector3Int.right, currentPos }, targetPos, blockFace);
                }
                else if (Terrain.Instance.Walkable(currentPos.x, currentPos.z - 1))
                {
                    // can walk south
                    ActionLoop = UsePlaceBlock(new Vector3Int[] { currentPos + Vector3Int.back, currentPos }, targetPos, blockFace);
                }
                else if (Terrain.Instance.Walkable(currentPos.x - 1, currentPos.z))
                {
                    // can walk west
                    ActionLoop = UsePlaceBlock(new Vector3Int[] { currentPos + Vector3Int.left, currentPos }, targetPos, blockFace);
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
                        ActionLoop = UsePlaceBlock(path, targetPos, blockFace);
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
                // TODO: auto cleanup
            };

            PathRequest newRequest = new PathRequest(Position, handler.Position, 32, callback);
            pathRequest = newRequest;
            PathRequestManager.RequestPath(newRequest);
        }
        #endregion

        #region ViewCraftingBlock
        protected virtual IEnumerator ViewCraftingBlock(Vector3Int[] path, Vector3Int targetPos)
        {
            yield return Inspect(path, targetPos);
            if (Terrain.Instance.GetBlock(targetPos) is CraftingBlock block)
            {

                Vector3Int delta = Position - block.Position;
                delta.y = 0;
                if (delta.magnitude > 2)
                {
                    Debug.LogError("error cannot use blocks this far away");
                }
                View(block);
            }
        }

        public virtual void StartViewCraftingBlock(Vector3Int targetPos)
        {
            if (pathRequest != null)
            {
                pathRequest.Cancel();
            }

            if (ReferenceEquals(viewable, Terrain.Instance.GetBlock(targetPos))) return;

            Action<Vector3Int[], bool> callback = (Vector3Int[] path, bool success) =>
            {
                this.path = path;
                if (success)
                {
                    ActionLoop = ViewCraftingBlock(path, targetPos);
                }
                else
                {
                    ActionLoop = TraversePath(path, false);
                }
            };

            PathRequest newRequest = new PathRequest(Position, targetPos, 32, callback);
            pathRequest = newRequest;
            PathRequestManager.RequestPath(newRequest);
        }
        #endregion

        #region Using
        protected virtual IEnumerator Using(IEnumerator useCoroutine, IUseable useable)
        {
            State = useable.UseState;
            yield return useCoroutine;
            State = AnimState.Idle;
        }

        /// <summary>
        /// Starts crafting with the current <see cref="IViewable"/>
        /// </summary>
        /// <param name="crafting"></param>
        public virtual void StartUsing(IEnumerator useCoroutine, IUseable useable)
        {
            ActionLoop = Using(useCoroutine, useable);
        }
        #endregion

        #region View
        protected virtual void View(IViewable viewable)
        {
            if (!ReferenceEquals(viewable, this.viewable))
            {
                CloseView();
                CloseUI();

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
        #endregion

        #region UI
        public void OpenUI()
        {
            // clear any currently opened view
            CloseView();

            if (settlerUI == null)
            {
                settlerUI = Instantiate(UIPrefab, GameController.Instance.WorldCanvas.transform);
                settlerUI.transform.SetAsFirstSibling();

                settlerUI.Init(name, RecipeCollections.Settler, craftingInventory, transform, Health, MaxHealth, Craft);
            }
            settlerUI.Active = ShowView;

            // update position in the same frame to prevent UI correction during play
            settlerUI.UpdatePosition();
        }

        public void CloseUI()
        {
            if (settlerUI != null)
            {
                Destroy(settlerUI.gameObject);
                settlerUI = null;
            }
        }

        public void Craft(int recipeIndex, int count)
        {
            ItemRecipe recipe = RecipeCollections.Settler[recipeIndex];
            if (!recipe.Match(craftingInventory, count)) return;

            for (int i = 0; i < recipe.Ingredients.Length; ++i)
            {
                Ingredient ingredient = recipe.Ingredients[i];
                craftingInventory.ConsumeItem(i, ingredient.count * count);
            }

            ItemStack result = ItemAtlas.Get(recipe.ResultItem).DeepClone();
            result.count = count;
            if (!Inventory.AddItem(result))
            {
                DropItem(result);
            }
        }
        #endregion
    }
}

