//Copyright: Made by Appfox

using UnityEngine;

namespace SummonEra.UI.OptimazeScroll
{
    /// <summary>
    /// Переинициализируемый объект в скролле.
    /// </summary>
    public interface IScrollInitializable
    {
        /// <summary>
        /// Инициализация объекта в скролле.
        /// </summary>
        /// <param name="objectForInitialize">Объект для инициализации.</param>
        void InScrollInitialize(IScrollInitializeObject objectForInitialize);
        /// <summary>
        /// Создание копии объекта с помощью Instantiate.
        /// </summary>
        IScrollInitializable Clone(Transform parent);
        /// <summary>
        /// RectTransform инициалиируемого объекта.
        /// </summary>
        /// <returns></returns>
        RectTransform GetRectTransform();
        void Destroy();
        GameObject intializableInstance { get; }
    }
}