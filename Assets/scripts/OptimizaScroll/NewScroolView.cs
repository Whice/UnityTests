using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class NewScroolView : MonoBehaviour
{
    [SerializeField] private bool isVertical = true;
    [SerializeField] private int elementsInLineCount = 5;
    [SerializeField] private float spacingBetweenLine = 10;
    [SerializeField] private float spacingInLine = 10;
    [SerializeField] private RectOffset padding = new RectOffset();
    public int allLines = 90;
    private int linesCount = 1;

    [SerializeField] private Transform contentContainer = null;
    [SerializeField] private Transform viewPortTransform = null;
    [SerializeField] private GameObject contentElement = null;
    [SerializeField] private NewScroolViewLineElement lineElementTemplate = null;
    private ScrollRect scrollRect;

    private List<NewScroolViewLineElement> lineElements = new List<NewScroolViewLineElement>();
    private float contentElementSize
    {
        get
        {
            RectTransform contentRectTransform = contentElement.GetComponent<RectTransform>();
            return isVertical ? contentRectTransform.sizeDelta.y : contentRectTransform.sizeDelta.x;
        }
    }
    private float viewPortSize
    {
        get
        {
            RectTransform rectTransform = viewPortTransform.GetComponent<RectTransform>();
            return isVertical ? rectTransform.rect.height : rectTransform.rect.width;
        }
    }
    private float contentElementSizeWithSpace;
    private NewScroolViewLineElement CreateLine()
    {
        GameObject[] contents = new GameObject[elementsInLineCount];
        for (int j = 0; j < elementsInLineCount; j++)
        {
            contents[j] = Instantiate(contentElement);
        }
        NewScroolViewLineElement lineElement = Instantiate(lineElementTemplate);
        lineElement.Init(contentContainer, contents, isVertical, spacingInLine, padding);
        lineElement.SetStartBorders(NewScroolViewLineElement.StartBorderPosition.top);
        return lineElement;
    }
    private void CreateLines()
    {
        for (int i = 0; i < linesCount; i++)
        {

            lineElements.Add(CreateLine());
        }
    }

    private void UpdateLines(bool isForward)
    {
        if (isForward)
        {
            NewScroolViewLineElement firstLineElement = lineElements[0];
            int lastElenmentIndex = lineElements.Count - 1;
            for (int i = 0; i < lastElenmentIndex; i++)
            {
                lineElements[i] = lineElements[i + 1];
            }
            firstLineElement.Destroy();
            lineElements[lastElenmentIndex] = CreateLine();
        }
        else
        {
            NewScroolViewLineElement lastLineElement = lineElements[lineElements.Count - 1];
            int firstElenmentIndex = 0;
            for (int i = lineElements.Count - 1; i > firstElenmentIndex; i--)
            {
                lineElements[i] = lineElements[i - 1];
            }
            lastLineElement.Destroy();
            lineElements[firstElenmentIndex] = CreateLine();
        }

        //Пересчитать положения линий
        for (int i = 0; i < lineElements.Count; i++)
        {
            //-1, т.к. надо, чтобы линии всегда появлись за пределами области видимости.
            lineElements[i].SetLinePoistion(true, (i + firstShowedLineIndex-1) * contentElementSizeWithSpace);
        }
    }
    private void Awake()
    {
        scrollRect = GetComponent<ScrollRect>();
        scrollRect.onValueChanged.AddListener(ContentPositionChanged);

        contentElementSizeWithSpace = spacingBetweenLine + contentElementSize;
        //+1 чтобы былb всегда строки, которые будут появляться вне области видимости и приезжать в нее.
        linesCount = (int)(viewPortSize / contentElementSizeWithSpace) + 1;

        CreateLines();

        if (lineElements.Count == 0)
        {
            Debug.LogError("Lines count most be more 0!");
            return;
        }

        RectTransform contentContainerRectTransform = contentContainer.GetComponent<RectTransform>();
        contentContainerRectTransform.sizeDelta  = new Vector2(contentContainerRectTransform.sizeDelta.x, allLines * contentElementSizeWithSpace + spacingBetweenLine * 2);


        UpdateLines(true);
    }

    private Vector3 containerPosition
    {
        get => contentContainer.localPosition;
    }
    private int firstShowedLineIndex = 0;
    private Vector3 lastUpdatePosition;
    private void ContentPositionChanged(Vector2 newPosition)
    {
        RectTransform contaierRectTransform = viewPortTransform.GetComponent<RectTransform>();
        float elementRatioSize = contentElementSizeWithSpace / contaierRectTransform.rect.height;

        Debug.Log($"elementRatioSize = {elementRatioSize}");
        Debug.Log($"newPosition = {newPosition}");
        if (isVertical)
        {
            firstShowedLineIndex = (int)(containerPosition.y / contentElementSizeWithSpace);
            float delta = containerPosition.y - lastUpdatePosition.y;
            if (MathF.Abs(delta) > contentElementSizeWithSpace)
            {
                int lineForUpdateCount = (int)(MathF.Abs(delta) / contentElementSizeWithSpace);
                for (int i = 0; i < lineForUpdateCount; i++)
                    UpdateLines(delta > 0);
                lastUpdatePosition = containerPosition;
            }
        }
    }
}
