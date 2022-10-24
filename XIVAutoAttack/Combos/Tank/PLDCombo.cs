using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Types;
using Lumina.Excel.GeneratedSheets;
using System.Collections.Generic;
using System.Linq;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Combos.CustomCombo;

namespace XIVAutoAttack.Combos.Tank;

internal class PLDCombo : JobGaugeCombo<PLDGauge>
{
    internal override uint JobID => 19;

    internal override bool HaveShield => LocalPlayer.HaveStatus(ObjectStatus.IronWill);

    private protected override BaseAction Shield => Actions.IronWill;

    protected override bool CanHealSingleSpell => TargetHelper.PartyMembers.Length == 1 && base.CanHealSingleSpell;

    /// <summary>
    /// 在4人本的道中已经聚好怪可以使用相关技能(不移动且身边有大于3只小怪)
    /// </summary>
    private static bool CanUseSpellInDungeonsMiddle => TargetHelper.PartyMembers.Length is > 1 and <= 4 && !Target.IsBoss() && !IsMoving
                                                    && TargetFilter.GetObjectInRadius(TargetHelper.HostileTargets, 5).Length >= 3;

    /// <summary>
    /// 在4人本的道中
    /// </summary>
    private static bool InDungeonsMiddle => TargetHelper.PartyMembers.Length is > 1 and <= 4 && !Target.IsBoss();

    private static bool inOpener = false;
    private static bool openerFinished = false;
    private static float RequiescatDelayTime = 0;
    private static float FightorFlightDelayTime = 0;

    //private bool SlowLoop => Config.GetBoolByName("SlowLoop");
    private bool SlowLoop = true;

    internal struct Actions
    {
        public static readonly BaseAction
            //钢铁信念
            IronWill = new (28, shouldEndSpecial: true),

            //先锋剑
            FastBlade = new (9),

            //暴乱剑
            RiotBlade = new (15),

            //沥血剑
            GoringBlade = new (3538, isDot:true)
            {
                TargetStatus = new []
                {
                    ObjectStatus.GoringBlade,
                    ObjectStatus.BladeofValor,
                }
            },

            //战女神之怒
            RageofHalone = new (21),

            //王权剑
            RoyalAuthority = new (3539),

            //投盾
            ShieldLob = new (24)
            {
                FilterForTarget = b => TargetFilter.ProvokeTarget(b),
            },

            //战逃反应
            FightorFlight = new (20)
            {
                OtherCheck = b =>
                {
                    return true;
                },
                AfterUse = () =>
                {
                    RequiescatDelayTime = WeaponRemain(11);
                }
            },

            //全蚀斩
            TotalEclipse = new (7381),

            //日珥斩
            Prominence = new (16457),

            //预警
            Sentinel = new (17)
            {
                BuffsProvide = GeneralActions.Rampart.BuffsProvide,
                OtherCheck = BaseAction.TankDefenseSelf,
            },

            //厄运流转
            CircleofScorn = new (23)
            {
                //OtherCheck = b =>
                //{
                //    if (LocalPlayer.HaveStatus(ObjectStatus.FightOrFlight)) return true;

                //    if (FightorFlight.IsCoolDown) return true;

                //    return false;
                //}
            },

            //深奥之灵
            SpiritsWithin = new (29)
            {
                //OtherCheck = b =>
                //{
                //    if (LocalPlayer.HaveStatus(ObjectStatus.FightOrFlight)) return true;

                //    if (FightorFlight.IsCoolDown) return true;

                //    return false;
                //}
            },

            //神圣领域
            HallowedGround = new (30)
            {
                OtherCheck = BaseAction.TankBreakOtherCheck,
            },

            //圣光幕帘
            DivineVeil = new (3540),

            //深仁厚泽
            Clemency = new (3541, true, true),

            //干预
            Intervention = new (7382, true)
            {
                ChoiceTarget = TargetFilter.FindAttackedTarget,
            },

