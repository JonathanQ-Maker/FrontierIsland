

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

        public bool ShowUI
        {
            get { return stationUI == null ? false : stationUI.gameObject.activeSelf; }

            set
            {
                if (stationUI == null)
                {
                    if (!value)
                    {
                        return;
                    }

                    stationUI = Instantiate(stationUIPrefab, GameController.Instance.MainCanvas.transform);
                    stationUI.transform.SetSiblingIndex(0);
                    stationUI.Station = this;
                }

                stationUI.gameObject.SetActive(value);
                stationUI.UpdatePosition();
            }
        }

        public abstract string Title { get; }
        public Transform FocusTransform { get { return transform; } }

        private void OnDestroy()
        {
            // clean up possible ui
            if (stationUI != null) Destroy(stationUI.gameObject);
        }
    }
}
