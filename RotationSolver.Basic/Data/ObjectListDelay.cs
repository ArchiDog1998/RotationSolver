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
    : IEnumerable<T> where T : GameObject
{
    IEnumerable<T> _list = Array.Empty<T>();
    readonly Func<(float min, float max)> _getRange = getRange;
    SortedList<uint, DateTime> _revealTime = [];
    readonly Random _ran = new(DateTime.Now.Millisecond);

    /// <summary>
    /// The delayed list.
    /// </summary>
    /// <param name="originData"></param>
    public void Delay(IEnumerable<T> originData)
    {
        var outList = new List<T>(originData.Count());
        var revealTime = new SortedList<uint, DateTime>();
        var now = DateTime.Now;

        foreach (var item in originData)
        {
            if (!_revealTime.TryGetValue(item.ObjectId, out var time))
            {
                var (min, max) = _getRange();
                var delaySecond = min + (float)_ran.NextDouble() * (max - min);
                time = now + new TimeSpan(0, 0, 0, 0, (int)(delaySecond * 1000));
            }
            revealTime.Add(item.ObjectId, time);

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
