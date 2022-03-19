using Dalamud.Game.ClientState.Conditions;
using XIVComboPlus;

namespace XIVComboPlus.Combos;

internal class ASTdotFeature : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ASTdotFeature;


    protected internal override uint[] ActionIDs { get; } = new uint[4] { 3596u, 3598u, 16555u, 7442u };


    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == 3596 || actionID == 3598 || actionID == 16555 || actionID == 7442)
        {
            if ((TargetBuffDuration(1881) < 3f && level >= 72 || TargetBuffDuration(843) < 3f && level >= 46 && level < 72 || TargetBuffDuration(838) < 3f && level > 3 && level < 46) && HasCondition((ConditionFlag)26))
            {
                return OriginalHook(3599u);
            }
            return OriginalHook(3596u);
        }
        return actionID;
    }
}
