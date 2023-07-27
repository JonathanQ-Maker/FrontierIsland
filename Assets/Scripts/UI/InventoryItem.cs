using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using static UnityEngine.EventSystems.PointerEventData;
using System;
using System.Collections;

namespace FrontierIsland
{
    public class InventoryItem : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        private Image image;
        [SerializeField]
        private TextMeshProUGUI countDisplay;


        private ItemSlot prevSlot, itemSlot;
        public ItemSlot PrevSlot { get { return prevSlot; } }
        public ItemSlot ItemSlot 
        {
            get { return itemSlot; }
            set 
            {
                if (!ReferenceEquals(value, itemSlot) && itemSlot != null)
                    prevSlot = itemSlot;
                itemSlot = value;

                if (itemSlot != null)
                    index = itemSlot.SlotIndex;
            } 
        }

        public int index;

        private ItemStack itemStack;
        public ItemStack ItemStack
        {
            get
            {
                if (ItemSlot != null)
                    return ItemSlot.ItemStack;
                return itemStack;
            }
        }

        private Canvas canvas;
        private GameObject hover;


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
                actionLoop = value;
                if (actionLoop != null)
                    StartCoroutine(actionLoop);
            }
        }

        private void Start()
        {
            Canvas[] canvases = GetComponentsInParent<Canvas>();
            canvas = canvases[canvases.Length - 1]; // topmost canvas
        }

        public void UpdateContent()
        {
            image.sprite = ItemStack.GetIcon();
            if (ItemStack.count == 1)
            {
                countDisplay.text = string.Empty;
            }
            else
            {
                countDisplay.text = $"{ItemStack.count}";
            }
        }

        public void ResetPosition()
        {
            transform.SetParent(null);
            transform.SetParent(ItemSlot.ItemHolder);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (eventData.button != InputButton.Left) return;

            if (Input.GetKey(KeyCode.LeftControl) && ItemStack.count > 1)
            {
                InventoryItem item = Instantiate(ItemSlot.Window.InvItemPrefab, transform.parent);
                item.ItemSlot = ItemSlot;
                ItemSlot.inventoryItem = item;
                itemStack = ItemSlot.ItemStack.SplitStack(Mathf.CeilToInt(ItemStack.count/2f));
                UpdateContent();
            }
            else
            {
                ItemSlot.inventoryItem = null;
                itemStack = ItemSlot.Window.Inventory.RemoveStack(ItemSlot.SlotIndex);
            }
            ItemSlot = null;

            transform.SetParent(canvas.transform);
            transform.SetAsLastSibling();
            image.raycastTarget = false;
            ActionLoop = HandleClick();
            hover = eventData.pointerEnter;
        }

        public void OnDrag(PointerEventData eventData)
        {

            if (eventData.button != InputButton.Left) return;
            
            
            transform.position = eventData.position;
            hover = eventData.pointerEnter;
        }

        private IEnumerator HandleClick()
        {
            while (true)
            {
                if (Input.GetMouseButtonDown(1))
                {
                    if (ItemStack.count > 1)
                    if (hover.TryGetComponent(out InventoryItem other))
                    {
                        if (other.ItemStack.AddFrom(ItemStack, 1))
                        {
                            UpdateContent();
                        }
                    }
                    else if (hover.TryGetComponent(out ItemSlot slot))
                    {
                        if (slot.inventoryItem == null)
                        {
                            slot.Window.Inventory.SetItem(slot.SlotIndex, ItemStack.SplitStack(1));
                            hover = slot.gameObject;
                            UpdateContent();
                        }
                        else
                        {
                            if (slot.inventoryItem.ItemStack.AddFrom(ItemStack, 1))
                            {
                                UpdateContent();
                            }
                        }
                    }
                }
                yield return null;
            }
        }

        // order of op: up, click, drop, end drag
        public void OnEndDrag(PointerEventData eventData)
        {
            if (eventData.button != InputButton.Left) return;
            if (ItemSlot == null)
            {
                //if (window.Inventory.Holder != null)
                //{
                //    window.Inventory.Holder.DropItem(SlotIndex);
                //}
                //else
                //{
                //    window.Inventory.RemoveStack(SlotIndex);
                //}
                Debug.Log("Destroyed inv item");

                if (ItemStack.count > 0)
                {
                    // drop remaining items, then delete item reference
                    PrevSlot.Window.Inventory.Holder?.DropItem(itemStack);
                }
                Destroy(gameObject);
            }
            else
            {
                ResetPosition();
            }

            // safe to set null because ItemStck is
            // either dropped or stored through ItemSlot
            itemStack = null;
            image.raycastTarget = true;
            ActionLoop = null;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (ItemStack == null) return;
            GameController.Instance.ToolTipWindow.Active = true;
            GameController.Instance.ToolTipWindow.LoadItemTip(ItemStack);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            GameController.Instance.ToolTipWindow.Active = false;
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
