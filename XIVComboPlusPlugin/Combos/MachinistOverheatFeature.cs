//using Dalamud.Game.ClientState.JobGauge.Types;
//using XIVComboPlus;

//namespace XIVComboPlus.Combos;

//internal class MachinistOverheatFeature : CustomCombo
//{
//    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MachinistOverheatFeature;


//    protected internal override uint[] ActionIDs { get; } = new uint[2] { 7410u, 16497u };


//    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
//    {
//        if (actionID == 7410 || actionID == 16497)
//        {
//            MCHGauge jobGauge = GetJobGauge<MCHGauge>();
//            if (level >= 30 && !jobGauge.IsOverheated)
//            {
//                return 17209u;
//            }
//            if (level < 52)
//            {
//                return 7410u;
//            }
//        }
//        return actionID;
//    }
//}
