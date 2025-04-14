using UnityEngine;

namespace FrontierIsland
{
    [CreateAssetMenu(fileName = "FluidMaterials", menuName = "FrontierIsland/FluidMaterials", order = 1)]
    public class FluidMaterials : ScriptableObject
    {
        [SerializeField]
        private Material[] sceneMaterials, uiMaterials;

        /// <summary>
        /// Gets the material assigned to Scene objects
        /// </summary>
        /// <param name="fluidType"></param>
        /// <returns></returns>
        public Material GetSceneMaterial(FluidType fluidType)
        { 
            return sceneMaterials[(int)fluidType];
        }

        /// <summary>
        /// Gets the material assigned to UI objects
        /// </summary>
        /// <param name="fluidType"></param>
        /// <returns></returns>
        public Material GetUIMaterial(FluidType fluidType)
        {
            return uiMaterials[(int)fluidType];
        }
    }
}
