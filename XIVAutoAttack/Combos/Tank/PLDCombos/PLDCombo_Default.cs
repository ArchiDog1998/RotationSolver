using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Types;
using Lumina.Excel.GeneratedSheets;
using System.Collections.Generic;
using System.Linq;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.Attributes;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Updaters;
using static XIVAutoAttack.Combos.Tank.PLDCombos.PLDCombo_Default;

namespace XIVAutoAttack.Combos.Tank.PLDCombos;

[ComboDevInfo(@"https://github.com/ArchiDog1998/XIVAutoAttack/blob/main/XIVAutoAttack/Combos/Tank/PLDCombos/PLDCombo_Default.cs")]
internal sealed class PLDCombo_Default : PLDCombo<CommandType>
{
    public override string Author => "汐ベMoon";

    internal enum CommandType : byte
    {
        None,
    }

    protected override SortedList<CommandType, string> CommandDescription => new SortedList<CommandType, string>()
    {
        //{CommandType.None, "" }, //写好注释啊！用来提示用户的。
    };


    protected override bool CanHealSingleSpell => TargetUpdater.PartyMembers.Length == 1 && base.CanHealSingleSpell;

    /// <summary>
    /// 在4人本的道中已经聚好怪可以使用相关技能(不移动且身边有大于3只小怪)
    /// </summary>
    private static bool CanUseSpellInDungeonsMiddle => TargetUpdater.PartyMembers.Length is > 1 and <= 4 && !Target.IsBoss() && !IsMoving
                                                    && TargetFilter.GetObjectInRadius(TargetUpdater.HostileTargets, 5).Length >= 3;

    /// <summary>
    /// 在4人本的道中
    /// </summary>
    private static bool InDungeonsMiddle => TargetUpdater.PartyMembers.Length is > 1 and <= 4 && !Target.IsBoss();

    private bool SlowLoop = false;

    public override SortedList<DescType, string> DescriptionDict => new()
    {
        {DescType.单体治疗, $"{Clemency}"},
        {DescType.范围防御, $"{DivineVeil}, {PassageofArms}"},
        {DescType.单体防御, $"{Sentinel}, {Sheltron}"},
        {DescType.移动技能, $"{Intervene}"},
    };

    private protected override bool GeneralGCD(out IAction act)
    {
        //三个大招
        if (BladeofValor.ShouldUse(out act, mustUse: true)) return true;
        if (BladeofFaith.ShouldUse(out act, mustUse: true)) return true;
        if (BladeofTruth.ShouldUse(out act, mustUse: true)) return true;

        //魔法三种姿势
        if (CanUseConfiteor(out act)) return true;

        //AOE 二连
        if (Prominence.ShouldUse(out act)) return true;
        if (TotalEclipse.ShouldUse(out act)) return true;

        //赎罪剑
        if (Atonement.ShouldUse(out act))
        {
            if (!SlowLoop && Player.HaveStatus(StatusIDs.FightOrFlight)
                   && IsLastWeaponSkill(true, Atonement, RoyalAuthority)
                   && !Player.WillStatusEndGCD(2, 0, true, StatusIDs.FightOrFlight)) return true;
            if (!SlowLoop && Player.FindStatusStack(StatusIDs.SwordOath) > 1) return true;

            if (SlowLoop) return true;
        }
        //单体三连
        if (GoringBlade.ShouldUse(out act)) return true;
        if (RageofHalone.ShouldUse(out act)) return true;
        if (RiotBlade.ShouldUse(out act)) return true;
        if (FastBlade.ShouldUse(out act)) return true;

        //投盾
        if (CommandController.Move && MoveAbility(1, out act)) return true;
        if (ShieldLob.ShouldUse(out act)) return true;

        return false;
    }

    private protected override bool MoveAbility(byte abilityRemain, out IAction act)
    {
        //调停
        if (Intervene.ShouldUse(out act, emptyOrSkipCombo: true)) return true;

        return false;
    }

    private protected override bool HealSingleGCD(out IAction act)
    {
        //深仁厚泽
        if (Clemency.ShouldUse(out act)) return true;

        return false;
    }

    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        //圣光幕帘
        if (DivineVeil.ShouldUse(out act)) return true;

        //武装戍卫
        if (PassageofArms.ShouldUse(out act)) return true;

        if (Reprisal.ShouldUse(out act, mustUse: true)) return true;

