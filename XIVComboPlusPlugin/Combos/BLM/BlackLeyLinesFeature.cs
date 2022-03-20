using XIVComboPlus;
using XIVComboPlus.Combos;

namespace XIVComboPlus.Combos.BLM;

internal class BlackLeyLinesFeature : BLMCombo
{
    public override string ComboFancyName => "替换黑魔纹为魔纹步";

    public override string Description => "当黑魔纹正在启用时，就把黑魔纹变成魔纹步！";

    protected internal override uint[] ActionIDs { get; } = { Actions.LeyLines };


    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (level >= Levels.BetweenTheLines && HasEffect(Buffs.LeyLines))
        {
            return Actions.BetweenTheLines;
        }
        return actionID;
    }
}
