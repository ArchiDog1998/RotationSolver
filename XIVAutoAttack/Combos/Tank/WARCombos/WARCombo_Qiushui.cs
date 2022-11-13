using XIVAutoAttack.Actions;
using XIVAutoAttack.Combos.Basic;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using static XIVAutoAttack.Combos.Tank.WARCombos.WARCombo_Qiushui;

namespace XIVAutoAttack.Combos.Tank.WARCombos;

internal class WARCombo_Qiushui :WARCombo_Base<CommandType>
{
    public enum CommandType : byte
    {
        None,
    }

    public override string Author => "秋水Demo";

    static WARCombo_Qiushui()
    {
        //保证原初之魂释放期间内，战场风暴能覆盖到
        InnerBeast.AddOtherCheck(b => !Player.WillStatusEndGCD(3, 0, true, StatusID.SurgingTempest));
    }

    private protected override bool AttackAbility(byte abilityRemain, out IAction act)
    {
        //动乱 
        if (Upheaval.ShouldUse(out act)) return true;

        return false;
    }

    private protected override bool GeneralGCD(out IAction act)
    {
        //暴风碎
        if (StormsEye.ShouldUse(out act)) return true;

        //暴风斩
        if (StormsPath.ShouldUse(out act)) return true;

        //凶残裂
        if (Maim.ShouldUse(out act)) return true;

        //重劈
        if (HeavySwing.ShouldUse(out act)) return true;

        return false;
    }
}
