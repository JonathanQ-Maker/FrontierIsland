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

        protected virtual IEnumerator Inspect(Vector3Int from, Block block)
        {
            yield return MoveTo(from, MoveSpeed);
            yield return LookAt(block.transform.position);
            State = AnimState.Harvesting;
        }

        public virtual void StartInspect(Vector3Int from, Block block)
        {
            ActionLoop = Inspect(from, block);
        }

        protected virtual IEnumerator HarvestBlock(Block block)
        {
            yield return Inspect(block.Position + Vector3Int.forward, block);
            State = AnimState.Harvesting;
            yield return new WaitForSeconds(block.Hardness);
            Terrain.Instance.DestroyBlock(block);
            State = AnimState.Idle;
        }

        public virtual void StartHarvestBlock(Block block)
        {
            ActionLoop = HarvestBlock(block);        
        }
    }
}

