using System;
using System.Collections;
using UnityEngine;

namespace FrontierIsland
{
    public class Settler : LivingEntity
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

        public void Start()
        {
            if (animator == null)
                Debug.LogError("Missing animator");
        }

        protected override IEnumerator MoveTo(Vector3Int targetPos, float maxSpeed)
        {
            Vector3 delta = new Vector3(targetPos.x, transform.position.y, targetPos.z) - transform.position;
            if (delta.magnitude < 0.1f) yield break;
            State = AnimState.Walking;
            yield return base.MoveTo(targetPos, maxSpeed);
            State = AnimState.Idle;
        }

        protected virtual IEnumerator Inspect(Vector3Int[] path, Block block)
        {
            yield return TraversePath(path);
            yield return LookAt(block.transform.position);
            State = AnimState.Harvesting;
        }

        public virtual void StartInspect(Block block)
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
                    ActionLoop = Inspect(path, block);
                }
            };

            PathRequest newRequest = new PathRequest(Vector3Int.FloorToInt(transform.position), block.Position, 32, callback);
            pathRequest = newRequest;
            PathRequestManager.RequestPath(newRequest);
        }

        protected virtual IEnumerator HarvestBlock(Vector3Int[] path, Block block)
        {
            yield return Inspect(path, block);
            State = AnimState.Harvesting;
            yield return new WaitForSeconds(block.Hardness);
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
                    ActionLoop = HarvestBlock(path, block);
                }
            };

            PathRequest newRequest = new PathRequest(Vector3Int.FloorToInt(transform.position), block.Position, 32, callback);
            pathRequest = newRequest;
            PathRequestManager.RequestPath(newRequest);
        }
    }
}

