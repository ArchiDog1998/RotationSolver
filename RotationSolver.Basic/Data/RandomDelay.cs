namespace RotationSolver.Basic.Data;

/// <summary>
/// Random delay the bool.
/// </summary>
public struct RandomDelay
{
    DateTime _startDelayTime = DateTime.Now;
    float _delayTime = -1;
    readonly Func<(float min, float max)> _getRange;
    readonly Random _ran = new(DateTime.Now.Millisecond);
    bool _lastValue = false;

    /// <summary>
    /// Constructer.
    /// </summary>
    /// <param name="getRange"></param>
    public RandomDelay(Func<(float min, float max)> getRange)
    {
        _getRange = getRange;
    }

    /// <summary>
    /// Delay the bool.
    /// </summary>
    /// <param name="originData"></param>
    /// <returns></returns>
    public bool Delay(bool originData)
    {
        if (_getRange == null) return false;

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
            var (min, max) = _getRange();
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
}
