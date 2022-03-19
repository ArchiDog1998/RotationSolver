//using Dalamud.Game.ClientState.JobGauge.Types;
//using XIVComboPlus;

//namespace XIVComboPlus.Combos;

//internal class BardStraightShotUpgradeFeature : CustomCombo
//{
//    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BardStraightShotUpgradeFeature;


//    protected internal override uint[] ActionIDs { get; } = new uint[1] { 97u };


//    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
//    {
//        if (actionID == 97)
//        {
//            if (IsEnabled(CustomComboPreset.BardApexFeature))
//            {
//                BRDGauge jobGauge = GetJobGauge<BRDGauge>();
//                if (level >= 80 && jobGauge.SoulVoice == 100)
//                {
//                    return 16496u;
//                }
//                if (level >= 86 && HasEffect(2692))
//                {
//                    return 25784u;
//                }
//            }
//            if (level >= 2 && HasEffect(122))
//            {
//                return OriginalHook(98u);
//            }
//        }
//        return actionID;
//    }
//}
