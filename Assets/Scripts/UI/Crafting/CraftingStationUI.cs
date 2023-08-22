using System;
using UnityEngine;

namespace FrontierIsland
{
    public class CraftingStationUI : WorldUI
    {

        [SerializeField]
        private ProgressUI progressUI;
        public ProgressUI ProgressUI { get { return progressUI; } }

        [SerializeField]
        private CraftingUI craftingUI;
        public CraftingUI CraftingUI { get { return craftingUI; } }
        private ICraftingStation station;

        public void Init(ICraftingStation station)
        {
            this.station = station;
            craftingUI.Init(station.Title, station.Recipes, station.Inventory);
            focus = station.transform;
            craftingUI.onCraft = station.OnCraft;
            progressUI.onAbort = OnAbort;
        }

        public void OnAbort()
        {
            OnFinishCrafting();
            station.OnAbort();
        }

        public void OnFinishCrafting()
        {
            progressUI.Active = false;
            craftingUI.Active = true;
        }

        DebugTracker tracker;
        private void Start()
        {
            tracker = new DebugTracker("CraftingStationUI");
        }
    }
}
