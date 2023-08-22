using TMPro;
using UnityEngine;

public class DebugDisplay : MonoBehaviour
{
    public TextMeshProUGUI displayText;
    void Update()
    {
        displayText.text =
            $"FPS: {(int)(1 / Time.smoothDeltaTime)}\n" +
            $"Screen Size: {Screen.width}x{Screen.height}\n" +
            $"Debug Trackers: {DebugTracker.Count}";
    }
}
