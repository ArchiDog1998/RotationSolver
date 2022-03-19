//using XIVComboPlus;

//namespace XIVComboPlus.Combos;

//internal class WArriorPrimalRendFeature : CustomCombo
//{
//    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WarriorPrimalRendFeature;


//    protected internal override uint[] ActionIDs { get; } = new uint[2] { 49u, 51u };


//    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
//    {
//        if (actionID == 49 || actionID == 51)
//        {
//            if (level >= 90 && HasEffect(2624))
//            {
//                return 25753u;
//            }
//            return OriginalHook(actionID);
//        }
//        return actionID;
//    }
//}
