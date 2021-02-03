using UnityEngine;

public class Line : MonoBehaviour
{
    [SerializeField]
    LineRenderer rend;

    public void Apply(Vector3 start, Vector3 end)
    {
        start.z = end.z = 0;

        rend.SetPosition(0, start);
        rend.SetPosition(1, end);

        rend.startWidth = rend.endWidth = DrawManager.lineWidth;
    }
}
