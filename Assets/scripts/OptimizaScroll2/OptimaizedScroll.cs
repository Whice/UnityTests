using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.OptimazeScroll
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
        /// <summary>
        /// Задать каждому элементу в скролле фиксированный размер.
        /// </summary>
        [SerializeField] private bool isFixedSize = false;
        /// <summary>
        /// Фиксированный размер для элементов скролла.
        /// </summary>
        [SerializeField] private Vector2 fixedSize;

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
        /// Событие создания экземпляра инициализируемого объекта скроллом.
        /// </summary>
        public event Action<IScrollInitializable> createdIScrollInitializable;
        /// <summary>
        /// Удалить все подписки на создание объектов в скролле.
        /// </summary>
        public void CreatedIScrollInitializableRemoveListeners()
        {
            createdIScrollInitializable = null;
        }
        /// <summary>
        /// Сбросить информацию о индексе первой показываемой линии на значения по умолчанию.
        /// </summary>
        private void ResetFirstShowedIndexInfo()
        {
            firstShowedLineIndex = 0;
            prevFirstShowedLineIndex = 0;
        }
        /// <summary>
        /// Очистить список линий.
        /// </summary>
        private void LinesClear()
        {
            foreach (var line in disableLines)
            {
                line.Destroy();
            }
            disableLines.Clear();

            for (int i = 0; i < lineElements.Count; i++)
            {
                lineElementsInDefaulPosition[i].Destroy();
                lineElementsInDefaulPosition[i] = null;
                lineElements[i].Destroy();
                lineElements[i] = null;
            }
            lineElements.Clear();
            lineElementsInDefaulPosition.Clear();
        }
        /// <summary>
        /// Передать в скролл внешние данные.
        /// </summary>
        /// <param name="contentElementTemplate">Задать шаблон визуализатора обектов в скролле.</param>
        /// <param name="items">Передать объекты для визуализирования в скролле.</param>
        /// <param name="isNotRecreateContentElementsIfSameType">Не пересоздавать элементы содержимого, если они одного типа.</param>
        public void Initialize(
            IScrollInitializable contentElementTemplate,
            IScrollInitializeObject[] items,
            bool isNotRecreateContentElementsIfSameType = false)
        {
            //Получить ScrollRect и подписаться на изменение положения контента
            if (scrollRect == null)
            {
                scrollRect = GetComponent<ScrollRect>();
                scrollRect.onValueChanged.AddListener(ContentPositionChanged);
            }
            scrollRect.StopMovement();
            scrollRect.vertical = isVertical;
            scrollRect.horizontal = !isVertical;

            //Переданный шаблон контента имеет такой же тип, что и предыдущий.
            bool isContentElementTemplateSameType = false;
            if (this.contentElementTemplate != null && contentElementTemplate != null)
            {
                isContentElementTemplateSameType = contentElementTemplate.GetType() == this.contentElementTemplate.GetType();
            }
            this.contentElementTemplate = contentElementTemplate;
            scrollInitializeItems = items;
            allLinesCount = (items.Length / elementsInLineCount) + 2;
            //Если в последней строке нет предметов
            //(т.е. количество всех предметов картно количеству предметов в строке), то она не нужна.
            if ((allLinesCount - 2) * elementsInLineCount == items.Length)
            {
                allLinesCount -= 1;
            }

            StartBorderPosition containerBorderPosition = isVertical ? StartBorderPosition.top : StartBorderPosition.left;
            lineBorderPosition = isVertical ? StartBorderPosition.left : StartBorderPosition.top;


            //Получить размеры из RectTransform
            RectTransform contentRectTransform = contentElementTemplate.GetRectTransform();
            if (isFixedSize)
            {
                contentRectTransform.sizeDelta = fixedSize;
                contentRectTransform.pivot = new Vector2(0.5f, 0.5f);
                contentRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                contentRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                contentRectTransform.anchoredPosition = new Vector2(0, 0);
            }
            contentElementSize = isVertical ? contentRectTransform.rect.height : contentRectTransform.rect.width;
            RectTransform rectTransform = viewPortTransform.GetComponent<RectTransform>();
            viewPortSize = isVertical ? rectTransform.rect.height : rectTransform.rect.width;
            float viewPortWidth = isVertical ? rectTransform.rect.width : rectTransform.rect.height;

            //Посчитать количество видимых линий
            contentElementSizeWithSpace = spacingBetweenLine + contentElementSize;
            //+3 чтобы было всегда строки, которые будут появляться вне области видимости и приезжать в нее.
            visibleLinesCount = (int)(viewPortSize / contentElementSizeWithSpace) + 3;



            //Рассчитать размер контейнера линий исходя из количества самих линий, их размера и расстояний между ними.
            RectTransform contentContainerRectTransform = contentContainer.GetComponent<RectTransform>();
            if (isVertical)
                contentContainerRectTransform.sizeDelta = new Vector2(viewPortWidth, (allLinesCount - 1) * contentElementSizeWithSpace);
            else
                contentContainerRectTransform.sizeDelta = new Vector2((allLinesCount - 1) * contentElementSizeWithSpace, viewPortWidth);
            StartBorderPositionSetter.SetStartBorders(containerBorderPosition, contentContainerRectTransform);
            lineElementTemplate.SetStartBorders(lineBorderPosition);

            ResetFirstShowedIndexInfo();
            CalculateFirstShowedLineIndex();
            //Пересоздать линии с контентом, только если установлено их всегда пересоздавать или тип контента отличается.
            if (!isNotRecreateContentElementsIfSameType || (isNotRecreateContentElementsIfSameType && !isContentElementTemplateSameType))
            {
                currentLinesCount = 0;
                LinesClear();
                //Создать линии
                CreateLines();
                //Если линий нет, что-то пошло не так.
                if (lineElements.Count == 0)
                {
                    Debug.LogError("Lines count most be more 0!");
                    return;
                }
            }
            else
            {
                ResetLinesPositionToDefaul();
            }
            //Обновить данные в контейнерах.
            UpdateLineItems();
            //Обновить положение линий.
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
        /// Получить массив иницализируемых объектов в скролле.
        /// </summary>
        /// <typeparam name="IScrollInitializableInheritable">Тип к которому будут приведены инициалихируемые объекты.
        /// Этот тип должен наследоваться от <see cref="IScrollInitializable"/>.</typeparam>
        /// <returns></returns>
        public IScrollInitializableInheritable[] GetContentElementsAs<IScrollInitializableInheritable>() where IScrollInitializableInheritable : class
        {

            if (lineElements != null)
            {
                IScrollInitializableInheritable[] contentElements = new IScrollInitializableInheritable[visibleLinesCount * elementsInLineCount];
                for (int lineNumber = 0; lineNumber < lineElements.Count; lineNumber++)
                {
                    IScrollInitializable[] lineContentElements = lineElements[lineNumber].GetContentElements();
                    for (int elementNumber = 0; elementNumber < elementsInLineCount; elementNumber++)
                    {
                        contentElements[lineNumber * elementsInLineCount + elementNumber] = lineContentElements[elementNumber] as IScrollInitializableInheritable;
                    }
                }
                return contentElements;
            }
            else
            {
                return new IScrollInitializableInheritable[0];
            }
        }

        /// <summary>
        /// Список линий в том порядке, в котором они были созданы.
        /// </summary>
        private List<OptimaizedScrollLine> lineElementsInDefaulPosition = new List<OptimaizedScrollLine>();
        /// <summary>
        /// Создать одну линию и инициализировать ее.
        /// </summary>
        /// <returns></returns>
        private OptimaizedScrollLine CreateLine()
        {
            ++currentLinesCount;
            OptimaizedScrollLine lineElement = Instantiate(lineElementTemplate);
            lineElement.createdIScrollInitializable += createdIScrollInitializable;
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
                OptimaizedScrollLine lineElement = PopLine();
                lineElements.Add(lineElement);
                lineElementsInDefaulPosition.Add(lineElement);
            }
        }
        /// <summary>
        /// Перезаписать линии с контентов в списке в том порядке, в котором они были при создании.
        /// </summary>
        private void ResetLinesPositionToDefaul()
        {
            for (int i = 0; i < lineElements.Count; i++)
            {
                lineElements[i] = lineElementsInDefaulPosition[i];
                lineElements[i].isNew = true;
            }
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
            line.isNew = true;
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
        public event Action linesUpdated;
        /// <summary>
        /// Перераспределить хранимые в линиях внутри контейнеры.
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
                    for (int j = 0; j < elementsInLineCount; j++)
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
            linesUpdated?.Invoke();
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
                int end = lineToHideCount < lineElements.Count ? lineToHideCount : lineElements.Count;
                for (int i = 0; i < end; i++)
                    lineElements[i].SetActive(false);
            }
            //скролл достиг конца.
            int lastShowedIndex = firstShowedLineIndexForCalculate + visibleLinesCount;
            if (lastShowedIndex >= allLinesCount)
            {
                //Если есть хоть один предмет в новой строке, то надо будет ее все равно отобразить.
                int additionalLines = 0;
                int allVisibleItems = (allLinesCount - 1) * elementsInLineCount;
                if (allVisibleItems < scrollInitializeItems.Length)
                {
                    additionalLines = 1;
                }


                int lineToHideCount = lastShowedIndex - allLinesCount - additionalLines;
                int lastIndex = lineElements.Count - 1;
                while (lineToHideCount > 0 && lastIndex != 0)
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

        #endregion обновление линий

    }
}