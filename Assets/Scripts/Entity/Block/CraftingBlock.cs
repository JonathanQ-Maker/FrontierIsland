

using Unity.VisualScripting;
using UnityEngine;

namespace FrontierIsland
{
    public abstract class CraftingBlock : Block, ICraftingStation
    {
        [SerializeField]
        private CraftingStationUI stationUIPrefab;
        private CraftingStationUI stationUI;

        public override MaterialType MaterialType { get { return MaterialType.None; } }

        public abstract ItemRecipe[] Recipes { get; }

        public bool IsViewed
        {
            get { return Viewer != null; }
        }

        public abstract string Title { get; }

        public Settler Viewer { get; protected set; }

        private bool showUI = true;
        public bool ShowUI 
        { 
            get { return showUI; }
            set 
            { 
                showUI = value;
                if (stationUI != null)
                    stationUI.Active = ShowUI;
            }
        }

        public Inventory Inventory { get { return inventory; } }

        private Inventory inventory;

        private void Start()
        {
            inventory = new Inventory(4, 2, this);
        }

        private void OnDestroy()
        {
            // clean up possible ui
            if (stationUI != null) CloseUI();
        }

        public virtual void CloseUI()
        {
            if (stationUI != null)
            {
                Destroy(stationUI.gameObject);
                stationUI = null;
                Viewer = null;
            }
        }

        public bool OpenUI(Settler viewer)
        {
            if (IsViewed) return false;

            if (stationUI == null)
            {
                stationUI = Instantiate(stationUIPrefab, GameController.Instance.WorldCanvas.transform);
                stationUI.transform.SetAsFirstSibling();
                stationUI.Station = this;
                stationUI.Inventory = Inventory;
            }
            stationUI.Active = ShowUI;
            Viewer = viewer;
            // update position in the same frame to prevent UI correction during play
            stationUI.UpdatePosition();
            return true;
        }

        public void OnInventoryChange(int index)
        {
            
        }

        public void DropItem(ItemStack itemStack)
        {
            // TODO: implement dropping items
            Debug.LogWarning($"CraftingBlock DropItem() not implemented");
        }
    }
}
