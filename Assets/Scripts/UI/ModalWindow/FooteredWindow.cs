using UnityEngine;
using TMPro;
using UnityEngine.UI;
namespace NS_Game
{
    public class FooteredWindow : ModalWindow
    {
        public Button button1, button2, button3;
        public TextMeshProUGUI buttonText1, buttonText2, buttonText3;
        public delegate void ButtonClick(FooteredWindow footeredWindow);
        public event ButtonClick Button1Click, Button2Click, Button3Click;

        public void OnButton1Click()
        {
            Button1Click?.Invoke(this);
        }

        public void OnButton2Click()
        {
            Button2Click?.Invoke(this);
        }

        public void OnButton3Click()
        {
            Button3Click?.Invoke(this);
        }
    }
}
