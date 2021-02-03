using UnityEngine;
using UnityEngine.UI;

public class Dot : MonoBehaviour
{
    [SerializeField]
    Image image;
    
    public void Apply(float x, float y)
    {
        transform.localPosition = new Vector3(x, y);
        transform.localScale = Vector3.one * DrawManager.dotSize;
    }
}
