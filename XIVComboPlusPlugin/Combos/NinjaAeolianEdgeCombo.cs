//using XIVComboPlus;

//namespace XIVComboPlus.Combos;

//internal class NinjaAeolianEdgeCombo : CustomCombo
//{
//    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NinjaAeolianEdgeCombo;


//    protected internal override uint[] ActionIDs { get; } = new uint[1] { 2255u };


//    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
//    {
//        int num2;
//        if (actionID == 2255)
//        {
//            if (IsEnabled(CustomComboPreset.NinjaBunshinFeature) && level >= 82)
//            {
//                float? num = BuffDuration(2723);
//                if (num.HasValue)
//                {
//                    float valueOrDefault = num.GetValueOrDefault();
//                    if (valueOrDefault < 6f)
//                    {
//                        num2 = valueOrDefault > 0f ? 1 : 0;
//                        goto IL_004d;
//                    }
//                }
//                num2 = 0;
//                goto IL_004d;
//            }
//            goto IL_006f;
//        }
//        return actionID;
//    IL_004d:
//        if (((uint)num2 | (TargetHasEffect(638) && HasEffect(2723) ? 1u : 0u)) != 0)
//        {
//            return 25774u;
//        }
//        goto IL_006f;
//    IL_006f:
//        if (IsEnabled(CustomComboPreset.NinjaArmorCrushRaijuFeature))
//        {
//            if (level >= 90 && HasEffect(2691))
//            {
//                return OriginalHook(25778u);
//            }
//            if (level >= 90 && HasEffect(2690))
//            {
//                return 25778u;
//            }
//        }
//        if (IsEnabled(CustomComboPreset.NinjaAeolianEdgeRaijuFeature))
//        {
//            if (level >= 90 && HasEffect(2691))
//            {
//                return OriginalHook(25777u);
//            }
//            if (level >= 90 && HasEffect(2690))
//            {
//                return 25777u;
//            }
//        }
//        if (comboTime > 0f)
//        {
//            if (lastComboMove == 2242 && level >= 26)
//            {
//                return 2255u;
//            }
//            if (lastComboMove == 2240 && level >= 4)
//            {
//                return 2242u;
//            }
//        }
//        return 2240u;
//    }
//}
