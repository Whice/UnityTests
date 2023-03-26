using UnityEngine;

public class StartBorderPositionSetter : MonoBehaviour
{
    public static void SetStartBorders(StartBorderPosition borderPosition, RectTransform rectTransform)
    {
        // Set the anchors
        float x = 0;
        float y = 0;
        switch (borderPosition)
        {
            case StartBorderPosition.top:
                {
                    x = 0.5f;
                    y = 1f;
                    rectTransform.pivot = new Vector2(0.5f, 1f);
                    break;
                }
            case StartBorderPosition.bottom:
                {
                    x = 0.5f;
                    y = 0f;
                    rectTransform.pivot = new Vector2(0.5f, 0f);
                    break;
                }
            case StartBorderPosition.left:
                {
                    x = 0f;
                    y = 0.5f;
                    rectTransform.pivot = new Vector2(0f, 0.5f);
                    break;
                }
            case StartBorderPosition.right:
                {
                    x = 1f;
                    y = 0.5f;
                    rectTransform.pivot = new Vector2(1f, 0.5f);
                    break;
                }
                {
                    x = 0f;
                    y = 0.5f;
                    rectTransform.pivot = new Vector2(0f, 0.5f);
                    break;
                }
            case StartBorderPosition.center:
                {
                    x = 0.5f;
                    y = 0.5f;
                    rectTransform.pivot = new Vector2(0.5f, 0.5f);
                    break;
                }
        }
        rectTransform.anchorMin = new Vector2(x, y);
        rectTransform.anchorMax = new Vector2(x, y);

        // Set the position
        rectTransform.anchoredPosition = new Vector2(0, 0);
    }
}
