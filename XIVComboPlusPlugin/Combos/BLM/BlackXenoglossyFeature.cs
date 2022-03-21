using Dalamud.Game.ClientState.JobGauge.Types;
using XIVComboPlus;
using XIVComboPlus.Combos;

namespace XIVComboPlus.Combos.BLM;

internal class BlackXenoglossyFeature : BLMCombo
{
    public override string ComboFancyName => "替换异言为秽浊";

    public override string Description => "如果等级不够高，那就替换异言为秽浊。";

    protected internal override uint[] ActionIDs => new uint[] { Actions.Xenoglossy.ActionID };


    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if(Actions.Xenoglossy.TryUseAction(level, out _)) return Actions.Xenoglossy.ActionID;
        return Actions.Foul.ActionID;
    }
}
