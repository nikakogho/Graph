using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    public Slider dotDeltaSlider, dotSizeSlider, lineWidthSlider, minXSlider, maxXSlider;
    public InputField dotDeltaField, dotSizeField, lineWidthField, minXField, maxXField;

    public InputField markPointInputField;

    string oldMethodString = "x";

    public static InputManager instance;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        dotDeltaSlider.value = DrawManager.drawDelta;
        dotSizeSlider.value = DrawManager.dotSize;
        lineWidthSlider.value = DrawManager.lineWidth;

        minXSlider.value = DrawManager.minX;
        maxXSlider.value = DrawManager.maxX;

        OnDotDeltaChanged(true);
        OnDotSizeChanged(true);
        OnLineWidthChanged(true);
        OnMinXChanged(true);
        OnMaxXChanged(true);
    }

    public void OnFunctionChanged(InputField functionField)
    {
        string methodString = functionField.text;

        if (methodString == oldMethodString) return;

        LogicManager.Function oldFunction = new LogicManager.Function(LogicManager.function);

        if (LogicManager.UpdateFunction(methodString, ref LogicManager.function))
        {
            oldMethodString = methodString;

            DrawManager.Draw();
        }
        else
        {
            LogicManager.function = oldFunction;
            functionField.text = oldMethodString;
        }
    }

    public void OnDotDeltaChanged(bool fromSlider)
    {
        if (fromSlider)
            dotDeltaField.text = string.Format("{00:0.00}", dotDeltaSlider.value);
        else
            dotDeltaSlider.value = float.Parse(dotDeltaField.text);

        DrawManager.drawDelta = dotDeltaSlider.value;
        DrawManager.Draw();
    }

    public void OnDotSizeChanged(bool fromSlider)
    {
        if (fromSlider)
            dotSizeField.text = string.Format("{00:0.0}", dotSizeSlider.value);
        else
            dotSizeSlider.value = float.Parse(dotSizeField.text);

        DrawManager.dotSize = dotSizeSlider.value;
        DrawManager.DrawDots();
    }

    public void OnLineWidthChanged(bool fromSlider)
    {
        if (fromSlider)
            lineWidthField.text = string.Format("{00:0.00}", lineWidthSlider.value);
        else
            lineWidthSlider.value = float.Parse(lineWidthField.text);

        DrawManager.lineWidth = lineWidthSlider.value;
        DrawManager.DrawLines();
    }

    public void OnMinXChanged(bool fromSlider)
    {
        if (fromSlider)
            minXField.text = string.Format("{00:0.00}", minXSlider.value);
        else
            minXSlider.value = float.Parse(minXField.text);
        
        float minX = minXSlider.value;
        
        maxXSlider.minValue = minX;
        DrawManager.minX = minX;
    }

    public void OnMaxXChanged(bool fromSlider)
    {
        if (fromSlider)
            maxXField.text = string.Format("{00:0.00}", maxXSlider.value);
        else
            maxXSlider.value = float.Parse(maxXField.text);

        float maxX = maxXSlider.value;
        
        minXSlider.maxValue = maxX;
        DrawManager.maxX = maxX;
    }

    public void ToggleDots()
    {
        DrawManager.ToggleDots();
    }

    public void ToggleLines()
    {
        DrawManager.ToggleLines();
    }

    public void MarkPoint()
    {
        double x = System.Convert.ToDouble(markPointInputField.text);

        double y = LogicManager.function(x);

        DrawManager.MarkPoint(x, y);
    }
}
