//using XIVComboPlus;

//namespace XIVComboPlus.Combos;

//internal class MachinistZiDongFeature : CustomCombo
//{
//    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.MachinistZiDongFeature;


//    protected internal override uint[] ActionIDs { get; } = new uint[2] { 2866u, 7411u };


//    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
//    {
//        if (actionID == 2866 || actionID == 7411)
//        {
//            IconReplacer.CooldownData cooldown = GetCooldown(2874u);
//            IconReplacer.CooldownData cooldown2 = GetCooldown(2890u);
//            if (cooldown.CooldownRemaining > cooldown2.CooldownRemaining && level >= 50)
//            {
//                return 2890u;
//            }
//        }
//        return actionID;
//    }
//}
