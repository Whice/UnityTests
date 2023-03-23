using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class NewScroolViewLineElement : MonoBehaviour
{
    private HorizontalOrVerticalLayoutGroup layoutGroup;
    private ContentSizeFitter contentSizeFitter;

    private GameObject[] contentElements;
    public void Init(Transform contentContainer, GameObject[] contentElements, bool isVertical, float spacing, RectOffset padding)
    {
        if(isVertical)
        {
            layoutGroup = gameObject.AddComponent<HorizontalLayoutGroup>();
        }
        else
        {
            layoutGroup = gameObject.AddComponent<VerticalLayoutGroup>();
        }
        layoutGroup.childControlHeight = false;
        layoutGroup.childControlWidth = false;
        layoutGroup.childScaleHeight = false;
        layoutGroup.childScaleWidth = false;
        layoutGroup.childForceExpandHeight = false;
        layoutGroup.childForceExpandWidth = false;
        layoutGroup.spacing = spacing;
        layoutGroup.padding = padding;
        contentSizeFitter = GetComponent<ContentSizeFitter>();
        contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;


        transform.SetParent(contentContainer, false);
        this.contentElements = contentElements;
        foreach (GameObject element in contentElements)
        {
            element.transform.SetParent(transform, false);
        }
    }
    public void SetLinePoistion(bool isVecrtical, float newPosition)
    {
        if (isVecrtical)
            transform.localPosition = new Vector3(transform.localPosition.x, -newPosition);
        else
            transform.localPosition = new Vector3(-newPosition, transform.localPosition.y);
    }
    public enum StartBorderPosition
    {
        top = 0,
        bottom = 1,
        left = 2,
        right = 3
    }

    public void SetStartBorders(StartBorderPosition borderPosition)
    {
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();


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
        }
        rectTransform.anchorMin = new Vector2(x, y);
        rectTransform.anchorMax = new Vector2(x, y);

        // Set the position
        rectTransform.anchoredPosition = new Vector2(0, 0);
    }

    
    private bool isDestroied = false;
    public void Destroy()
    {
        if (!isDestroied)
        {
            Destroy(gameObject);
            isDestroied = true;
        }
    }
    public void SetActive(bool isActive)
    {
        if (!isDestroied)
        {
            gameObject.SetActive(isActive);
        }
    }
}
