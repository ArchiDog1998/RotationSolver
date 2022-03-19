//using XIVComboPlus;

//namespace XIVComboPlus.Combos;

//internal class DarkShadowbringerFeature : CustomCombo
//{
//    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DarkShadowbringerFeature;


//    protected internal override uint[] ActionIDs { get; } = new uint[2] { 3643u, 3641u };


//    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
//    {
//        if (actionID == 3643 || actionID == 3641)
//        {
//            if (level >= 90)
//            {
//                uint[] obj = new uint[4] { 0u, 25757u, 3639u, 25755u };
//                obj[0] = actionID;
//                return CalcBestAction(actionID, obj);
//            }
//            if (level >= 86)
//            {
//                return CalcBestAction(actionID, actionID, 3639u, 25755u);
//            }
//            if (level >= 56)
//            {
//                return CalcBestAction(actionID, actionID, 3639u);
//            }
//            if (level >= 52)
//            {
//                return 3639u;
//            }
//        }
//        return actionID;
//    }
//}
