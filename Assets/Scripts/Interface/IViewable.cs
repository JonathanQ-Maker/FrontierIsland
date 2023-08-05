using UnityEngine;

namespace FrontierIsland
{
    public interface IViewable
    {
        /// <summary>
        /// <see langword="true"/> if this is being viewed
        /// </summary>
        bool IsViewed { get; }

        Settler Viewer { get; }

        /// <summary>
        /// Tries to open the UI for <paramref name="viewer"/>
        /// </summary>
        /// <param name="viewer"></param>
        /// <returns><see langword="true"/> if UI opened successfully</returns>
        bool OpenUI(Settler viewer);

        /// <summary>
        /// Close this UI if exists
        /// </summary>
        void CloseUI();

        /// <summary>
        /// <see langword="true"/> if ui is shown
        /// </summary>
        bool ShowUI { get; set; }
    }
}
