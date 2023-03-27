using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OptimazeScroll
{
    /// <summary>
    /// Оптимизированный скролл.
    /// </summary>
    public class OptimaizedScroll : MonoBehaviour
    {
        #region Внешние данные

        /// <summary>
        /// Положение скролла, по какой оси он будет двигаться.
        /// </summary>
        [SerializeField] private bool isVertical = true;
        /// <summary>
        /// Количество элементов в линии.
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

        [Header("Technical information")]
        [SerializeField] private Transform contentContainer = null;
        [SerializeField] private Transform viewPortTransform = null;
        [SerializeField] private OptimaizedScrollLine lineElementTemplate = null;


        /// <summary>
        /// Всего линий в будет в скролле, не сколько отображается, а именно общее количество.
        /// Расчитывается на основе количества всех объектов в скролле 
        /// деленного на количество объектов в строке.
        /// </summary>
        private int allLinesCount = 90;
        /// <summary>
        /// Количество линий видимых в скролле.
        /// Рассчитывается исходя из размера <see cref="viewPortTransform"/> 
        /// деленного на размер строки и расстояния между строк.
        /// </summary>
        private int visibleLinesCount = 1;
        /// <summary>
        /// Шаблон визуализатора обектов в скролле.
        /// </summary>
        private IScrollInitializable contentElementTemplate;
        /// <summary>
        /// Объекты для визуализирования в скролле.
        /// </summary>
        private IScrollInitializeObject[] scrollInitializeItems;
        /// <summary>
        /// Передать в скролл внешние данные.
        /// </summary>
        /// <param name="contentElementTemplate">Задать шаблон визуализатора обектов в скролле.</param>
        /// <param name="items">Передать объекты для визуализирования в скролле.</param>
        public void Initialize(IScrollInitializable contentElementTemplate, IScrollInitializeObject[] items)
        {

            this.contentElementTemplate = contentElementTemplate;
            scrollInitializeItems = items;
            allLinesCount = (items.Length / elementsInLineCount) + 2;
            //Если в последней строке нет предметов
            //(т.е. количество всех предметов картно количеству предметов в строке), то она не нужна.
            if ((allLinesCount -2)* elementsInLineCount == items.Length)
            {
                allLinesCount -= 1;
            }

            lineBorderPosition = isVertical ? StartBorderPosition.top : StartBorderPosition.left;

            //Получить ScrollRect и подписаться на изменение положения контента
            scrollRect = GetComponent<ScrollRect>();
            scrollRect.onValueChanged.AddListener(ContentPositionChanged);
            scrollRect.vertical = isVertical;
            scrollRect.horizontal = !isVertical;

            //Получить размеры из RectTransform
            RectTransform contentRectTransform = contentElementTemplate.GetRectTransform();
            contentElementSize = isVertical ? contentRectTransform.rect.height : contentRectTransform.rect.width;
            RectTransform rectTransform = viewPortTransform.GetComponent<RectTransform>();
            viewPortSize = isVertical ? rectTransform.rect.height : rectTransform.rect.width;

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
                contentContainerRectTransform.sizeDelta = new Vector2(contentContainerRectTransform.sizeDelta.x, (allLinesCount -1) * contentElementSizeWithSpace);
            else
                contentContainerRectTransform.sizeDelta = new Vector2((allLinesCount - 1) * contentElementSizeWithSpace, contentContainerRectTransform.sizeDelta.y);
            StartBorderPositionSetter.SetStartBorders(lineBorderPosition, contentContainerRectTransform);
            lineElementTemplate.SetStartBorders(lineBorderPosition);

            //Обновить положение линий.
            CalculateFirstShowedLineIndex();
            UpdateLinePosition();
        }

        #endregion Внешние данные

        #region Внутренние данные.

        /// <summary>
        /// К какому краю будут прикреплены линии.
        /// </summary>
        private StartBorderPosition lineBorderPosition = StartBorderPosition.left;
        private ScrollRect scrollRect;
        /// <summary>
        /// Список линий с объектами.
        /// </summary>
        private List<OptimaizedScrollLine> lineElements = new List<OptimaizedScrollLine>();
        /// <summary>
        /// Размер объекта в линии.
        /// </summary>
        private float contentElementSize;
        /// <summary>
        /// Размер вьюпорта - родителя контейнера с линиями.
        /// </summary>
        private float viewPortSize;
        /// <summary>
        /// Размер объекта в линии с разницей между объектами.
        /// </summary>
        private float contentElementSizeWithSpace;

        #endregion Внутренние данные.

        #region создание линий.

        /// <summary>
        /// Количество созданных на этот момент линий за все время.
        /// </summary>
        private int currentLinesCount = 0;
        /// <summary>
        /// Создать одну линию и инициализировать ее.
        /// </summary>
        /// <returns></returns>
        private OptimaizedScrollLine CreateLine()
        {
            ++currentLinesCount;
            OptimaizedScrollLine lineElement = Instantiate(lineElementTemplate);
            lineElement.Init(contentContainer, elementsInLineCount, contentElementTemplate, isVertical, spacingInLine, padding);
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
                lineElements.Add(PopLine());
            }
            UpdateLineItems();
        }

        #endregion создание линий.

        #region хранение линий

        /// <summary>
        /// Отключенные линии.
        /// </summary>
        private Stack<OptimaizedScrollLine> disableLines = new Stack<OptimaizedScrollLine>();
        /// <summary>
        /// Получить отключенную линию.
        /// </summary>
        /// <returns></returns>
        private OptimaizedScrollLine PopLine()
        {
            if (disableLines.Count == 0)
            {
                disableLines.Push(CreateLine());
            }
            OptimaizedScrollLine line = disableLines.Pop();
            line.SetActive(true);

            return line;
        }
        /// <summary>
        /// Отправить линию в отключенные.
        /// </summary>
        /// <param name="lineElement"></param>
        private void PushLine(OptimaizedScrollLine lineElement)
        {
            lineElement.SetActive(false);
            disableLines.Push(lineElement);
        }

        #endregion хранение линий.

        #region обновление линий

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
        private int firstShowedLineIndexForCalculate
        {
            get
            {
                return isVertical ? firstShowedLineIndex : -firstShowedLineIndex;
            }
        }
        /// <summary>
        /// Пересчитать положения линий внутри контейнера.
        /// </summary>
        private void UpdateLinePosition()
        {
            int indexShift = 0;
            int firstShowedIndex = firstShowedLineIndexForCalculate;
            for (int i = 0; i < lineElements.Count; i++)
            {
                //-1, т.к. надо, чтобы линии всегда появлись за пределами области видимости.
                indexShift = i + firstShowedIndex - 1;
                lineElements[i].SetLinePoistion(isVertical, indexShift * contentElementSizeWithSpace);
            }
        }
        /// <summary>
        /// Пересчитать положения линий внутри контейнера.
        /// </summary>
        private void UpdateLineItems()
        {
            int firstShowedIndex = firstShowedLineIndexForCalculate;
            firstShowedIndex -= 1;
            firstShowedIndex = firstShowedIndex * elementsInLineCount;
            int itemIndex = 0;
            OptimaizedScrollLine currentLine = null;
            for (int i = 0; i < lineElements.Count; i++)
            {
                currentLine = lineElements[i];
                if (currentLine.isNew)
                {
                    for(int j=0;j<elementsInLineCount;j++)
                    {
                        itemIndex = firstShowedIndex + i * elementsInLineCount + j;
                        if (itemIndex >= scrollInitializeItems.Length || itemIndex < 0)
                        {
                            currentLine.InitContent(null, j);
                        }
                        else
                        {
                            currentLine.InitContent(scrollInitializeItems[itemIndex], j);
                        }
                    }
                }
            }

            UpdateVisibilityLines();
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
                OptimaizedScrollLine firstLineElement = lineElements[0];
                int lastElenmentIndex = lineElements.Count - 1;
                for (int i = 0; i < lastElenmentIndex; i++)
                {
                    lineElements[i] = lineElements[i + 1];
                }
                PushLine(firstLineElement);
                lineElements[lastElenmentIndex] = PopLine();
            }
            else
            {
                OptimaizedScrollLine lastLineElement = lineElements[lineElements.Count - 1];
                int firstElenmentIndex = 0;
                for (int i = lineElements.Count - 1; i > firstElenmentIndex; i--)
                {
                    lineElements[i] = lineElements[i - 1];
                }
                PushLine(lastLineElement);
                lineElements[firstElenmentIndex] = PopLine();
            }
            UpdateLinePosition();
        }

        #endregion обновление линий

        /// <summary>
        /// Обработать изменение первого покакзываемого индекса.
        /// </summary>
        private void OnChangeFirstShowedIndex()
        {
            int delta = prevFirstShowedLineIndex - firstShowedLineIndex;
            int lineForUpdateCount = Mathf.Abs(delta);
            for (int i = 0; i < lineForUpdateCount; i++)
                UpdateLines(delta < 0);

            UpdateLineItems();
            prevFirstShowedLineIndex = firstShowedLineIndex;
        }
        /// <summary>
        /// Обновить видимость строк в зависимости от того, был ли достигнут край контейнера.
        /// Строки за краями контейнера будут скрыты.
        /// </summary>
        private void UpdateVisibilityLines()
        {
            for (int i = 0; i < lineElements.Count; i++)
            {
                lineElements[i].SetActive(true);
            }
            //Скролл достиг начала
            if (firstShowedLineIndexForCalculate <= 0)
            {
                int lineToHideCount = Math.Abs(firstShowedLineIndexForCalculate) + 1;
                for (int i = 0; i < lineToHideCount; i++)
                    lineElements[i].SetActive(false);
            }
            //скролл достиг конца.
            int lastShowedIndex = firstShowedLineIndexForCalculate + visibleLinesCount;
            if (lastShowedIndex >= allLinesCount)
            {
                //Если есть хоть предмет в новой строке, то надо будет ее все равно отобразить.
                int additionalLines = 0;
                int allVisibleItems = (allLinesCount - 1) * elementsInLineCount;
                if (allVisibleItems < scrollInitializeItems.Length)
                {
                    additionalLines = 1;
                }


                int lineToHideCount = lastShowedIndex - allLinesCount - additionalLines;
                int lastIndex = lineElements.Count - 1;
                while (lineToHideCount > 0)
                {
                    lineElements[lastIndex].SetActive(false);
                    --lastIndex;
                    --lineToHideCount;
                }
            }
        }
        /// <summary>
        /// Изменить положение линий в зависимости от сдвига контейнера линий.
        /// </summary>
        /// <param name="newPosition">Метод нужен только чтобы отлавливать движение контейнера линий, сам параметр не требуется.</param>
        private void ContentPositionChanged(Vector2 newPosition)
        {
            CalculateFirstShowedLineIndex();
            if (prevFirstShowedLineIndex != firstShowedLineIndex)
                OnChangeFirstShowedIndex();
        }
    }
}