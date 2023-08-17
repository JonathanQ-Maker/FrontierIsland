using UnityEngine;

namespace FrontierIsland
{
    public abstract class Foliage : Block
    {
        public override MaterialType MaterialType { get { return MaterialType.None; } }

        public override bool Solid { get { return false; } }

        public override float Hardness { get { return 0.5f; } }

        protected override void Start()
        {
            base.Start();
            RandomRotate();
        }

        private void RandomRotate()
        {
            float noise = 359f + 89f * transform.position.x + 239f * transform.position.z;
            Model.rotation = Quaternion.Euler(0, noise % 360, 0);
        }
    }
}