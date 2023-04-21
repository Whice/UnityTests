//Copyright: Made by Appfox


namespace SummonEra.UI.OptimazeScroll
{
    /// <summary>
    /// Объект для инициализации внутри оптимизированного скролла.
    /// </summary>
    public interface IScrollInitializeObject { }

    /// <summary>
    /// Объект для инициализации внутри оптимизированного скролла.
    /// </summary>
    public interface IScrollInitializeContainerObject<T> : IScrollInitializeObject
    {
        T Item { get; }
    }
}