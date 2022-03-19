using Dalamud.Game.ClientState.JobGauge.Types;
using XIVComboPlus;

namespace XIVComboPlus.Combos;

internal class MachinistSpreadShotFeature : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MachinistSpreadShotFeature;


    protected internal override uint[] ActionIDs { get; } = new uint[1] { 2870u };


    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (actionID == 2870)
        {
            MCHGauge jobGauge = GetJobGauge<MCHGauge>();
            if (level >= 52 && jobGauge.IsOverheated)
            {
                return 16497u;
            }
        }
        return actionID;
    }
}
