using Dalamud.Game.ClientState.JobGauge.Types;
using XIVComboPlus;
using XIVComboPlus.Combos;

namespace XIVComboPlus.Combos.BLM;

internal class BlackManaFeature : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BlackManaFeature;


    protected internal override uint[] ActionIDs { get; } = new uint[1] { 149u };


    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == 149)
        {
            BLMGauge jobGauge = GetJobGauge<BLMGauge>();
            if (level >= 76 && jobGauge.InUmbralIce && jobGauge.IsEnochianActive)
            {
                return 16506u;
            }
        }
        return actionID;
    }
}
