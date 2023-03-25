using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Оптимизированный скролл.
/// </summary>
public class NewScroolView : MonoBehaviour
{
    /// <summary>
    /// Положение скролла, по какой оси он будет двигаться.
    /// </summary>
    [SerializeField] private bool isVertical = true;
    /// <summary>
    /// Количество элекментов в линии.
    /// </summary>
    [SerializeField] private int elementsInLineCount = 5;
    /// <summary>
    /// Расстояние между линиями.
    /// </summary>
    [SerializeField] private float spacingBetweenLine = 10;
    /// <summary>
    /// Растояние между объектами в линии.
    /// </summary>
    [SerializeField] private float spacingInLine = 10;
    /// <summary>
    /// Оступы от краев.
    /// </summary>
    [SerializeField] private RectOffset padding = new RectOffset();
    /// <summary>
    /// К какому краю будут прикреплены линии.
    /// </summary>
    [SerializeField] private StartBorderPosition lineBorderPosition = StartBorderPosition.left;
    /// <summary>
    /// Всего линий в будет в скролле, не сколько отображается, а именно общее количество.
    /// Расчитывается на основе количества всех объектов в скролле 
    /// деленного на количество объектов в строке.
    /// </summary>
    public int allLinesCount = 90;
    /// <summary>
    /// Количество линий видимых в скролле.
    /// Рассчитывается исходя из размера <see cref="viewPortTransform"/> 
    /// деленного на размер строки и расстояния между строк.
    /// </summary>
    private int visibleLinesCount = 1;

    [Header("Technical information")]
    [SerializeField] private Transform contentContainer = null;
    [SerializeField] private Transform viewPortTransform = null;
    [SerializeField] private GameObject contentElement = null;
    [SerializeField] private NewScroolViewLineElement lineElementTemplate = null;


    private ScrollRect scrollRect;
    /// <summary>
    /// Список линий с объектами.
    /// </summary>
    private List<NewScroolViewLineElement> lineElements = new List<NewScroolViewLineElement>();
    /// <summary>
    /// Размер объекта в линии.
    /// </summary>
    private float contentElementSize
    {
        get
        {
            RectTransform contentRectTransform = contentElement.GetComponent<RectTransform>();
            return isVertical ? contentRectTransform.rect.height : contentRectTransform.rect.width;
        }
    }
    /// <summary>
    /// Размер вьюпорта - родителя контейнера с линиями.
    /// </summary>
    private float viewPortSize
    {
        get
        {
            RectTransform rectTransform = viewPortTransform.GetComponent<RectTransform>();
            return isVertical ? rectTransform.rect.height : rectTransform.rect.width;
        }
    }
    /// <summary>
    /// Размер объекта в линии с разницей между объектами.
    /// </summary>
    private float contentElementSizeWithSpace;
    /// <summary>
    /// Количество созданных на этот момент линий за все время.
    /// </summary>
    private int currentLinesCount = 0;
    /// <summary>
    /// Создать одну линию и инициализировать ее.
    /// </summary>
    /// <returns></returns>
    private NewScroolViewLineElement CreateLine()
    {
        ++currentLinesCount;
        GameObject[] contents = new GameObject[elementsInLineCount];
        for (int j = 0; j < elementsInLineCount; j++)
        {
            contents[j] = Instantiate(contentElement);
        }
        NewScroolViewLineElement lineElement = Instantiate(lineElementTemplate);
        lineElement.Init(contentContainer, contents, isVertical, spacingInLine, padding);
        lineElement.SetLineNumber(currentLinesCount);
        return lineElement;
    }
    /// <summary>
    /// Создать линии заданного изначально количества.
    /// </summary>
    private void CreateLines()
    {
        for (int i = 0; i < visibleLinesCount; i++)
        {
            lineElements.Add(CreateLine());
        }
    }


