//namespace XIVComboPlus.Combos;

//internal class GunbreakerNoMercyFeature : CustomCombo
//{
//    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.GunbreakerNoMercyFeature;


//    protected internal override uint[] ActionIDs { get; } = new uint[1] { 16138u };


//    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
//    {
//        if (actionID == 16138 && level >= 2 && HasEffect(1831))
//        {
//            if (level >= 62)
//            {
//                return CalcBestAction(16153u, 16153u, 16159u);
//            }
//            if (level >= 54)
//            {
//                return 16153u;
//            }
//        }
//        return actionID;
//    }
//}
