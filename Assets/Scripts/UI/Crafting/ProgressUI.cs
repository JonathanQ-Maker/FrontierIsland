using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FrontierIsland
{
    public class ProgressUI : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI title, timeDisplay;

        public string Title 
        { 
            get { return title.text; }
            set { title.text = value; }
        }
        public int TimeDisplay
        {
            set { timeDisplay.text = $"{value}s"; }
        }

        [SerializeField]
        private Slider progressSlider;
        public Slider ProgressSlider { get { return progressSlider; } }

        public bool Active
        {
            get { return gameObject.activeSelf; }
            set 
            {
                gameObject.SetActive(value);
            }
        }

        public delegate void OnAbort();
        public OnAbort onAbort;

        public void OnClickAbort()
        { 
            onAbort();
        }

        DebugTracker tracker;
        private void Start()
        {
            tracker = new DebugTracker("ProgressUI");
        }
    }
}
