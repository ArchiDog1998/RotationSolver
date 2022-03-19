//using XIVComboPlus;

//namespace XIVComboPlus.Combos;

//internal class ReaperSliceCombo : CustomCombo
//{
//    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.ReaperSliceCombo;


//    protected internal override uint[] ActionIDs { get; } = new uint[1] { 24375u };


//    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
//    {
//        if (actionID == 24375)
//        {
//            if (IsEnabled(CustomComboPreset.ReaperSoulReaverGibbetFeature) && level >= 70 && (HasEffect(2587) || HasEffect(2593)))
//            {
//                if (HasEffect(2588))
//                {
//                    return OriginalHook(24382u);
//                }
//                if (HasEffect(2589))
//                {
//                    return OriginalHook(24383u);
//                }
//                if (IsEnabled(CustomComboPreset.ReaperSoulReaverGibbetOption))
//                {
//                    return OriginalHook(24383u);
//                }
//                return OriginalHook(24382u);
//            }
//            if (comboTime > 0f)
//            {
//                if (lastComboMove == 24374 && level >= 30)
//                {
//                    return 24375u;
//                }
//                if (lastComboMove == 24373 && level >= 5)
//                {
//                    return 24374u;
//                }
//            }
//            return 24373u;
//        }
//        return actionID;
//    }
//}
