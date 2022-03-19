using Dalamud.Game.ClientState.JobGauge.Types;
using XIVComboPlus;
using XIVComboPlus.Combos;

namespace XIVComboPlus.Combos.BLM;

internal class BlackFireFeature : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BlackFireFeature;


    protected internal override uint[] ActionIDs { get; } = new uint[1] { 141u };


    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == 141)
        {
            BLMGauge jobGauge = GetJobGauge<BLMGauge>();
            if (level >= 35 && (!jobGauge.InAstralFire || HasEffect(165)))
            {
                return 152u;
            }
            return OriginalHook(141u);
        }
        return actionID;
    }
}
