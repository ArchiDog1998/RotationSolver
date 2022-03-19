//using XIVComboPlus;

//namespace XIVComboPlus.Combos;

//internal class GunbreakerBowShockSonicBreakFeature : CustomCombo
//{
//    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.GunbreakerBowShockSonicBreakFeature;


//    protected internal override uint[] ActionIDs { get; } = new uint[2] { 16159u, 16153u };


//    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
//    {
//        if ((actionID == 16159 || actionID == 16153) && level >= 62)
//        {
//            return CalcBestAction(actionID, 16159u, 16153u);
//        }
//        return actionID;
//    }
//}
