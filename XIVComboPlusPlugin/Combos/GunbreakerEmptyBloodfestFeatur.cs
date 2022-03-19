//using Dalamud.Game.ClientState.JobGauge.Types;
//using XIVComboPlus;

//namespace XIVComboPlus.Combos;

//internal class GunbreakerEmptyBloodfestFeature : CustomCombo
//{
//    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.GunbreakerEmptyBloodfestFeature;


//    protected internal override uint[] ActionIDs { get; } = new uint[2] { 16162u, 16163u };


//    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
//    {
//        if (actionID == 16162 || actionID == 16163)
//        {
//            if (IsEnabled(CustomComboPreset.GunbreakerBurstStrikeCont) && level >= 86 && HasEffect(2686))
//            {
//                return 25759u;
//            }
//            GNBGauge jobGauge = GetJobGauge<GNBGauge>();
//            if (level >= 76 && jobGauge.Ammo == 0)
//            {
//                return 16164u;
//            }
//        }
//        return actionID;
//    }
//}
