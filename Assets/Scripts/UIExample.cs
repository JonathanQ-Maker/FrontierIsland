using UnityEngine;
using UnityEngine.UI;

public class UIExample : MonoBehaviour
{
    public Image linePrfab;
    public Canvas canvas;

    public Image start, end;

    public void CreateLine(Vector2 start, Vector2 end)
    {
        int width = 8;
        Vector2 delta = end - start;
        Vector2 normDelta = delta.normalized;
        float zAngle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg - 90;
        Image line = Instantiate(linePrfab, Vector3.zero, Quaternion.Euler(0, 0, zAngle), canvas.transform);

        // add correction offset
        line.rectTransform.anchoredPosition = start + new Vector2(-normDelta.y, normDelta.x) * width / 2f;
        line.rectTransform.sizeDelta = new Vector2(width, Mathf.FloorToInt(delta.magnitude / width) * width);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            Debug.Log($"Mouse Pos: {Input.mousePosition}");

            Vector2 pos = Input.mousePosition / canvas.scaleFactor;
            CreateLine(new Vector2(200, 200), pos);
        }
    }
}
