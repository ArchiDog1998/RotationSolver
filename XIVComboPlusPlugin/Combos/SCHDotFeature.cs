using Dalamud.Game.ClientState.Conditions;
using XIVComboPlus;

namespace XIVComboPlus.Combos;

internal class SCHDotFeature : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SCHDotFeature;


    protected internal override uint[] ActionIDs { get; } = new uint[3] { 17869u, 3584u, 7435u };


    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == 17869 || actionID == 3584 || actionID == 7435)
        {
            if ((TargetBuffDuration(1895) < 2.5 && level >= 72 || TargetBuffDuration(179) < 2f && level > 1 && level < 26 || TargetBuffDuration(189) < 2f && level >= 26 && level < 72) && HasCondition((ConditionFlag)26))
            {
                return OriginalHook(17864u);
            }
            return OriginalHook(17869u);
        }
        return actionID;
    }
}
