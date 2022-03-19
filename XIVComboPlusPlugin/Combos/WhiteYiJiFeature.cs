//namespace XIVComboPlus.Combos;

//internal class WhiteYiJiFeature : CustomCombo
//{
//    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WhiteYiJiFeature;


//    protected internal override uint[] ActionIDs { get; } = new uint[1] { 135u };


//    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
//    {
//        if (actionID == 133)
//        {
//            if (BuffDuration(150) >= 5f || lastComboMove == 133)
//            {
//                return 124u;
//            }
//            return 133u;
//        }
//        return actionID;
//    }
//}
