using Dalamud.Game.ClientState.JobGauge.Types;
using XIVComboPlus;

namespace XIVComboPlus.Combos;

internal class WhiteMageSolaceMiseryFeature : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WhiteMageSolaceMiseryFeature;


    protected internal override uint[] ActionIDs { get; } = new uint[1] { 16531u };


    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == 16531)
        {
            WHMGauge jobGauge = GetJobGauge<WHMGauge>();
            if (level >= 74 && jobGauge.BloodLily == 3)
            {
                return 16535u;
            }
        }
        return actionID;
    }
}
