using UnityEngine;
using UnityEngine.EventSystems;

namespace FrontierIsland
{
    public class NoZoom : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public int flagIndex;

        public void OnPointerEnter(PointerEventData eventData)
        {
            CameraManager.Instance.scrollFlag |= 1 << flagIndex;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            CameraManager.Instance.scrollFlag &= ~(1 << flagIndex);
        }
    }
}
