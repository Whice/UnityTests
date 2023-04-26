using UnityEngine;

namespace UI.OptimazeScroll
{
    public class StartBorderPositionSetter : MonoBehaviour
    {
        public static void SetStartBorders(StartBorderPosition borderPosition, RectTransform rectTransform, float anchorMin = 0.5f, float anchorMax = 0.5f)
        {
            switch (borderPosition)
            {
                case StartBorderPosition.top:
                    {
                        anchorMax = 1f;
                        break;
                    }
                case StartBorderPosition.bottom:
                    {
                        anchorMax = 0f;
                        break;
                    }
                case StartBorderPosition.left:
                    {
                        anchorMin = 0f;
                        break;
                    }
                case StartBorderPosition.right:
                    {
                        anchorMin = 1f;
                        break;
                    }
                case StartBorderPosition.center:
                    {
                        break;
                    }
            }
            rectTransform.pivot = new Vector2(anchorMin, anchorMax);
            rectTransform.anchorMin = new Vector2(anchorMin, anchorMax);
            rectTransform.anchorMax = new Vector2(anchorMin, anchorMax);

            // Set the position
            rectTransform.anchoredPosition = new Vector2(0, 0);
        }
    }
}