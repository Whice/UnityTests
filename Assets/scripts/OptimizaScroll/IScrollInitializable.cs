using UnityEngine;

namespace OptimazeScroll
{
    /// <summary>
    /// Инициализируемый объект.
    /// </summary>
    public interface IScrollInitializable
    {
        /// <summary>
        /// Инициализировать обект в скролле.
        /// </summary>
        /// <param name="objectForInitialize">Объект, которым будет инициализировать инициализируемый объект.</param>
        void InScrollInitialize(IScrollInitializeObject objectForInitialize);
        /// <summary>
        /// Создать новый объект на основе пердосталенного с использованием Instantiate.
        /// </summary>
        IScrollInitializable Clone(Transform parent);
        /// <summary>
        /// Получить RectTransform этого объекта.
        /// </summary>
        /// <returns></returns>
        RectTransform GetRectTransform();
    }
}