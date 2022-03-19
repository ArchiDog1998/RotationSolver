//using XIVComboPlus;

//namespace XIVComboPlus.Combos;

//internal class NinjaGCDNinjutsuFeature : CustomCombo
//{
//    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NinjaGCDNinjutsuFeature;


//    protected internal override uint[] ActionIDs { get; } = new uint[3] { 2255u, 3563u, 16488u };


//    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
//    {
//        if ((actionID == 2255 || actionID == 3563 || actionID == 16488) && level >= 30 && HasEffect(496))
//        {
//            return OriginalHook(2260u);
//        }
//        return actionID;
//    }
//}
