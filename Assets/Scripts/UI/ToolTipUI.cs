using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace FrontierIsland
{
    public class ToolTipUI : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI toolTip;
        [SerializeField]
        private Canvas canvas;

        public RectTransform rectTransform { get { return transform as RectTransform; } }

        public string Text 
        { 
            get { return toolTip.text; } 
            set { toolTip.text = value; } 
        }

        public bool Active 
        { 
            get { return gameObject.activeSelf; }
            set 
            {
                SetPosition(Input.mousePosition);
                gameObject.SetActive(value);
                //Cursor.visible = !gameObject.activeSelf;
            }
        }

        public void LoadItemTip(ItemStack itemStack)
        {
            Text = itemStack.GetToolTip();
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        }

        private void SetPosition(Vector2 position)
        {
            rectTransform.position = position;
            UpdatePivot();
        }

        private void UpdatePivot()
        {
            Vector2 topRight = rectTransform.anchoredPosition + rectTransform.sizeDelta;
            Vector2 currentPivot = rectTransform.pivot;
            if (topRight.x > Screen.width / canvas.scaleFactor)
            {
                currentPivot.x = 1;
            }
            else
            {
                currentPivot.x = 0;
            }

            if (topRight.y > Screen.height / canvas.scaleFactor)
            {
                currentPivot.y = 1;
            }
            else
            {
                currentPivot.y = 0;
            }
            rectTransform.pivot = currentPivot;
        }

        private void Update()
        {
            SetPosition(Input.mousePosition);
        }

        private void Start()
        {
            // make sure tooltip start off as hidden
            gameObject.SetActive(false);
        }
    }
}
