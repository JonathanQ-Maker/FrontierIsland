using System;
using UnityEngine;

namespace FrontierIsland
{
    /// <summary>
    /// An atlas of all items
    /// </summary>
    public static class ItemAtlas
    {
        private static readonly ItemStack[] items = new ItemStack[Enum.GetValues(typeof(ItemType)).Length];

        private static void RegisterItem(ItemStack item)
        {
            items[(int)item.ItemType] = item;
        }

        /// <summary>
        /// Gets the <paramref name="itemType"/> instance in <see cref="ItemAtlas"/>
        /// </summary>
        /// <param name="itemType"></param>
        /// <returns></returns>
        public static ItemStack Get(ItemType itemType)
        {
            return items[(int)itemType];
        }


        /// <summary>
        /// Registers all game items
        /// </summary>
        public static void SetUp()
        {
            RegisterItem(new WoodAxe());
            RegisterItem(new StoneAxe());
            RegisterItem(new StoneItem(1));
            RegisterItem(new CrateItem(1));
            RegisterItem(new TreeItem(1, TreeBlock.TreeStage.Sapling));
            RegisterItem(new GrassItem(1));
            RegisterItem(new MushroomsItem(1));
            RegisterItem(new CampFireItem(1));
            RegisterItem(new BinItem(1));
            RegisterItem(new CarpenterBenchItem(1));
            RegisterItem(new WoodLog(1));
            RegisterItem(new Twig(1));
            RegisterItem(new TreeCone(1));
            RegisterItem(new FistHatchet());
            RegisterItem(new Leaves(1));
            RegisterItem(new TreeSap(1));

#if UNITY_EDITOR
            for (int i = 0; i < items.Length; ++i)
            {
                if (items[i] == null)
                {
                    Debug.LogWarning($"[itemAtlas]: {(ItemType)i} is missing from ItemAtlas");
                }
            }
#endif
        }
    }
}
