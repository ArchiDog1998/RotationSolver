using Dalamud.Game.ClientState.JobGauge.Types;
using System.Collections.Generic;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.Attributes;
using XIVAutoAttack.Combos.Basic;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using static XIVAutoAttack.Combos.Melee.RPRCombos.RPRCombo_Default;

namespace XIVAutoAttack.Combos.Melee.RPRCombos;

[ComboDevInfo(@"https://github.com/ArchiDog1998/XIVAutoAttack/blob/main/XIVAutoAttack/Combos/Melee/RPRCombos/RPRCombo_Default.cs")]
internal sealed class RPRCombo_Default : RPRCombo_Base<CommandType>
{
    public override string Author => "逆光";

    internal enum CommandType : byte
    {
        None,
    }

    protected override SortedList<CommandType, string> CommandDescription => new SortedList<CommandType, string>()
    {
        //{CommandType.None, "" }, //写好注释啊！用来提示用户的。
    };
    public override SortedList<DescType, string> DescriptionDict => new()
    {
        {DescType.单体防御, $"{ArcaneCrest}"},
    };


    private protected override bool GeneralGCD(out IAction act)
    {
        //开场获得收获月
        if (Soulsow.ShouldUse(out act)) return true;

        //处于变身状态。
        if (Enshrouded)
        {
            if (ShadowofDeath.ShouldUse(out act)) return true;

            //夜游魂衣-虚无/交错收割 阴冷收割
            if (CrossReaping.ShouldUse(out act)) return true;
            if (VoidReaping.ShouldUse(out act)) return true;
            if (GrimReaping.ShouldUse(out act)) return true;

            if (JobGauge.LemureShroud == 1 && Communio.EnoughLevel)
            {
                if (!IsMoving && Communio.ShouldUse(out act, mustUse: true))
                {
                    return true;
                }
                //跑机制来不及读条？补个buff混一下
                else
                {
                    if (ShadowofDeath.ShouldUse(out act, mustUse: IsMoving)) return true;
                    if (WhorlofDeath.ShouldUse(out act, mustUse: IsMoving)) return true;
                }
            }
        }

        //处于补蓝状态，赶紧补蓝条。
        if (Guillotine.ShouldUse(out act)) return true;
        if (Gibbet.ShouldUse(out act)) return true;
        if (Gallows.ShouldUse(out act)) return true;

        //上Debuff
        if (WhorlofDeath.ShouldUse(out act)) return true;
        if (ShadowofDeath.ShouldUse(out act)) return true;

        //大丰收
        if (PlentifulHarvest.ShouldUse(out act, mustUse: true)) return true;

        //灵魂切割
        if (SoulSlice.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
        //灵魂钐割
        if (SoulScythe.ShouldUse(out act, emptyOrSkipCombo: true)) return true;

        //群体二连
        if (NightmareScythe.ShouldUse(out act)) return true;
        if (SpinningScythe.ShouldUse(out act)) return true;

        //单体三连
        if (InfernalSlice.ShouldUse(out act)) return true;
        if (WaxingSlice.ShouldUse(out act)) return true;
        if (Slice.ShouldUse(out act)) return true;

        //摸不到怪 先花掉收获月
        if (HarvestMoon.ShouldUse(out act, mustUse: true)) return true;
        if (Harpe.ShouldUse(out act)) return true;

        return false;
    }

    private protected override bool AttackAbility(byte abilityRemain, out IAction act)
    {
        if (SettingBreak)
        {
            //神秘环
            if (ArcaneCircle.ShouldUse(out act)) return true;
            //夜游魂衣
            if (Enshroud.ShouldUse(out act)) return true;
        }

        if (Enshrouded)
        {
            //夜游魂衣-夜游魂切割 夜游魂钐割
            if (LemuresSlice.ShouldUse(out act)) return true;
            if (LemuresScythe.ShouldUse(out act)) return true;
        }

        //暴食
        if (Gluttony.ShouldUse(out act, mustUse: true)) return true;
        //AOE
        if (GrimSwathe.ShouldUse(out act)) return true;
        //单体
        if (BloodStalk.ShouldUse(out act)) return true;
        act = null;
        return false;
    }

    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        //牵制
        if (!Enshrouded && !SoulReaver)
        {
            if (Feint.ShouldUse(out act)) return true;
        }
        act = null;
        return false;
    }

    private protected override bool DefenceSingleAbility(byte abilityRemain, out IAction act)
    {
        //神秘纹
        if (ArcaneCrest.ShouldUse(out act)) return true;
        return false;
    }
}
