//using Dalamud.Game.ClientState.JobGauge.Types;
//using XIVComboPlus;

//namespace XIVComboPlus.Combos;

//internal class WarriorOverpowerCombo : CustomCombo
//{
//    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.WarriorOverpowerCombo;


//    protected internal override uint[] ActionIDs { get; } = new uint[1] { 41u };


//    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
//    {
//        if (actionID == 41)
//        {
//            if (IsEnabled(CustomComboPreset.WarriorInnerReleaseFeature) && HasEffect(1177))
//            {
//                return OriginalHook(3550u);
//            }
//            byte beastGauge = GetJobGauge<WARGauge>().BeastGauge;
//            if (comboTime > 0f && lastComboMove == 41 && level >= 40)
//            {
//                if (beastGauge >= 90 && level >= 74 && IsEnabled(CustomComboPreset.WarriorGaugeOvercapFeature))
//                {
//                    return OriginalHook(3550u);
//                }
//                return 16462u;
//            }
//        }
//        return actionID;
//    }
//}
