using XIVComboPlus;
using XIVComboPlus.Combos;

namespace XIVComboPlus.Combos.BLM;

internal class BLMLeyLinesFeature : BLMCombo
{
    public override string ComboFancyName => "替换黑魔纹为魔纹步";

    public override string Description => "当黑魔纹正在启用时，就把黑魔纹变成魔纹步！";

    protected internal override uint[] ActionIDs => new uint[] { Actions.Leylines.ActionID };

    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if(Actions.BetweenTheLines.TryUseAction(level, out uint act)) return act;
        //if (Actions.Leylines.TryUseAction(level, out act)) return act;
        return Actions.Leylines.ActionID;
    }
}
