//using Dalamud.Game.ClientState.JobGauge.Types;

//namespace XIVComboPlus.Combos;

//internal class AstrologianCardsOnDrawFeature : CustomCombo
//{
//    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.AstrologianCardsOnDrawFeature;


//    protected internal override uint[] ActionIDs { get; } = new uint[1] { 17055u };


//    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
//    {
//        //IL_0015: Unknown result type (might be due to invalid IL or missing references)
//        if (actionID == 17055)
//        {
//            ASTGauge jobGauge = GetJobGauge<ASTGauge>();
//            if (level >= 30 && (int)jobGauge.DrawnCard == 0)
//            {
//                return 3590u;
//            }
//        }
//        return actionID;
//    }
//}
