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

        public void Start()
        {
            if (animator == null)
                Debug.LogError("Missing animator");

            inventory = new Inventory(10, 1, this);
            inventory[0, 0] = new WoodAxe();
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

            PathRequest newRequest = new PathRequest(Vector3Int.FloorToInt(transform.position), Vector3Int.FloorToInt(target), 32, callback);
            pathRequest = newRequest;
            PathRequestManager.RequestPath(newRequest);
        }
        #endregion

        #region HarvestBlock
        protected virtual IEnumerator HarvestBlock(Vector3Int[] path, Block block)
        {
            yield return Inspect(path, block.transform.position);
            if (block == null) yield break;

            if ((Vector3Int.FloorToInt(transform.position) - block.Position).magnitude > 2)
            {
                Debug.LogError("error cannot harvest blocks this far away");
            }
            State = AnimState.Harvesting;
            yield return new WaitForSeconds(block.Hardness);

            if (block != null) // is null if destroyed by another
                Terrain.Instance.DestroyBlock(block);
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

            PathRequest newRequest = new PathRequest(Vector3Int.FloorToInt(transform.position), block.Position, 32, callback);
            pathRequest = newRequest;
            PathRequestManager.RequestPath(newRequest);
        }
        #endregion

        #region CollectItem
        protected virtual IEnumerator CollectItem(Vector3Int[] path, ItemHandler handler)
        {
            yield return Inspect(path, handler.transform.position);

            if (handler == null) yield break; // check if collected by another

            if ((Vector3Int.FloorToInt(transform.position) - handler.transform.position).magnitude > 2)
            {
                Debug.LogError("error");
            }
            State = AnimState.Harvesting;
            yield return new WaitForSeconds(0.5f);

            if (handler != null && handler.transform.parent == null) // check if picked up by another
            {
                handler.Item.AddToInventory(Inventory);
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

            PathRequest newRequest = new PathRequest(Vector3Int.FloorToInt(transform.position), handler.Position, 32, callback);
            pathRequest = newRequest;
            PathRequestManager.RequestPath(newRequest);
        }
        #endregion

        public virtual void OnSetItem(ItemStack newStack, ItemStack oldStack, int index)
        {
            
        }

        public virtual void DropItem(int index)
        {
            Inventory.DropItem(index, Position);
        }
    }
}

