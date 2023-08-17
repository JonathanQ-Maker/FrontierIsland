using UnityEngine;

namespace FrontierIsland
{
    public interface IViewable
    {
        /// <summary>
        /// Can this Settler view this
        /// </summary>
        /// <param name="viewer"></param>
        /// <returns></returns>
        bool CanView(Settler viewer);

        Settler Viewer { get; }

        /// <summary>
        /// <br>
        /// Tries to open the UI for <paramref name="viewer"/>
        /// </br>
        /// <br></br>
        /// 
        /// <br>
        /// NOTE: An UI is "Open" if it exists in the game but not necessarily visible,
        /// </br>
        /// <br>
        /// <see cref="IViewable.ShowUI"/> determins if the opened UI is visible
        /// </br>
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
