namespace RotationSolver.Basic.Data;

/// <summary>
/// Off set the whole bool
/// </summary>
/// <param name="getDelay"></param>
public struct OffsetDelay(Func<float> getDelay)
{
    bool _lastValue = false;
    bool _nowValue = false;
    readonly Queue<DateTime> _changeTimes = new();

    /// <summary>
    /// 
    /// </summary>
    public readonly Func<float> GetDelay => getDelay;

    /// <summary>
    /// Delay the value to change.
    /// </summary>
    /// <param name="originData"></param>
    /// <returns></returns>
    public bool Delay(bool originData)
    {
        if (originData != _lastValue)
        {
            _lastValue = originData;
            _changeTimes.Enqueue(DateTime.Now + TimeSpan.FromSeconds(GetDelay()));
        }

        if (_changeTimes.TryPeek(out var time) && time < DateTime.Now)
        {
            _changeTimes.Dequeue();
            _nowValue = !_nowValue;
        }

        return _nowValue;
    }
}
