using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Элемент в скролле, который пердставляет собой линию.
/// Строку, если скролл вертикальный, столбец, если он горизонтальный.
/// </summary>
public class NewScroolViewLineElement : MonoBehaviour
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
    private GameObject[] contentElements;
    /// <summary>
    /// Задать параметры линии.
    /// </summary>
    /// <param name="contentContainer">Контейнер, где линия будт находиться в иерархии.</param>
    /// <param name="contentElements">Элементы для отображении в линии.</param>
    /// <param name="isVertical">Положение скролла, вертикальное или горизонтальное.</param>
    /// <param name="spacing">Расстояние между объектами внутри линии.</param>
    /// <param name="padding">Сдвиги от краев объектов внутри линии.</param>
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

    public void SetStartBorders(StartBorderPosition borderPosition)
    {
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        StartBorderPositionSetter.SetStartBorders(borderPosition, rectTransform);
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
            Destroy(gameObject);
            isDestroied = true;
        }
    }
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
