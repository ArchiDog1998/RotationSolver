//using Dalamud.Game.ClientState.Conditions;
//using Dalamud.Game.ClientState.JobGauge.Types;

//namespace XIVComboPlus.Combos;

//internal class NinjaHideMugFeature : CustomCombo
//{
//    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.NinjaHideMugFeature;


//    protected internal override uint[] ActionIDs { get; } = new uint[1] { 2245u };


//    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
//    {
//        if (actionID == 2245)
//        {
//            byte ninki = GetJobGauge<NINGauge>().Ninki;
//            IconReplacer.CooldownData cooldown = GetCooldown(16493u);
//            if (HasCondition((ConditionFlag)26))
//            {
//                if (ninki >= 50 && level > 68 && IsEnabled(CustomComboPreset.NinjaLiangPuFeature))
//                {
//                    if (!cooldown.IsCooldown && level > 79)
//                    {
//                        return 16493u;
//                    }
//                    if (cooldown.IsCooldown)
//                    {
//                        return 7402u;
//                    }
//                }
//                return 2248u;
//            }
//        }
//        return actionID;
//    }
//}