        return false;
    }

    private protected override bool AttackAbility(byte abilityRemain, out IAction act)
    {
        if (SettingBreak)
        {
            //战逃反应 加Buff
            if (abilityRemain == 1 && CanUseFightorFlight(out act))
            {
                return true;
            }

            //安魂祈祷
            //if (SlowLoop && CanUseRequiescat(out act)) return true;
            if (abilityRemain == 1 && CanUseRequiescat(out act)) return true;
        }


        //厄运流转
        if (CircleofScorn.ShouldUse(out act, mustUse: true))
        {
            if (InDungeonsMiddle) return true;

            if (FightorFlight.ElapsedAfterGCD(2)) return true;

            //if (SlowLoop && inOpener && IsLastWeaponSkill(false, Actions.RiotBlade)) return true;

            //if (!SlowLoop && inOpener && OpenerStatus && IsLastWeaponSkill(true, Actions.RiotBlade)) return true;

        }

        //深奥之灵
        if (SpiritsWithin.ShouldUse(out act, mustUse: true))
        {
            //if (SlowLoop && inOpener && IsLastWeaponSkill(true, Actions.RiotBlade)) return true;

            if (InDungeonsMiddle) return true;

            if (FightorFlight.ElapsedAfterGCD(3)) return true;
        }

        //调停
        if (Intervene.Target.DistanceToPlayer() < 1 && !IsMoving && Target.HaveStatus(StatusIDs.GoringBlade))
        {
            if (FightorFlight.ElapsedAfterGCD(2) && Intervene.ShouldUse(out act, emptyOrSkipCombo: true)) return true;

            if (Intervene.ShouldUse(out act)) return true;
        }

        //Special Defense.
        if (JobGauge.OathGauge == 100 && Defense(out act) && Player.CurrentHp < Player.MaxHp) return true;

        act = null;
        return false;
    }
    private protected override bool EmergercyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        //神圣领域 如果谢不够了。
        if (HallowedGround.ShouldUse(out act)) return true;
        return false;
    }
    private protected override bool DefenceSingleAbility(byte abilityRemain, out IAction act)
    {
        if (Defense(out act)) return true;

        if (abilityRemain == 1)
        {
            //预警（减伤30%）
            if (Sentinel.ShouldUse(out act)) return true;

            //铁壁（减伤20%）
            if (Rampart.ShouldUse(out act)) return true;
        }
        //降低攻击
        //雪仇
        if (Reprisal.ShouldUse(out act)) return true;

        //干预（减伤10%）
        if (!HaveShield && Intervention.ShouldUse(out act)) return true;

        act = null;
        return false;
    }

    /// <summary>
    /// 判断能否使用战逃反应
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    private bool CanUseFightorFlight(out IAction act)
    {
        if (FightorFlight.ShouldUse(out act))
        {
            //在4人本道中
            if (InDungeonsMiddle)
            {
                if (CanUseSpellInDungeonsMiddle && !Player.HaveStatus(StatusIDs.Requiescat)
                    && !Player.HaveStatus(StatusIDs.ReadyForBladeofFaith)
                    && Player.CurrentMp < 2000) return true;

                return false;
            }

            if (SlowLoop)
            {
                //if (openerFinished && Actions.Requiescat.ElapsedAfterGCD(12)) return true;

            }
            else
            {
                //起手在先锋剑后
                return true;

            }


        }

        act = null;
        return false;
    }

    /// <summary>
    /// 判断能否使用安魂祈祷
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    private bool CanUseRequiescat(out IAction act)
    {
        //安魂祈祷
        if (Requiescat.ShouldUse(out act, mustUse: true))
        {
            //在4人本道中
            if (InDungeonsMiddle)
            {
                if (CanUseSpellInDungeonsMiddle) return true;

                return false;
            }

            //长循环
            if (SlowLoop)
            {
                //if (inOpener && IsLastWeaponSkill(true, Actions.FastBlade)) return true;

                //if (openerFinished && Actions.FightorFlight.ElapsedAfterGCD(12)) return true;
            }
            else
            {
                //在战逃buff时间剩17秒以下时释放
                if (Player.HaveStatus(StatusIDs.FightOrFlight) && Player.WillStatusEnd(17, false, StatusIDs.FightOrFlight) && Target.HaveStatus(StatusIDs.GoringBlade))
                {
                    //在起手中时,王权剑后释放
                    return true;
                }
            }

        }

        act = null;
        return false;
    }


    /// <summary>
    /// 悔罪,圣灵,圣环
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    private bool CanUseConfiteor(out IAction act)
    {
        act = null;
        if (Player.HaveStatus(StatusIDs.SwordOath)) return false;

        //有安魂祈祷buff,且没在战逃中
        if (Player.HaveStatus(StatusIDs.Requiescat) && !Player.HaveStatus(StatusIDs.FightOrFlight))
        {
            if (SlowLoop && !IsLastWeaponSkill(true, GoringBlade) && !IsLastWeaponSkill(true, Atonement)) return false;

            var statusStack = Player.FindStatusStack(StatusIDs.Requiescat);
            if (statusStack == 1 || Player.HaveStatus(StatusIDs.Requiescat) && Player.WillStatusEnd(3, false, StatusIDs.Requiescat) || Player.CurrentMp <= 2000)
            {
                if (Confiteor.ShouldUse(out act, mustUse: true)) return true;
            }
            else
            {
                if (HolyCircle.ShouldUse(out act)) return true;
                if (HolySpirit.ShouldUse(out act)) return true;
            }
        }

        act = null;
        return false;
    }

    private bool Defense(out IAction act)
    {
        act = null;
        if (JobGauge.OathGauge < 50) return false;

        if (HaveShield)
        {
            //盾阵
            if (Sheltron.ShouldUse(out act)) return true;
        }
        else
        {
            //保护
            if (Cover.ShouldUse(out act)) return true;
        }

        return false;
    }
}
