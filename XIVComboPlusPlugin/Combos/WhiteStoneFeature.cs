//using Dalamud.Game.ClientState.Conditions;
//using XIVComboPlus;

//namespace XIVComboPlus.Combos;

//internal class WhiteStoneFeature : CustomCombo
//{
//    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WhiteStoneFeature;


//    protected internal override uint[] ActionIDs { get; } = new uint[5] { 119u, 127u, 3568u, 7431u, 16533u };


//    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
//    {
//        if (actionID == 119 || actionID == 127 || actionID == 3568 || actionID == 7431 || actionID == 16533)
//        {
//            if ((TargetBuffDuration(1871) < 3f && level > 71 || TargetBuffDuration(144) < 3f && level > 45 && level <= 71 || TargetBuffDuration(143) < 3f && level > 3 && level < 46) && HasCondition((ConditionFlag)26))
//            {
//                return OriginalHook(121u);
//            }
//            return OriginalHook(119u);
//        }
//        return actionID;
//    }
//}