            //调停
            Intervene = new (16461, shouldEndSpecial: true)
            {
                ChoiceTarget = TargetFilter.FindMoveTarget,
            },

            //赎罪剑
            Atonement = new (16460)
            {
                BuffsNeed = new [] { ObjectStatus.SwordOath },
            },

            //偿赎剑
            Expiacion = new (25747),

            //英勇之剑
            BladeofValor = new (25750),

            //真理之剑
            BladeofTruth = new (25749),

            //信念之剑
            BladeofFaith = new (25748)
            {
                BuffsNeed = new [] { ObjectStatus.ReadyForBladeofFaith },
            },

            //安魂祈祷
            Requiescat = new (7383)
            {
                AfterUse = () =>
                {
                    FightorFlightDelayTime = WeaponRemain(11);
                }
            },

            //悔罪
            Confiteor = new (16459)
            {
                OtherCheck = b => LocalPlayer.CurrentMp >= 1000,
            },

            //圣环
            HolyCircle = new (16458)
            {
                OtherCheck = b => LocalPlayer.CurrentMp >= 2000,
            },

            //圣灵
            HolySpirit = new (7384)
            {
                OtherCheck = b => LocalPlayer.CurrentMp >= 2000,
            },

            //武装戍卫
            PassageofArms = new (7385),

            //保护
            Cover = new BaseAction(27, true)
            {
                ChoiceTarget = TargetFilter.FindAttackedTarget,
            },

            //盾阵
            Sheltron = new (3542);
        //盾牌猛击
        //ShieldBash = new BaseAction(16),
    }
    internal override SortedList<DescType, string> Description => new ()
    {
        {DescType.单体治疗, $"{Actions.Clemency.Action.Name}"},
        {DescType.范围防御, $"{Actions.DivineVeil.Action.Name}, {Actions.PassageofArms.Action.Name}"},
        {DescType.单体防御, $"{Actions.Sentinel.Action.Name}, {Actions.Sheltron.Action.Name}"},
        {DescType.移动, $"{Actions.Intervene.Action.Name}"},
    };

    private protected override bool BreakAbility(byte abilityRemain, out IAction act)
    {
        //战逃反应 加Buff
        if (abilityRemain == 1 && CanUseFightorFlight(out act))
        {
            return true;
        }

        //安魂祈祷
        if (SlowLoop && CanUseRequiescat(out act)) return true;
        if (abilityRemain == 1 && CanUseRequiescat(out act)) return true;
  

        act = null;
        return false;
    }

    private protected override bool GeneralGCD(uint lastComboActionID, out IAction act)
    {
        //起手重置
        if (!InBattle)
        {
            inOpener = false;
            openerFinished = false;
        }
        else if (Level > Actions.Requiescat.Level && !openerFinished && !inOpener)
        {
            inOpener = true;
        }

        //起手完成判断
        if (inOpener || !Actions.TotalEclipse.ShouldUse(out _))
        {
            if (IsLastWeaponSkill(true, Actions.Confiteor) || (!LocalPlayer.HaveStatus(ObjectStatus.Requiescat) && Actions.Requiescat.IsCoolDown && Actions.Requiescat.RecastTimeRemain <= 59))
            {
                inOpener = false;
                openerFinished = true;
            }
        }

        //三个大招
        if (Actions.BladeofValor.ShouldUse(out act, lastComboActionID, mustUse: true)) return true;
        if (Actions.BladeofFaith.ShouldUse(out act, mustUse: true)) return true;
        if (Actions.BladeofTruth.ShouldUse(out act, lastComboActionID, mustUse: true)) return true;

        //魔法三种姿势
        if (CanUseConfiteor(out act)) return true;


        //AOE 二连
        if (Actions.Prominence.ShouldUse(out act, lastComboActionID)) return true;
        if (Actions.TotalEclipse.ShouldUse(out act, lastComboActionID)) return true;


        //赎罪剑
        if (Actions.Atonement.ShouldUse(out act))
        {
            if (!SlowLoop && LocalPlayer.HaveStatus(ObjectStatus.FightOrFlight)
                   && IsLastWeaponSkill(true, Actions.Atonement, Actions.RoyalAuthority)
                   && LocalPlayer.FindStatusTime(ObjectStatus.FightOrFlight) >= WeaponRemain(2)) return true;
            if (!SlowLoop && LocalPlayer.FindStatusStack(ObjectStatus.SwordOath) > 1) return true;

            if (SlowLoop) return true;
        }
        //单体三连
        if (Actions.GoringBlade.ShouldUse(out act, lastComboActionID)) return true;
        if (Actions.RageofHalone.ShouldUse(out act, lastComboActionID)) return true;
        if (Actions.RiotBlade.ShouldUse(out act, lastComboActionID)) return true;
        if (Actions.FastBlade.ShouldUse(out act, lastComboActionID)) return true;

        //投盾
        if (IconReplacer.Move && MoveAbility(1, out act)) return true;
        if (Actions.ShieldLob.ShouldUse(out act)) return true;

        return false;
    }

