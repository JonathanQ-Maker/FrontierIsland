using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace FrontierIsland
{
    public class ToolTipWindow : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI toolTip;

        public Vector2 offset = new Vector3(5, 5);

        public bool Active 
        { 
            get { return gameObject.activeSelf; }
            set 
            { 
                gameObject.SetActive(value);
            }
        }

        public void LoadItemTip(ItemStack itemStack)
        {
            toolTip.text = itemStack.GetToolTip();
            LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
        }

        private void SetPosition(Vector2 position)
        {
            RectTransform rectTransform = transform as RectTransform;
            rectTransform.position = position + offset;
        }

        private void Update()
        {
            SetPosition(Input.mousePosition);
        }

        private void Start()
        {
            gameObject.SetActive(false);
        }
    }
}
