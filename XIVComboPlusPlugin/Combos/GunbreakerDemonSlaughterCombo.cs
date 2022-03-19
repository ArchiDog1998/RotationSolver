//using Dalamud.Game.ClientState.JobGauge.Types;
//using XIVComboPlus;

//namespace XIVComboPlus.Combos;

//internal class GunbreakerDemonSlaughterCombo : CustomCombo
//{
//    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.GunbreakerDemonSlaughterCombo;


//    protected internal override uint[] ActionIDs { get; } = new uint[1] { 16149u };


//    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
//    {
//        if (actionID == 16149)
//        {
//            if (comboTime > 0f && lastComboMove == 16141 && level >= 40)
//            {
//                if (IsEnabled(CustomComboPreset.GunbreakerFatedCircleFeature) && level >= 72)
//                {
//                    GNBGauge jobGauge = GetJobGauge<GNBGauge>();
//                    int num = level >= 88 ? 3 : 2;
//                    if (jobGauge.Ammo == num)
//                    {
//                        return 16163u;
//                    }
//                }
//                return 16149u;
//            }
//            return 16141u;
//        }
//        return actionID;
//    }
//}
