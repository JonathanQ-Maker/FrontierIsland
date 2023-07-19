using NBT.Tags;
using UnityEngine;
using System.Collections;
using System;

namespace FrontierIsland
{
    public abstract class LivingEntity : MonoBehaviour, INBTSerializable<CompoundTag>, ISelectable
    {
        private IEnumerator actionLoop;
        protected virtual IEnumerator ActionLoop
        {
            get { return actionLoop; }
            set
            {
                if (actionLoop != null)
                {
                    StopCoroutine(actionLoop);
                }
                actionLoop = value;
                StartCoroutine(actionLoop);
            }
        }


        [SerializeField]
        private int health, maxHealth;
        public virtual int Health
        {
            get { return health; }
            set
            {
                health = value;
                if (health < 0) health = 0;
            }
        }

        public virtual int MaxHealth
        {
            get { return maxHealth; }
            set
            {
                health = value;
                if (value < 1) maxHealth = 1;
            }
        }

        [SerializeField]
        private float moveSpeed;
        public virtual float MoveSpeed
        {
            get { return moveSpeed; }
            set
            {
                moveSpeed = value;
                if (value < 0) moveSpeed = 0;
            }
        }

        public virtual bool IsDead
        {
            get { return health <= 0; }
        }

        public abstract LivingEntityType EntityType { get; }

        protected PathRequest pathRequest;
        protected Vector3Int[] path;

        public virtual void DeserializeNBT(CompoundTag tag)
        {
            MaxHealth = tag.GetInt("MaxHealth", 1);
            Health = tag.GetInt("Health", 1);
        }

        public virtual CompoundTag SerializeNBT()
        {
            CompoundTag tag = new CompoundTag(2);
            tag.PutInt("MaxHealth", MaxHealth);
            tag.PutInt("Health", Health);
            return tag;
        }

        public void OnSelect()
        {
            //throw new System.NotImplementedException();
        }

        public virtual void StartLookAt(Vector3Int targetPos)
        {
            ActionLoop = LookAt(targetPos);
        }

        protected virtual IEnumerator MoveTo(Vector3Int targetPos, float maxSpeed)
        {
            Vector3 tPos = new Vector3(targetPos.x, transform.position.y, targetPos.z);
            Vector3 velocity = Vector3.zero;
            float angleVelocity = 0;
            Vector3 delta = tPos - transform.position;
            if (delta.magnitude < 0.1f) yield break;

            float angle = 180 + Mathf.Atan2(delta.x, delta.z) * Mathf.Rad2Deg;
            while ((tPos - transform.position).sqrMagnitude > 0.01f || (Mathf.Abs(Mathf.DeltaAngle(angle, transform.eulerAngles.y)) > 10f))
            {
                transform.position = Vector3.SmoothDamp(transform.position, tPos, ref velocity, 0.1f, maxSpeed);
                transform.rotation = Quaternion.Euler(0, Mathf.SmoothDampAngle(transform.eulerAngles.y, angle, ref angleVelocity, 0.1f), 0);
                yield return null;
            }
            transform.position = tPos;
            transform.rotation = Quaternion.Euler(0, angle, 0);
        }

        protected virtual IEnumerator LookAt(Vector3 targetPos)
        {
            Vector3 tPos = new Vector3(targetPos.x, transform.position.y, targetPos.z);
            Vector3 delta = tPos - transform.position;
            float angleVelocity = 0;
            float angle = 180 + Mathf.Atan2(delta.x, delta.z) * Mathf.Rad2Deg;

            while (Mathf.Abs(Mathf.DeltaAngle(angle, transform.eulerAngles.y)) > 10f)
            {
                transform.rotation = Quaternion.Euler(0, Mathf.SmoothDampAngle(transform.eulerAngles.y, angle, ref angleVelocity, 0.1f), 0);
                yield return null;
            }
            transform.rotation = Quaternion.Euler(0, angle, 0);
        }

        public void StartMoveTo(Vector3Int targetPos)
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
                    ActionLoop = TraversePath(path);
                }
            };

            PathRequest newRequest = new PathRequest(Vector3Int.FloorToInt(transform.position), targetPos, 32, callback);
            pathRequest = newRequest;
            PathRequestManager.RequestPath(newRequest);
        }

        protected virtual IEnumerator TraversePath(Vector3Int[] path)
        {
            if (path.Length <= 0) yield break;
            for (int i = 0; i < path.Length - 1; ++i)
            {
                yield return MoveTo(path[i], MoveSpeed);
            }

            // Path finder ignores walkabilty on the end node such that it can support entities
            // pathing to block rather than just pathing to empty space.
            // This means we must check if last waypoint/node is walkable.
            Vector3Int lastWaypoint = path[path.Length - 1];
            if (Terrain.Instance.Walkable(lastWaypoint.x, lastWaypoint.z))
                yield return MoveTo(lastWaypoint, MoveSpeed);
        }

        private void OnDrawGizmos()
        {
            if (path == null) return;
            Vector3 size = new Vector3(0.5f, 0.5f, 0.5f);
            Gizmos.color = Color.yellow;

            if (pathRequest != null)
            Gizmos.DrawCube(pathRequest.start, size);
            Gizmos.color = Color.red;
            foreach (Vector3Int node in path)
            {
                Gizmos.DrawCube(node, size);
            }
        }
    }
}