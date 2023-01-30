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
    Func<bool> _getCheck;
    Func<(float min, float max)> _getRange;
	Random _ran = new Random(DateTime.Now.Millisecond);

    public bool Check => _getCheck?.Invoke() ?? false;
    public RandomDelay(Func<bool> getCheck, Func<(float min, float max)> getRange)
	{
        _getCheck = getCheck;
        _getRange = getRange;
	}

	public bool Update()
	{
        if(_getCheck == null || _getRange == null) return false;

        if (!Check)
        {
            _delayTime = -1;
            return false;
        }

        //Not start
        if (_delayTime < 0)
        {
            _startDelayTime = DateTime.Now;
            var range = _getRange();
            _delayTime = range.min + (float)_ran.NextDouble() * (range.max - range.min);
        }
        //Times up
        else if ((DateTime.Now - _startDelayTime).TotalSeconds >= _delayTime)
        {
            _delayTime = -1;
            return true;
        }

        return false;
    }
}
