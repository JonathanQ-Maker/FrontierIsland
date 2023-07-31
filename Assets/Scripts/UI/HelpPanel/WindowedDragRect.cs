using UnityEngine;
using UnityEngine.EventSystems;

namespace FrontierIsland
{
    public class WindowedDragRect : MonoBehaviour, IBeginDragHandler, IDragHandler, IScrollHandler
    {
        [SerializeField]
        public float minScale, maxScale, scrollSensitivity;

        [SerializeField]
        private RectTransform content, viewRect;

        public RectTransform Content { get { return content; } }
        public RectTransform ViewRect { get { return viewRect; } }

        private Vector2 dragStartPosition, contentStartPosition;
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Right)
                return;

            dragStartPosition = Vector2.zero;
            contentStartPosition = content.anchoredPosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(viewRect, eventData.position, eventData.pressEventCamera, out dragStartPosition);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Right)
                return;

            Vector2 localCursor;
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(viewRect, eventData.position, eventData.pressEventCamera, out localCursor))
                return;

            Vector2 position = contentStartPosition + (localCursor - dragStartPosition);
            SetPosition(position);
            UpdateBounds();
        }

        public void OnScroll(PointerEventData data)
        {
            Vector3 delta = Vector3.one * data.scrollDelta.y * scrollSensitivity; // 1 for up or -1 for down
            delta += content.localScale;
            float minX = Mathf.Max(minScale, viewRect.sizeDelta.x/content.sizeDelta.x);
            float minY = Mathf.Max(minScale, viewRect.sizeDelta.y/content.sizeDelta.y);
            float min = Mathf.Max(minX, minY);
            content.localScale = new Vector3(Mathf.Clamp(delta.x, min, maxScale), 
                Mathf.Clamp(delta.y, min, maxScale), 
                content.localScale.z);
            SetPosition(content.anchoredPosition);
            UpdateBounds();
        }

        private void SetPosition(Vector2 pos)
        {
            float xmin = Mathf.Min((viewRect.sizeDelta.x - content.sizeDelta.x * content.localScale.x) / 2, 0);
            float xmax = -xmin;

            float ymin = Mathf.Min((viewRect.sizeDelta.y - content.sizeDelta.y * content.localScale.y) / 2, 0);
            float ymax = -ymin;

            Vector2 position = new Vector2(Mathf.Clamp(pos.x, xmin, xmax),
                                   Mathf.Clamp(pos.y, ymin, ymax));
            content.anchoredPosition = position;
        }

        public void UpdateBounds()
        {
            content.sizeDelta = viewRect.sizeDelta * 2 / content.localScale;
        }
    }
}
