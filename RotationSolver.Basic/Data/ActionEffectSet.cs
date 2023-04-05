using FFXIVClientStructs.FFXIV.Client.Game;
using Action = Lumina.Excel.GeneratedSheets.Action;

namespace RotationSolver.Basic.Data;

public unsafe struct ActionEffectSet
{
    public Action Action { get; }
    public ActionType Type { get; }
    public GameObject Target { get; }
    public TargetEffect[] TargetEffects { get; }
    public ActionEffectSet(ActionEffectHeader* effectHeader, ActionEffect* effectArray, ulong* effectTargets)
    {
        Type = effectHeader->actionType;
        Action = Service.GetSheet<Action>().GetRow(effectHeader->actionId);
        Target = Service.ObjectTable.SearchById(effectHeader->animationTargetId);

        TargetEffects = new TargetEffect[effectHeader->NumTargets];
        for (int i = 0; i < effectHeader->NumTargets; i++)
        {
            TargetEffects[i] = new TargetEffect(Service.ObjectTable.SearchById(effectTargets[i]), &effectArray[8 * i]);
        }
    }

    public override string ToString()
    {
        var str = $"Type: {Type}, Name: {Action?.Name}, Target:{Target?.Name}";
        if(TargetEffects != null)
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
    public ActionEffect* Effects;

    public TargetEffect(GameObject target, ActionEffect* effects)
    {
        Target = target;
        Effects = effects;
    }

    public override string ToString()
    {
        var str = Target?.Name?.ToString();
        for (int i = 0; i < 8; i++)
        {
            var e = Effects[i];
            str += "\n    " + e.ToString();
        }
        return str;
    }
}

