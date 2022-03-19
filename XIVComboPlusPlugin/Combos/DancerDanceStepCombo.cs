//using Dalamud.Game.ClientState.JobGauge.Types;
//using XIVComboPlus;

//namespace XIVComboPlus.Combos;

//internal class DancerDanceStepCombo : CustomCombo
//{
//    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DancerDanceStepCombo;


//    protected internal override uint[] ActionIDs { get; } = new uint[2] { 15997u, 15998u };


//    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
//    {
//        switch (actionID)
//        {
//            case 15997u:
//                {
//                    DNCGauge jobGauge2 = GetJobGauge<DNCGauge>();
//                    if (level >= 15 && jobGauge2.IsDancing && HasEffect(1818))
//                    {
//                        if (jobGauge2.CompletedSteps < 2)
//                        {
//                            return jobGauge2.NextStep;
//                        }
//                        return OriginalHook(15997u);
//                    }
//                    return 15997u;
//                }
//            case 15998u:
//                {
//                    DNCGauge jobGauge = GetJobGauge<DNCGauge>();
//                    if (level >= 70 && jobGauge.IsDancing && HasEffect(1819) && jobGauge.CompletedSteps < 4)
//                    {
//                        return jobGauge.NextStep;
//                    }
//                    return OriginalHook(15998u);
//                }
//            default:
//                return actionID;
//        }
//    }
//}
