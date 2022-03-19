using Dalamud.Game.ClientState.JobGauge.Types;
using XIVComboPlus;
using XIVComboPlus.Combos;

namespace XIVComboPlus.Combos.BLM;

internal class BlackEnochianFeature : BLMCombo
{
    public override string ComboFancyName => "ÌìÓï×´Ì¬";

    public override string Description => "²âÊÔÓÃÎÄ×Ö";

    protected internal override uint[] ActionIDs { get; } = { (uint)Actions.Blizzard4, (uint)Actions.Fire4 };


    protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
    {
        if (level >= (byte)Levels.Blizzard4 && JobGauge.InUmbralIce)
        {
            return (uint)Actions.Blizzard4;
        }
        if (level >= (byte)Levels.Fire4)
        {
            return (uint)Actions.Fire4;
        }
        return actionID;
    }
}
