using Microsoft.Xna.Framework;

namespace Race2;

public class Timer
{
    private float elapsedTime; 
    private bool isRunning;   
    private float bestTime;

    public Timer()
    {
        elapsedTime = 0f;
        isRunning = false;
        bestTime = 0;
    }

    /// <summary>
    /// Обновляет таймер, если он запущен.
    /// </summary>
    /// <param name="gameTime">Объект GameTime для получения прошедшего времени.</param>
    public void Update(GameTime gameTime)
    {
        if (isRunning)
        {
            elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
    }

    /// <summary>
    /// Запускает таймер.
    /// </summary>
    public void Start()
    {
        isRunning = true;
    }

    /// <summary>
    /// Останавливает таймер.
    /// </summary>
    public void Stop()
    {
        isRunning = false;
    }

    /// <summary>
    /// Обнуляет таймер.
    /// </summary>
    public void Reset()
    {
        elapsedTime = 0f;
    }

    /// <summary>
    /// Возвращает время в формате "м:с.мс".
    /// </summary>
    /// <returns>Строка в формате минуты:секунды.миллисекунды.</returns>
    public string GetFormattedTime(float lap)
    {
        int minutes = (int)(lap / 60);
        int seconds = (int)(lap % 60);
        int milliseconds = (int)((lap - (int)lap) * 1000);

        return $"{minutes:D2}:{seconds:D2}.{milliseconds:D3}";
    }

    /// <summary>
    /// Возвращает текущее значение времени в секундах.
    /// </summary>
    public float GetElapsedTime()
    {
        return elapsedTime;
    }

    /// <summary>
    /// Проверяет и обновляет лучшее время.
    /// </summary>
    /// <param name="newTime">Новое время для сравнения.</param>
    /// <returns>Возвращает true, если новое время стало лучшим.</returns>
    public float UpdateBestTime(float newTime)
    {
        if ( newTime > 1 && (bestTime == 0 || newTime < bestTime))
        {
            bestTime = newTime;
            return bestTime;
        }
        return bestTime; 
    }

    
}