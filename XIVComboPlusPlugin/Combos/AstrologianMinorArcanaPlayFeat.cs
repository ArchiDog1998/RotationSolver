//using Dalamud.Game.ClientState.JobGauge.Types;
//using XIVComboPlus;

//namespace XIVComboPlus.Combos;

//internal class AstrologianMinorArcanaPlayFeature : CustomCombo
//{
//    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.AstrologianMinorArcanaPlayFeature;


//    protected internal override uint[] ActionIDs { get; } = new uint[1] { 7443u };


//    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
//    {
//        //IL_0013: Unknown result type (might be due to invalid IL or missing references)
//        //IL_0027: Unknown result type (might be due to invalid IL or missing references)
//        if (actionID == 7443 && level >= 70 && (int)GetJobGauge<ASTGauge>().DrawnCard == 0)
//        {
//            ASTGauge jobGauge = GetJobGauge<ASTGauge>();
//            if (level >= 70 && (int)jobGauge.DrawnCrownCard != 0)
//            {
//                return OriginalHook(25869u);
//            }
//        }
//        return actionID;
//    }
//}
