using Dalamud.Game.ClientState.JobGauge.Types;
using XIVComboPlus;

namespace XIVComboPlus.Combos;

internal class SamuraiIkishotenNamikiriFeature : CustomCombo
{
    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SamuraiIkishotenNamikiriFeature;


    protected internal override uint[] ActionIDs { get; } = new uint[1] { 16482u };


    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        //IL_002a: Unknown result type (might be due to invalid IL or missing references)
        //IL_0030: Invalid comparison between Unknown and I4
        if (actionID == 16482 && level >= 90)
        {
            SAMGauge jobGauge = GetJobGauge<SAMGauge>();
            if (level >= 80 && jobGauge.MeditationStacks >= 3)
            {
                return 16487u;
            }
            if ((int)jobGauge.Kaeshi == 4)
            {
                return 25782u;
            }
            if (HasEffect(2959))
            {
                return 25781u;
            }
        }
        return actionID;
    }
}
