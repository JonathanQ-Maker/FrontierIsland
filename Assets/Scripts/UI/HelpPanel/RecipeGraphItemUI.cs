using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FrontierIsland
{
    public class RecipeGraphItemUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {

        [SerializeField]
        private Image iconImage;
        public string toolTip;

        [NonSerialized]
        public RecipeGraphUI graph;
        public RectTransform rectTransform { get { return transform as RectTransform; } }

        public void Load(Sprite icon, string toolTip)
        {
            iconImage.sprite = icon;
            this.toolTip = toolTip;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
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
