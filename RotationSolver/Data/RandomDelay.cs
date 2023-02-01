using FFXIVClientStructs.FFXIV.Client.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotationSolver.Data;

internal struct RandomDelay
{
	DateTime _startDelayTime = DateTime.MinValue;
	float _delayTime = -1;
    Func<(float min, float max)> _getRange;
	Random _ran = new Random(DateTime.Now.Millisecond);
    bool _lastValue = false;
    public RandomDelay(Func<(float min, float max)> getRange)
	{
        _getRange = getRange;
	}

	public bool Delay(bool originData)
	{
        if(_getRange == null) return false;

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
            var range = _getRange();
            _delayTime = range.min + (float)_ran.NextDouble() * (range.max - range.min);
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
