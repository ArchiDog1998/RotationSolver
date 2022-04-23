using XIVComboPlus;
using XIVComboPlus.Combos;

namespace XIVComboPlus.Combos.BLM;

internal class BLMLeyLinesFeature : BLMCombo
{
    public override string ComboFancyName => "Ìæ»»ºÚÄ§ÎÆÎªÄ§ÎÆ²½";

    public override string Description => "Ìæ»»Ô¡ÑªÎª,µ±ºÚÄ§ÎÆÕýÔÚÆôÓÃÊ±£¬¾Í°ÑºÚÄ§ÎÆ±ä³ÉÄ§ÎÆ²½£¡";

    protected internal override uint[] ActionIDs => new uint[] { GeneralActions.Bloodbath.ActionID };

    private protected override bool EmergercyAbility(byte level, byte abilityRemain, BaseAction nextGCD, out BaseAction act)
    {
        if (Actions.BetweenTheLines.TryUseAction(level, out act)) return true;
        if (Actions.Leylines.TryUseAction(level, out act)) return true;
        return base.EmergercyAbility(level, abilityRemain, nextGCD, out act);
    }
}
