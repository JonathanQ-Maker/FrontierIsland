using TreeEditor;
using UnityEngine;

namespace FrontierIsland
{
    public class Foliage : Block
    {
        [SerializeField]
        protected Transform model;

        private void Start()
        {
            RandomRotate();
        }

        private void RandomRotate()
        {
            float noise = 359f + 89f * transform.position.x + 239f * transform.position.z;
            model.rotation = Quaternion.Euler(0, noise % 360, 0);
        }
    }
}