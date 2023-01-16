using RotationSolver.Combos.CustomCombo;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RotationSolver.Combos.CustomCombo;

internal abstract partial class CustomCombo
{
    public string Description => string.Join('\n', DescriptionDict.Select(pair => pair.Key.ToName() + " → " + pair.Value));

    /// <summary>
    /// Description about the actions.
    /// </summary>
    public virtual SortedList<DescType, string> DescriptionDict { get; } = new SortedList<DescType, string>();

    DateTime _lactCommandTime = DateTime.Now;
    protected TimeSpan LastCommandElapsed => DateTime.Now - _lactCommandTime;

    /// <summary>
    /// 有什么是需要每一帧进行更新数据用的，放这里。如果有自定义字段，需要在此函数内全部更新一遍。
    /// </summary>
    private protected virtual void UpdateInfo() { }

}
