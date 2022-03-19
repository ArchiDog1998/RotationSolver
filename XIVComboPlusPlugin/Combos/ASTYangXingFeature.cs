//using Dalamud.Game.ClientState.Conditions;
//using XIVComboPlus;

//namespace XIVComboPlus.Combos;

//internal class ASTYangXingFeature : CustomCombo
//{
//    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ASTYangXingFeature;


//    protected internal override uint[] ActionIDs { get; } = new uint[1] { 3601u };


//    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
//    {
//        if (actionID == 3601)
//        {
//            if ((BuffDuration(836) > 5f || BuffDuration(837) > 5f || lastComboMove == 3601 || lastComboMove == 17152) && HasCondition((ConditionFlag)26))
//            {
//                return OriginalHook(3600u);
//            }
//            return OriginalHook(3601u);
//        }
//        return actionID;
//    }
//}
