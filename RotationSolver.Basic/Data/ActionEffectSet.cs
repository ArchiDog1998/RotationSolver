using FFXIVClientStructs.FFXIV.Client.Game;
using Lumina.Excel.GeneratedSheets;
using System.Collections;
using Action = Lumina.Excel.GeneratedSheets.Action;

namespace RotationSolver.Basic.Data;

public unsafe struct ActionEffectSet
{
    public Action Action { get; }
    public ActionType Type { get; }
    public GameObject Target { get; }
    public GameObject Source { get; }
    public TargetEffect[] TargetEffects { get; }
    public float AnimationLock { get;  }
    public int MyProperty { get; set; }
    public ActionEffectSet(uint sourceId, ActionEffectHeader* effectHeader, ActionEffect* effectArray, ulong* effectTargets)
    {
        Type = effectHeader->actionType;
        Action = Service.GetSheet<Action>().GetRow(effectHeader->actionId);
        Target = Service.ObjectTable.SearchById(effectHeader->animationTargetId);
        Source = Service.ObjectTable.SearchById(sourceId);
        AnimationLock = effectHeader->animationLockTime;

        TargetEffects = new TargetEffect[effectHeader->NumTargets];
        for (int i = 0; i < effectHeader->NumTargets; i++)
        {
            TargetEffects[i] = new TargetEffect(Service.ObjectTable.SearchById(effectTargets[i]), effectArray + 8 * i);
        }
    }

    public Dictionary<uint, ushort> GetSpecificTypeEffect(ActionEffectType type)
    {
        var result = new Dictionary<uint, ushort>();
        foreach (var effect in TargetEffects)
        {
            if(effect.GetSpecificTypeEffect(type, out var e))
            {
                result[effect.Target.ObjectId] = e.Value;
            }
        }
        return result;
    }

    public override string ToString()
    {
        var str = $"S:{Source?.Name}, T:{Target?.Name}, Lock:{AnimationLock}";
        str = $"Lock:{AnimationLock}";
        str += $"\nType: {Type}, Name: {Action?.Name}({Action?.RowId})";
        if (TargetEffects != null)
        {
            foreach (var effect in TargetEffects)
            {
                str += "\n" + effect.ToString();
            }
        }
        return str;
    }
}

public unsafe struct TargetEffect
{
    public GameObject Target;
    private ActionEffect* _effects;

    public TargetEffect(GameObject target, ActionEffect* effects)
    {
        Target = target;
        this._effects = effects;
    }

    /// <summary>
    /// Get Effect.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public ActionEffect this[int index]
    {
        get
        {
            if (index < 0 || index > 7) return default;
            return _effects[index];
        }
    }

    public override string ToString()
    {
        var str = Target?.Name?.ToString();
        for (int i = 0; i < 8; i++)
        {
            var e = this[i];
            str += "\n    " + e.ToString();
        }
        return str;
    }

    public bool GetSpecificTypeEffect(ActionEffectType type, out ActionEffect effect)
    {
        for (int i = 0; i < 8; i++)
        {
            var e = this[i];
            if(e.Type == type)
            {
                effect = e;
                return true;
            }
        }
        effect = default;
        return false;
    }
}

