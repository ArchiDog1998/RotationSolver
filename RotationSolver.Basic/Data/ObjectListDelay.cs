using System.Collections;

namespace RotationSolver.Basic.Data;

/// <summary>
/// A class to delay the object list checking.
/// </summary>
/// <typeparam name="T"></typeparam>
/// <remarks>
/// Constructer.
/// </remarks>
/// <param name="getRange"></param>
public class ObjectListDelay<T>(Func<(float min, float max)> getRange) 
    : IEnumerable<T> where T : IGameObject
{
    IEnumerable<T> _list = [];
    readonly Func<(float min, float max)> _getRange = getRange;
    SortedList<ulong, DateTime> _revealTime = [];
    readonly Random _ran = new(DateTime.Now.Millisecond);

    /// <summary>
    /// The default creator from the config.
    /// </summary>
    /// <param name="getRange">the way to get the config.</param>
    public ObjectListDelay(Func<Vector2> getRange)
        : this(() =>
        {
            var vec = getRange();
            return (vec.X, vec.Y);
        })
    {
        
    }

    /// <summary>
    /// The delayed list.
    /// </summary>
    /// <param name="originData"></param>
    public void Delay(IEnumerable<T> originData)
    {
        var outList = new List<T>(originData.Count());
        var revealTime = new SortedList<ulong, DateTime>();
        var now = DateTime.Now;

        foreach (var item in originData)
        {
            if (!_revealTime.TryGetValue(item.GameObjectId, out var time))
            {
                var (min, max) = _getRange();
                var delaySecond = min + (float)_ran.NextDouble() * (max - min);
                time = now + new TimeSpan(0, 0, 0, 0, (int)(delaySecond * 1000));
            }
            revealTime[item.GameObjectId] = time;

            if (now > time)
            {
                outList.Add(item);
            }
        }

        _list = outList;
        _revealTime = revealTime;
    }

    /// <summary>
    /// Enumerator.
    /// </summary>
    /// <returns></returns>
    public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => _list.GetEnumerator();
}
