//Copyright: Made by Appfox

using System;
using UnityEngine;
using UnityEngine.UI;

namespace SummonEra.UI.OptimazeScroll
{
    /// <summary>
    /// Элемент в скролле, который пердставляет собой линию.
    /// Строку, если скролл вертикальный, столбец, если он горизонтальный.
    /// </summary>
    public class OptimaizedScrollLine : MonoBehaviour
    {
        /// <summary>
        /// Компонент для размещения объектов внутри линии.
        /// </summary>
        private HorizontalOrVerticalLayoutGroup layoutGroup;
        /// <summary>
        /// Компонент для подстраивания размера линии под все ее элементы.
        /// </summary>
        private ContentSizeFitter contentSizeFitter;
        /// <summary>
        /// Элементы внутри линии.
        /// </summary>
        private IScrollInitializable[] contentElements;
        public IScrollInitializable[] GetContentElements()
        {
            return contentElements;
        }
        /// <summary>
        /// Событие создания экземпляра инициализируемого объекта.
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
        /// Задать параметры линии.
        /// </summary>
        /// <param name="contentContainer">Контейнер, где линия будт находиться в иерархии.</param>
        /// <param name="contentElements">Элементы для отображении в линии.</param>
        /// <param name="isVertical">Положение скролла, вертикальное или горизонтальное.</param>
        /// <param name="spacing">Расстояние между объектами внутри линии.</param>
        /// <param name="padding">Сдвиги от краев объектов внутри линии.</param>
        public void Init(
            Transform contentContainer, 
            int inLineElementsCount,
            IScrollInitializable contentElementsTemplate,
            bool isVertical,
            float spacing,
            RectOffset padding)
        {
            if (isVertical)
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
            contentElements = new IScrollInitializable[inLineElementsCount];
            for (int i = 0; i < inLineElementsCount; i++)
            {
                contentElements[i] = contentElementsTemplate.Clone(transform);
                createdIScrollInitializable?.Invoke(contentElements[i]);
            }
        }
        /// <summary>
        /// Установить номер линии вместо (Clone) в конце имени, полезно для дебага.
        /// </summary>
        /// <param name="number"></param>
        public void SetLineNumber(int number)
        {
            gameObject.name = gameObject.name.Replace("(Clone)", " " + number.ToString());
        }
        /// <summary>
        /// Установить положение линии внутри ее контайнера.
        /// </summary>
        /// <param name="isVecrtical"></param>
        /// <param name="newPosition"></param>
        public void SetLinePoistion(bool isVecrtical, float newPosition)
        {
            if (isVecrtical)
                transform.localPosition = new Vector3(transform.localPosition.x, -newPosition);
            else
                transform.localPosition = new Vector3(newPosition, transform.localPosition.y);
        }
        /// <summary>
        /// Инициализировать один визуализатор предмета.
        /// </summary>
        /// <param name="objectForInit"></param>
        /// <param name="index"></param>
        public void InitContent(IScrollInitializeObject objectForInit, int index)
        {
            if (index < 0 || index >= contentElements.Length)
            {
                Debug.LogError("Index in line out of range!");
            }

            contentElements[index].InScrollInitialize(objectForInit);
            isNew = false;
        }
        public void SetStartBorders(StartBorderPosition borderPosition)
        {
            RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
            float anchorMin = 0.5f;
            float anchorMax = 0.5f;
            if (borderPosition == StartBorderPosition.left || borderPosition == StartBorderPosition.right)
            {
                anchorMax = 1;
            }
            else if (borderPosition == StartBorderPosition.top || borderPosition == StartBorderPosition.bottom)
            {
                anchorMin = 1;
            }

            StartBorderPositionSetter.SetStartBorders(borderPosition, rectTransform, anchorMin, anchorMax);
        }

        /// <summary>
        /// Линия уничтожена.
        /// </summary>
        private bool isDestroied = false;
        /// <summary>
        /// Уничтожить объект линии.
        /// </summary>
        public void Destroy()
        {
            if (!isDestroied)
            {
                CreatedIScrollInitializableRemoveListeners();
                Destroy(gameObject);
                isDestroied = true;
            }
        }
        /// <summary>
        /// Новая линия, только что была вынута из пулла.
        /// </summary>
        public bool isNew { get; set; }
        /// <summary>
        /// Отключить объект линии.
        /// </summary>
        /// <param name="isActive"></param>
        public void SetActive(bool isActive)
        {
            if (!isDestroied)
            {
                gameObject.SetActive(isActive);
            }
        }

    }
}