//using XIVComboPlus;

//namespace XIVComboPlus.Combos;

//internal class RedMageContreFlecheFeature : CustomCombo
//{
//    protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.RedMageContreFlecheFeature;


//    protected internal override uint[] ActionIDs { get; } = new uint[2] { 7517u, 7519u };


//    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
//    {
//        if (actionID == 7517 || actionID == 7519)
//        {
//            if (level >= 56)
//            {
//                return CalcBestAction(actionID, 7517u, 7519u);
//            }
//            if (level >= 45)
//            {
//                return 7517u;
//            }
//        }
//        return actionID;
//    }
//}
