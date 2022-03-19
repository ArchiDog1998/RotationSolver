//using Dalamud.Game.ClientState.JobGauge.Types;
//using XIVComboPlus;

//namespace XIVComboPlus.Combos;

//internal class SummonerCarbyFeature : CustomCombo
//{
//    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SummonerCarbyFeature;


//    protected internal override uint[] ActionIDs { get; } = new uint[1] { 25798u };


//    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
//    {
//        //IL_000d: Unknown result type (might be due to invalid IL or missing references)
//        if (actionID == 25798 && (int)GetJobGauge<SMNGauge>().ReturnSummon != 0)
//        {
//            return 25799u;
//        }
//        return actionID;
//    }
//}
