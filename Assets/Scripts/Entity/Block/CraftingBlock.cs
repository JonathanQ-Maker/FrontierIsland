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

        private Inventory inventory;
        public virtual Inventory Inventory
        {
            get { return inventory; }
            set
            {
                if (inventory != null)
                {
                    inventory.onInventoryChange -= OnInventoryChange;
                }
                inventory = value;
                inventory.onInventoryChange += OnInventoryChange;
            }
        }

        private void Start()
        {
            Inventory = new Inventory(4, 2, this);
        }

        private void OnDestroy()
        {
            // clean up possible ui
            if (stationUI != null) CloseUI();
            Inventory.onInventoryChange -= OnInventoryChange;
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
            if (!CanView(viewer)) return false;

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

        public virtual void OnInventoryChange()
        {
            // intentionally left empty
        }

        public void DropItem(ItemStack itemStack)
        {
            if (Viewer == null)
            {
                itemStack.InstantiateHandler(Position, null);
            }
            else
            {
                Viewer.DropItem(itemStack);
            }
        }

        public bool CanView(Settler viewer)
        {
            return Viewer == null;
        }
    }
}
