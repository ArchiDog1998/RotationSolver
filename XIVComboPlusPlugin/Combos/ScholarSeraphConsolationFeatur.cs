//using Dalamud.Game.ClientState.JobGauge.Types;
//using XIVComboPlus;

//namespace XIVComboPlus.Combos;

//internal class ScholarSeraphConsolationFeature : CustomCombo
//{
//    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ScholarSeraphConsolationFeature;


//    protected internal override uint[] ActionIDs { get; } = new uint[1] { 16543u };


//    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
//    {
//        if (actionID == 16543)
//        {
//            SCHGauge jobGauge = GetJobGauge<SCHGauge>();
//            if (level >= 80 && jobGauge.SeraphTimer > 0)
//            {
//                return 16546u;
//            }
//        }
//        return actionID;
//    }
//}
