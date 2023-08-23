using System;
using UnityEngine;

namespace FrontierIsland
{
    public class CraftingStationUI : WorldUI
    {
        [SerializeField]
        private CraftingUI craftingUI;

        public void Init(ICraftingStation station)
        {
            craftingUI.Init(station.Title, station.Recipes, station.Inventory);
            focus = station.transform;
            craftingUI.onCraft = station.Craft;
        }

        DebugTracker tracker;
        private void Start()
        {
            tracker = new DebugTracker("CraftingStationUI");
        }
    }
}
