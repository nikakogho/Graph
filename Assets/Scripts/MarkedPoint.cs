using UnityEngine;
using UnityEngine.UI;

public class MarkedPoint : MonoBehaviour
{
    public Text text;

    public void ApplyPosition(double x, double y)
    {
        if(x < DrawManager.minX || x > DrawManager.maxX || x == double.NaN || y == double.NaN || y < DrawManager.MinY || y > DrawManager.MaxY)
        {
            gameObject.SetActive(false);
            return;
        }

        text.text = "(" + x + "; " + y + ")";

        transform.localPosition = new Vector3((float)x, (float)y);

        transform.localScale = Vector3.one * DrawManager.dotSize / 2;
    }
}
