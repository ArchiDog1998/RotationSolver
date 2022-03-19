using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboPlus.Combos;

internal class ScholarEnergyDrainFeature : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ScholarEnergyDrainFeature;


    protected internal override uint[] ActionIDs { get; } = new uint[1] { 167u };


    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == 167)
        {
            SCHGauge jobGauge = GetJobGauge<SCHGauge>();
            if (level >= 45 && jobGauge.Aetherflow == 0)
            {
                return 166u;
            }
        }
        return actionID;
    }
}
