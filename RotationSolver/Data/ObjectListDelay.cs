using Dalamud.Game.ClientState.Objects.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RotationSolver.Data;

internal class ObjectListDelay<T> : IEnumerable<T> where T : GameObject
{
    IEnumerable<T> _list = new T[0];
    Func<(float min, float max)> _getRange;
    SortedList<uint, DateTime> _revealTime = new SortedList<uint, DateTime>();
    Random _ran = new Random(DateTime.Now.Millisecond);

    public ObjectListDelay(Func<(float min, float max)> getRange)
    {
        _getRange = getRange;
    }

    public void Delay(IEnumerable<T> originData)
    {
        var outList= new List<T>(originData.Count());
        var revealTime = new SortedList<uint, DateTime>();
        var now = DateTime.Now;

        foreach (var item in originData)
        {
            if(!_revealTime.TryGetValue(item.ObjectId, out var time))
            {
                var range = _getRange();
                var delaySecond = range.min + (float)_ran.NextDouble() * (range.max - range.min);
                time = now + new TimeSpan(0, 0, 0, 0, (int)(delaySecond * 1000));
            }
            revealTime.Add(item.ObjectId, time);

            if (time > now)
            {
                outList.Add(item);
            }
        }

        _revealTime = revealTime;
    }


    public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => _list.GetEnumerator();
}