    private protected override bool MoveAbility(byte abilityRemain, out IAction act)
    {
        //调停
        if (Actions.Intervene.ShouldUse(out act, emptyOrSkipCombo: true)) return true;

        return false;
    }

    private protected override bool HealSingleGCD(uint lastComboActionID, out IAction act)
    {
        //深仁厚泽
        if (Actions.Clemency.ShouldUse(out act)) return true;

        return false;
    }

    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        //圣光幕帘
        if (Actions.DivineVeil.ShouldUse(out act)) return true;

        //武装戍卫
        if (Actions.PassageofArms.ShouldUse(out act)) return true;

        if (GeneralActions.Reprisal.ShouldUse(out act, mustUse: true)) return true;

        return false;
    }

    private protected override bool ForAttachAbility(byte abilityRemain, out IAction act)
    {
        var OpenerStatus = LocalPlayer.HaveStatus(ObjectStatus.FightOrFlight) && LocalPlayer.FindStatusTime(ObjectStatus.FightOrFlight) <= 19 && !IsLastWeaponSkill(true, Actions.FastBlade) && StatusHelper.HaveStatus(Target, ObjectStatus.GoringBlade);

        //厄运流转
        if (Actions.CircleofScorn.ShouldUse(out act, mustUse: true))
        {
            if (SlowLoop && inOpener && IsLastWeaponSkill(false, Actions.RiotBlade)) return true;

            if (!SlowLoop && inOpener && OpenerStatus && IsLastWeaponSkill(true, Actions.RiotBlade)) return true;

            if (!inOpener) return true;
        }

        //深奥之灵
        if (Actions.SpiritsWithin.ShouldUse(out act, mustUse: true))
        {
            if (SlowLoop && inOpener && IsLastWeaponSkill(true, Actions.RiotBlade)) return true;

            if (!SlowLoop && inOpener && OpenerStatus && IsLastWeaponSkill(true, Actions.RoyalAuthority)) return true;

            if (!inOpener) return true;
        }

        //调停
        if (Actions.Intervene.Target.DistanceToPlayer() < 1 && !IsMoving)
        {
            if (inOpener && OpenerStatus && IsLastWeaponSkill(true, Actions.RiotBlade) && Actions.Intervene.ShouldUse(out act) && Actions.Intervene.RecastTimeRemain == 0) return true;

            if (inOpener && OpenerStatus && IsLastWeaponSkill(true, Actions.Atonement) && Actions.Intervene.ShouldUse(out act, emptyOrSkipCombo: true)) return true;

            if (!inOpener && Actions.Intervene.ShouldUse(out act)) return true;
        }

        //Special Defense.
        if (JobGauge.OathGauge == 100 && Defense(out act) && LocalPlayer.CurrentHp < LocalPlayer.MaxHp) return true;

        act = null;
        return false;
    }
    private protected override bool EmergercyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        //神圣领域 如果谢不够了。
        if (Actions.HallowedGround.ShouldUse(out act)) return true;
        return false;
    }
    private protected override bool DefenceSingleAbility(byte abilityRemain, out IAction act)
    {
        if (Defense(out act)) return true;

        if (abilityRemain == 1)
        {
            //预警（减伤30%）
            if (Actions.Sentinel.ShouldUse(out act)) return true;

            //铁壁（减伤20%）
            if (GeneralActions.Rampart.ShouldUse(out act)) return true;
        }
        //降低攻击
        //雪仇
        if (GeneralActions.Reprisal.ShouldUse(out act)) return true;

        //干预（减伤10%）
        if (!HaveShield && Actions.Intervention.ShouldUse(out act)) return true;

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
        if (Actions.FightorFlight.ShouldUse(out act))
        {
            //在4人本道中
            if (InDungeonsMiddle)
            {
                if (CanUseSpellInDungeonsMiddle && !LocalPlayer.HaveStatus(ObjectStatus.Requiescat) 
                    && !LocalPlayer.HaveStatus(ObjectStatus.ReadyForBladeofFaith)
                    && LocalPlayer.CurrentMp < 2000) return true;

                return false;
            }

            if (SlowLoop)
            {
                if (openerFinished && Actions.Requiescat.RecastTimeElapsed > FightorFlightDelayTime) return true;
            }
            else
            {
                //起手在先锋剑后
                if (inOpener && IsLastWeaponSkill(true, Actions.FastBlade)) return true;

                //没在起手,冷却好了就用
                if (!inOpener) return true;
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
        if (Actions.Requiescat.ShouldUse(out act, mustUse: true))
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
                if (inOpener && IsLastWeaponSkill(true, Actions.FastBlade)) return true;

                if (openerFinished && Actions.FightorFlight.RecastTimeElapsed > RequiescatDelayTime) return true;
            }
            else
            {
                //在战逃buff时间剩17秒以下时释放
                if (LocalPlayer.HaveStatus(ObjectStatus.FightOrFlight) && LocalPlayer.FindStatusTime(ObjectStatus.FightOrFlight) < 17 && StatusHelper.HaveStatus(Target, ObjectStatus.GoringBlade))
                {
                    //在起手中时,王权剑后释放
                    if (inOpener && IsLastWeaponSkill(true, Actions.RoyalAuthority)) return true;

                    //没在起手时在
                    if (!inOpener) return true;
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
        if (LocalPlayer.HaveStatus(ObjectStatus.SwordOath)) return false;

        //有安魂祈祷buff,且没在战逃中
        if (LocalPlayer.HaveStatus(ObjectStatus.Requiescat) && !LocalPlayer.HaveStatus(ObjectStatus.FightOrFlight))
        {
            if (SlowLoop && (!IsLastWeaponSkill(true, Actions.GoringBlade) && !IsLastWeaponSkill(true, Actions.Atonement))) return false;

            var statusStack = LocalPlayer.FindStatusStack(ObjectStatus.Requiescat);
            var statusRemainingTime = LocalPlayer.FindStatusTime(ObjectStatus.Requiescat);
            if (statusStack == 1 || (statusRemainingTime is > 0 and < 3) || LocalPlayer.CurrentMp <= 2000)
            {
                if (Actions.Confiteor.ShouldUse(out act, mustUse: true)) return true;
            }
            else
            {
                if (Actions.HolyCircle.ShouldUse(out act)) return true;
                if (Actions.HolySpirit.ShouldUse(out act)) return true;
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
            if (Actions.Sheltron.ShouldUse(out act)) return true;
        }
        else
        {
            //保护
            if (Actions.Cover.ShouldUse(out act)) return true;
        }

        return false;
    }
}
