//using XIVComboPlus;

//namespace XIVComboPlus.Combos;

//internal class AllSwiftcastFeature : CustomCombo
//{
//    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.AllSwiftcastFeature;


//    protected internal override uint[] ActionIDs { get; } = new uint[5] { 125u, 173u, 3603u, 7523u, 24287u };


//    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
//    {
//        if ((actionID == 125 || actionID == 173 || actionID == 3603 || actionID == 7523 || actionID == 24287) && (GetCooldown(7561u).CooldownRemaining == 0f && !HasEffect(1249) || level <= 12 || level <= 64 && actionID == 7523))
//        {
//            return 7561u;
//        }
//        return actionID;
//    }
//}
