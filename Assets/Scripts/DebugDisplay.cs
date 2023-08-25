using FrontierIsland;
using System.Collections;
using TMPro;
using UnityEngine;

public class DebugDisplay : MonoBehaviour
{
    public TextMeshProUGUI displayText;

    private void Start()
    {
        StartCoroutine(Loop());
    }

    IEnumerator Loop()
    {
        while (true)
        {
            displayText.text =
                $"FPS: {(int)(1 / Time.smoothDeltaTime)}\n" +
                $"Screen Size: {Screen.width}x{Screen.height}\n" +
                $"ItemStack Trackers: {ItemStack.InstanceCount - ItemAtlas.Count}\n" +
                $"Debug Trackers: {DebugTracker.Count}\n";
            yield return new WaitForSeconds(0.2f);
        }
    }
}
