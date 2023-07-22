using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace FrontierIsland
{
    public class InventoryItem : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        private Image image;
        [SerializeField]
        private TextMeshProUGUI countDisplay;

        private ItemSlot slot;
        public ItemSlot Slot
        {
            get
            {
                return slot;
            }

            set
            {
                if (value != null)
                {
                    value.AssignInventoryItem(this);
                    if (ItemIndex != -1) // move item in inventory to match slot inventory item is in
                    {
                        ItemStack itemStack = slot.InventoryWindow.Inventory.RemoveItem(ItemIndex);
                        value.InventoryWindow.Inventory.SetItem(value.SlotIndex, itemStack);
                        ItemIndex = value.SlotIndex;
                    }
                }
                else
                {
                    if (value.InventoryWindow.Inventory.Holder != null)
                    {
                        value.InventoryWindow.Inventory.Holder.DropItem(ItemIndex);
                    }
                    else
                    {
                        value.InventoryWindow.Inventory.RemoveItem(ItemIndex);
                    }
                    Destroy(gameObject);
                    return;
                }
                slot = value;
                ResetPosition();
            }
        }

        [SerializeField]
        private int itemIndex = -1;
        public int ItemIndex
        {
            get
            {
                return itemIndex;
            }

            set
            {
                itemIndex = value;
                UpdateContent();
            }
        }

        public ItemStack ItemStack
        {
            get
            {
                return Slot.InventoryWindow.Inventory[ItemIndex];
            }
        }

        private Canvas canvas;

        private void Start()
        {
            Canvas[] canvases = GetComponentsInParent<Canvas>();
            canvas = canvases[canvases.Length - 1]; // topmost canvas
        }

        public void UpdateContent()
        {
            ItemStack item = slot.InventoryWindow.Inventory[itemIndex];
            image.sprite = item.GetIcon();
            if (item.count == 1)
            {
                countDisplay.text = string.Empty;
            }
            else
            {
                countDisplay.text = $"{item.count}";
            }
        }

        public void ResetPosition()
        {
            transform.SetParent(null);
            transform.SetParent(slot.ItemHolder);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            transform.SetParent(canvas.transform);
            transform.SetAsLastSibling();
            image.raycastTarget = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            //(transform as RectTransform).anchoredPosition += eventData.delta / slot.InventoryWindow.Canvas.scaleFactor;
            transform.position = eventData.position;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!RectTransformUtility.RectangleContainsScreenPoint(Slot.InventoryWindow.WindowRect, Input.mousePosition))
            {
                if (Slot.InventoryWindow.Inventory.Holder != null)
                {
                    Slot.InventoryWindow.Inventory.Holder.DropItem(ItemIndex);
                }
                else
                {
                    Slot.InventoryWindow.Inventory.RemoveItem(ItemIndex);
                }
                Destroy(gameObject);
            }
            else
            {
                ResetPosition();
            }
            image.raycastTarget = true;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            //GameManager.instance.toolTip.Active = true;
            //GameManager.instance.toolTip.LoadItemTip(ItemStack);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            //GameManager.instance.toolTip.Active = false;
        }

        private void OnDestroy()
        {
            //GameManager.instance.toolTip.Active = false;
        }

        private void OnDisable()
        {
            //GameManager.instance.toolTip.Active = false;
        }
    }
}
