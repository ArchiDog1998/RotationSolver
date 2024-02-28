namespace RotationSolver.Basic.Data;

/// <summary>
/// Random delay the bool.
/// </summary>
/// <remarks>
/// Constructer.
/// </remarks>
/// <param name="getRange"></param>
public struct RandomDelay(Func<(float min, float max)> getRange)
{
    DateTime _startDelayTime = DateTime.Now;
    float _delayTime = -1;

    readonly Random _ran = new(DateTime.Now.Millisecond);
    bool _lastValue = false;

    /// <summary>
    /// 
    /// </summary>
    public readonly Func<(float min, float max)> GetRange => getRange;

    /// <summary>
    /// The constructor for the config.
    /// </summary>
    /// <param name="getRange">The way to get the config.</param>
    public RandomDelay(Func<Vector2> getRange)
        : this(() =>
        {
            var vec = getRange();
            return (vec.X, vec.Y);
        })
    {
        
    }

    /// <summary>
    /// Delay the bool to be true.
    /// </summary>
    /// <param name="originData"></param>
    /// <returns></returns>
    public bool Delay(bool originData)
    {
        if (GetRange == null) return originData;
        var (min, max) = GetRange();
        if (min <= 0 || max <= 0) return originData;

        if (!originData)
        {
            _lastValue = false;
            _delayTime = -1;
            return false;
        }

        //Not start And changed.
        if (_delayTime < 0 && !_lastValue)
        {
            _lastValue = true;
            _startDelayTime = DateTime.Now;
            _delayTime = min + (float)_ran.NextDouble() * (max - min);
        }
        //Times up
        else if ((DateTime.Now - _startDelayTime).TotalSeconds >= _delayTime)
        {
            //I set it.
            _delayTime = -1;
            return true;
        }

        return false;
    }

    /// <summary>
    /// The delay to get the item.
    /// </summary>
    /// <typeparam name="T">the type of anything.</typeparam>
    /// <param name="originData">original data.</param>
    /// <returns>the value.</returns>
    public T? Delay<T>(T? originData) where T : class
    {
        var b = originData != null;

        if (Delay(b))
        {
            return originData;
        }
        else
        {
            return null;
        }
    }
}
