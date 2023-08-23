

using UnityEngine;
using static FrontierIsland.Inventory;

namespace FrontierIsland
{
    public abstract class ViewableBlock : Block, IViewable
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

        protected virtual void OnDestroy()
        {
            // clean up possible ui
            Viewer?.CloseView();
            if (ui != null) CloseUI();
        }
    }
}
