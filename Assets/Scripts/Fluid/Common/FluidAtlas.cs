using System;
using UnityEngine;

namespace FrontierIsland
{
    public static class FluidAtlas
    {
        private static readonly Fluid[] fluids = new Fluid[Enum.GetValues(typeof(FluidType)).Length];

        private static void RegisterFluid(Fluid fluid)
        {
            fluids[(int)fluid.FluidType] = fluid;
        }



        public static void SetUp()
        {
            // NOTE: 100 units is one bucket worth of fluid
            RegisterFluid(new Water(Bucket.BUCKET_VOLUME));
            RegisterFluid(new Lechate(Bucket.BUCKET_VOLUME));



#if UNITY_EDITOR
            for (int i = 0; i < fluids.Length; ++i)
            {
                if (fluids[i] == null)
                {
                    Debug.LogWarning($"[FluidAtlas]: {(FluidType)i} is missing from FluidAtlas");
                }
            }
#endif
        }
    }
}
