//using XIVComboPlus;

//namespace XIVComboPlus.Combos;

//internal class SummonerShinyDemiCombo : CustomCombo
//{
//    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SummonerShinyDemiCombo;


//    protected internal override uint[] ActionIDs { get; } = new uint[2] { 25883u, 25884u };


//    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
//    {
//        if (actionID == 25883 || actionID == 25884)
//        {
//            if (OriginalHook(163u) == 25820 && level >= 70)
//            {
//                return 7429u;
//            }
//            if (OriginalHook(163u) == 16514)
//            {
//                return 16516u;
//            }
//        }
//        return actionID;
//    }
//}
