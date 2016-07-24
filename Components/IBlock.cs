public interface IBlock
{
    /// <summary>
    /// Запуск анимации уничтожения.
    /// </summary>
    void StartDestroy();
    
    /// <summary>
    /// Функция, срабатывающая в конце анимации уничтожения (event callback).
    /// </summary>
    void EndDestroy();
}
