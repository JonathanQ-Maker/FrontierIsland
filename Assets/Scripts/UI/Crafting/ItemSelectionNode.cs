using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FrontierIsland
{
    public class ItemSelectionNode : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public delegate void OnSelect(int index);

        public OnSelect onSelect;

        [SerializeField]
        private Image iconImage;
        public ItemType Item { get; private set; }
        public int Index { get; private set; } 

        /// <summary>
        /// Renders <paramref name="item"/> and register <paramref name="index"/>
        /// </summary>
        /// <param name="index"></param>
        public void Load(ItemType item, int index)
        {
            Index = index;
            Item = item;
            iconImage.sprite = GameController.Instance.ItemIcons[Item];
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            GameController.Instance.ToolTipWindow.Active = true;
            GameController.Instance.ToolTipWindow.LoadItemTip(ItemAtlas.Get(Item));
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            GameController.Instance.ToolTipWindow.Active = false;
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            // not using onSelect?.Invoke() because onSelect
            // should always be filled
            onSelect(Index);
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
