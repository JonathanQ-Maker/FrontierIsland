using System.Collections;
using UnityEngine;

namespace FrontierIsland
{
    public abstract class ContainerBlock : Block, IViewable, IInventoryHolder
    {
        public Settler Viewer { get; protected set; }

        [SerializeField]
        private WorldUI UIPrefab;
        protected WorldUI ui;


        private bool showUI = true;
        public bool ShowUI
        {
            get { return showUI; }
            set
            {
                showUI = value;
                if (ui != null)
                    ui.Active = ShowUI;
            }
        }

        public abstract string Title { get; }

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
                if (value != null)
                {
                    actionLoop = AutoCleanUp(value);
                }
                else
                {
                    actionLoop = value;
                }
                if (actionLoop != null)
                    StartCoroutine(actionLoop);
            }
        }

        /// <summary>
        /// Wrapper for ActionLoop coroutines that 
        /// cleans up after coroutine has exited
        /// </summary>
        /// <param name="coroutine"></param>
        /// <returns></returns>
        private IEnumerator AutoCleanUp(IEnumerator coroutine)
        {
            yield return coroutine;
            ActionLoop = null;
        }

        protected override void Start()
        {
            base.Start();
            Inventory = CreateInventory();
        }

        protected virtual Inventory CreateInventory()
        {
            return null;
        }

        protected virtual void OnInventoryChange()
        {
            // intentionally left empty
        }

        public virtual bool CanView(Settler viewer)
        {
            return Viewer == null;
        }

        public virtual void CloseUI()
        {
            if (ui != null)
            {
                Destroy(ui.gameObject);
                ui = null;
                Viewer = null;
            }
        }

        public bool OpenUI(Settler viewer)
        {
            if (!CanView(viewer)) return false;

            if (ui == null)
            {
                ui = Instantiate(UIPrefab, GameController.Instance.WorldCanvas.transform);
                ui.transform.SetAsFirstSibling();
                InitUI();
            }
            ui.Active = ShowUI;
            Viewer = viewer;

            // update position in the same frame to prevent UI correction during play
            ui.UpdatePosition();
            return true;
        }

        /// <summary>
        /// Called after the UI is first instantiated
        /// </summary>
        protected virtual void InitUI()
        { 
            // intentionally left empty
        }

        public virtual void DropItem(ItemStack itemStack)
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

        protected virtual void OnDestroy()
        {
            // clean up possible ui
            Viewer?.CloseView();
            if (ui != null) CloseUI();
            Inventory.onInventoryChange -= OnInventoryChange;
        }
    }
}
