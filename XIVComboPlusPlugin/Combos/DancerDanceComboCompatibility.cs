//using Dalamud.Game.ClientState.JobGauge.Types;

//namespace XIVComboPlus.Combos;

//internal class DancerDanceComboCompatibility : CustomCombo
//{
//    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DancerDanceComboCompatibility;


//    protected internal override uint[] ActionIDs { get; } = Service.Configuration.DancerDanceCompatActionIDs;


//    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
//    {
//        DNCGauge jobGauge = GetJobGauge<DNCGauge>();
//        if (level >= 15 && jobGauge.IsDancing)
//        {
//            uint[] actionIDs = ActionIDs;
//            if (actionID == actionIDs[0] || actionIDs[0] == 0 && actionID == 15989)
//            {
//                return OriginalHook(15989u);
//            }
//            if (actionID == actionIDs[1] || actionIDs[1] == 0 && actionID == 16013)
//            {
//                return OriginalHook(15990u);
//            }
//            if (actionID == actionIDs[2] || actionIDs[2] == 0 && actionID == 16007)
//            {
//                return OriginalHook(15991u);
//            }
//            if (actionID == actionIDs[3] || actionIDs[3] == 0 && actionID == 16008)
//            {
//                return OriginalHook(15992u);
//            }
//        }
//        return actionID;
//    }
//}