    /// <summary>
    /// Положение контейнера линий.
    /// </summary>
    private Vector3 containerPosition
    {
        get => contentContainer.localPosition;
    }
    /// <summary>
    /// Индекс первой линии которая рисуется в нынешний момент.
    /// </summary>
    private int firstShowedLineIndex = 0;
    /// <summary>
    /// Индекс первой линии при последней перерисовке строк.
    /// </summary>
    private int prevFirstShowedLineIndex = 0;
    /// <summary>
    /// Посчитать первый индекс показываемой линии в зависимости от сдвига контейнера линий.
    /// </summary>
    private void CalculateFirstShowedLineIndex()
    {
        if (isVertical)
        {
            float containerPositionY = containerPosition.y <= 0 ? containerPosition.y - 1 : containerPosition.y;
            firstShowedLineIndex = (int)(containerPositionY / contentElementSizeWithSpace);
        }
        else
        {
            float containerPositionX = containerPosition.x <= 0 ? containerPosition.x - 1 : containerPosition.x;
            firstShowedLineIndex = (int)(containerPositionX / contentElementSizeWithSpace);
        }
    }
    /// <summary>
    /// Пересчитать положения линий внутри контейнера.
    /// </summary>
    private void UpdateLinePosition()
    {
        int indexShift = 0;
        int firstShowedIndex = isVertical ? firstShowedLineIndex : -firstShowedLineIndex;
        for (int i = 0; i < lineElements.Count; i++)
        {
            //-1, т.к. надо, чтобы линии всегда появлись за пределами области видимости.
            indexShift = i + firstShowedIndex - 1;
            lineElements[i].SetLinePoistion(isVertical, indexShift * contentElementSizeWithSpace);
        }
    }
    /// <summary>
    /// Обновить линии внутри массива линий, для перерасчета их положения.
    /// </summary>
    /// <param name="isForward"></param>
    private void UpdateLines(bool isForward)
    {
        isForward = isVertical ? isForward : !isForward;
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
        UpdateLinePosition();
    }
    private void Awake()
    {
        //Получить ScrollRect и подписаться на изменение положения контента
        scrollRect = GetComponent<ScrollRect>();
        scrollRect.onValueChanged.AddListener(ContentPositionChanged);
        scrollRect.vertical = isVertical;
        scrollRect.horizontal = !isVertical;

        //Посчитать количество видимых линий
        contentElementSizeWithSpace = spacingBetweenLine + contentElementSize;
        //+2 чтобы было всегда строки, которые будут появляться вне области видимости и приезжать в нее.
        visibleLinesCount = (int)(viewPortSize / contentElementSizeWithSpace) + 2;

        //Создать линии
        CreateLines();

        //Если линий нет, что-то пошло не так.
        if (lineElements.Count == 0)
        {
            Debug.LogError("Lines count most be more 0!");
            return;
        }

        //Рассчитать размер контейнера линий исходя из количества самих линий, их размера и расстояний между ними.
        RectTransform contentContainerRectTransform = contentContainer.GetComponent<RectTransform>();
        if (isVertical)
            contentContainerRectTransform.sizeDelta = new Vector2(contentContainerRectTransform.sizeDelta.x, allLinesCount * contentElementSizeWithSpace);
        else
            contentContainerRectTransform.sizeDelta = new Vector2(allLinesCount * contentElementSizeWithSpace, contentContainerRectTransform.sizeDelta.y);
        StartBorderPositionSetter.SetStartBorders(lineBorderPosition, contentContainerRectTransform);
        lineElementTemplate.SetStartBorders(lineBorderPosition);

        //Обновить положение линий.
        CalculateFirstShowedLineIndex();
        UpdateLinePosition();
    }
    /// <summary>
    /// Изменить положение линий в зависимости от сдвига контейнера линий.
    /// </summary>
    /// <param name="newPosition">Метод нужен только чтобы отлавливать движение контейнера линий, сам параметр не требуется.</param>
    private void ContentPositionChanged(Vector2 newPosition)
    {
        CalculateFirstShowedLineIndex();
        if (prevFirstShowedLineIndex != firstShowedLineIndex)
        {
            int delta = prevFirstShowedLineIndex - firstShowedLineIndex;
            int lineForUpdateCount = Mathf.Abs(delta);
            for (int i = 0; i < lineForUpdateCount; i++)
                UpdateLines(delta < 0);
            prevFirstShowedLineIndex = firstShowedLineIndex;
        }
    }
}
