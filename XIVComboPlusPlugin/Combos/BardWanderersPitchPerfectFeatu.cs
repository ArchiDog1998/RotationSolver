using Dalamud.Game.ClientState.JobGauge.Types;
using XIVComboPlus;

namespace XIVComboPlus.Combos;

internal class BardWanderersPitchPerfectFeature : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BardWanderersPitchPerfectFeature;


    protected internal override uint[] ActionIDs { get; } = new uint[1] { 3559u };


    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        //IL_0015: Unknown result type (might be due to invalid IL or missing references)
        //IL_001b: Invalid comparison between Unknown and I4
        if (actionID == 3559)
        {
            BRDGauge jobGauge = GetJobGauge<BRDGauge>();
            if (level >= 52 && (int)jobGauge.Song == 3)
            {
                return 7404u;
            }
        }
        return actionID;
    }
}
