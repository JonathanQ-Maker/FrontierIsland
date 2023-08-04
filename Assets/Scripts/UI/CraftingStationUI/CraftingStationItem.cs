using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FrontierIsland
{
    public class CraftingStationItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField]
        private Image iconImage;

        [NonSerialized]
        public CraftingStationUI stationUI;
        public RectTransform rectTransform { get { return transform as RectTransform; } }

        public ItemType Item { get; private set; }

        private int index = -1;
        public int Index { get { return index; } }


        /// <summary>
        /// Renders and loads the <see cref="ItemRecipe"/> at <paramref name="index"/> in <see cref="CraftingStationUI.Station"/>
        /// </summary>
        /// <param name="index"></param>
        public void Load(int index)
        {
            Item = stationUI.Station.Recipes[index].ResultItem;
            iconImage.sprite = GameController.Instance.ItemIcons[Item];
            this.index = index;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            string toolTip = ItemAtlas.Get(Item).GetToolTip();
            if (string.IsNullOrEmpty(toolTip)) return;

            GameController.Instance.ToolTipWindow.Active = true;
            GameController.Instance.ToolTipWindow.Text = toolTip;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            GameController.Instance.ToolTipWindow.Active = false;
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            stationUI.SelectIndex = Index;
        }

        private void OnDestroy()
        {
            GameController.Instance.ToolTipWindow.Active = false;
        }

        private void OnDisable()
        {
            GameController.Instance.ToolTipWindow.Active = false;
        }
    }
}
