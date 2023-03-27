namespace OptimazeScroll
{
    /// <summary>
    /// От какой стороны внутри контейнера будет происходить заполнение линиями.
    /// </summary>
    public enum StartBorderPosition
    {
        /// <summary>
        /// Начать сверху.
        /// </summary>
        top = 0,
        /// <summary>
        /// Начать снизу.
        /// </summary>
        bottom = 1,
        /// <summary>
        /// Начать слева.
        /// </summary>
        left = 2,
        /// <summary>
        /// Начать справа.
        /// </summary>
        right = 3,
        /// <summary>
        /// Начать в центре.
        /// </summary>
        center = 4
    }
}