using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class DrawManager : MonoBehaviour
{
    public GameObject dotPrefab, linePrefab;

    public MarkedPoint markedPoint;

    public Transform graphUI;

    public Transform dotParent, lineParent;

    public Sprite enabledSprite, disabledSprite;

    public Image drawLinesImage, drawDotsImage;

    static List<Line> lines = new List<Line>();
    static List<Dot> dots = new List<Dot>();
    static List<Vector2> dotPoints = new List<Vector2>();

    public static Vector2 Center { get { return new Vector2(minX + maxX, 0) / 2; } }
    public static float Size { get { return (maxX - minX) / 2; } }

    public static float minX = -100;
    public static float maxX = 100;
    public static float MinY { get { return Center.y - Size; } }
    public static float MaxY { get { return Center.y + Size; } }

    static float ScreenScale { get { return Mathf.Min(Screen.width, Screen.height) / (Size * 2); } }

    static Camera Cam { get { return Camera.main; } }
    
    public static float drawDelta = 1;
    public static float dotSize = 1;
    public static float lineWidth = 0.01f;

    public static DrawManager instance;

    public static bool DrawingDots
    {
        get { return instance.drawDotsImage.sprite == instance.enabledSprite; }
        set { instance.drawDotsImage.sprite = value ? instance.enabledSprite : instance.disabledSprite; }
    }

    public static bool DrawingLines
    {
        get { return instance.drawLinesImage.sprite == instance.enabledSprite; }
        set { instance.drawLinesImage.sprite = value ? instance.enabledSprite : instance.disabledSprite; }
    }

    void Awake()
    {
        instance = this;

        DrawingDots = true;
        DrawingLines = false;
    }

    void Start()
    {
        Draw();
    }

    public static void Draw()
    {
        instance.graphUI.transform.localScale = Vector3.one;
        instance.graphUI.GetComponent<RectTransform>().sizeDelta = Vector2.one * Size * 2;

        DrawDots();
        DrawLines();
        
        instance.graphUI.transform.localScale = Vector3.one * ScreenScale;
    }

    public static void DrawDots()
    {
        if (!DrawingDots) return;

        RemoveDots();

        for(double x = minX; x < maxX; x += drawDelta)
        {
            double y = LogicManager.function(x);

            if (y < MinY || y > MaxY || y == double.NaN) continue;

            var clone = Instantiate(instance.dotPrefab, instance.dotParent);

            var dot = clone.GetComponent<Dot>();

            float pointX = (float)x * instance.graphUI.transform.localScale.x;
            float pointY = (float)y * instance.graphUI.transform.localScale.x;

            dot.Apply(pointX, pointY);

            dots.Add(dot);
            dotPoints.Add(new Vector2(pointX, pointY) * ScreenScale + new Vector2(Screen.width, Screen.height) / 2);
        }

        InputManager.instance.MarkPoint();
    }

    public static void DrawLines()
    {
        if (!DrawingLines) return;

        RemoveLines();

        for (int i = 1; i < dotPoints.Count; i++)
        {
            Vector3 startPoint = dotPoints[i - 1];
            Vector3 endPoint = dotPoints[i];

            var clone = Instantiate(instance.linePrefab, instance.lineParent);

            var line = clone.GetComponent<Line>();

            Vector3 start = Cam.ScreenToWorldPoint(startPoint);
            Vector3 end = Cam.ScreenToWorldPoint(endPoint);

            line.Apply(start, end);

            lines.Add(line);
        }
    }

    public static void RemoveDots()
    {
        foreach(var dot in dots)
        {
            Destroy(dot.gameObject);
        }

        dots.Clear();
        dotPoints.Clear();
    }

    public static void RemoveLines()
    {
        foreach (var line in lines)
        {
            Destroy(line.gameObject);
        }

        lines.Clear();
    }

    public static void ToggleDots()
    {
        DrawingDots = !DrawingDots;

        if (dots.Count > 0) RemoveDots();
        else DrawDots();
    }

    public static void ToggleLines()
    {
        DrawingLines = !DrawingLines;

        if (lines.Count > 0) RemoveLines();
        else DrawLines();
    }

    public static void MarkPoint(double x, double y)
    {
        if (!instance.markedPoint.gameObject.activeSelf) instance.markedPoint.gameObject.SetActive(true);

        instance.markedPoint.ApplyPosition(x, y);
    }
}
