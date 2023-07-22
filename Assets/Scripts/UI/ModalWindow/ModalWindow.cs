using UnityEngine;
using TMPro;

namespace NS_Game
{
    public class ModalWindow : MonoBehaviour
    {
        public TextMeshProUGUI HeaderText;
        public RectTransform WindowRect;
        public Canvas Canvas { get; private set; }
        public virtual string Header
        {
            get
            {
                return HeaderText.text;
            }

            set
            {
                HeaderText.text = value;
            }
        }

        public virtual bool Active
        {
            get
            {
                return gameObject.activeSelf;
            }

            set
            {
                gameObject.SetActive(value);
            }
        }

        protected virtual void Awake()
        {
            Canvas = GetComponentInParent<Canvas>();
        }
    }
}
