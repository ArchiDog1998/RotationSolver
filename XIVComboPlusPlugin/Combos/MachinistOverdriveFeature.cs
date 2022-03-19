//using Dalamud.Game.ClientState.JobGauge.Types;
//using XIVComboPlus;

//namespace XIVComboPlus.Combos;

//internal class MachinistOverdriveFeature : CustomCombo
//{
//    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MachinistOverdriveFeature;


//    protected internal override uint[] ActionIDs { get; } = new uint[1] { 2864u };


//    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
//    {
//        if (actionID == 2864 || actionID == 16501)
//        {
//            MCHGauge jobGauge = GetJobGauge<MCHGauge>();
//            if (level >= 40 && jobGauge.IsRobotActive)
//            {
//                return OriginalHook(7415u);
//            }
//        }
//        return actionID;
//    }
//}
