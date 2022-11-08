using Dalamud.Game.ClientState.JobGauge.Types;
using System.Collections.Generic;
using System.Linq;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Updaters;
using static XIVAutoAttack.Combos.RangedPhysicial.DNCCombo;

namespace XIVAutoAttack.Combos.RangedPhysicial;

internal sealed class DNCCombo : JobGaugeCombo<DNCGauge, CommandType>
{
    internal enum CommandType : byte
    {
        None,
    }

    protected override SortedList<CommandType, string> CommandDescription => new SortedList<CommandType, string>()
    {
        //{CommandType.None, "" }, //写好注释啊！用来提示用户的。
    };

    public override uint[] JobIDs => new uint[] { 38 };

    public static readonly BaseAction

        //瀑泻
        Cascade = new(15989)
        {
            BuffsProvide = new[] { ObjectStatus.SilkenSymmetry }
        },

        //喷泉
        Fountain = new(15990)
        {
            BuffsProvide = new[] { ObjectStatus.SilkenFlow }
        },

        //逆瀑泻
        ReverseCascade = new(15991)
        {
            BuffsNeed = new[] { ObjectStatus.SilkenSymmetry, ObjectStatus.SilkenSymmetry2 },
        },

        //坠喷泉
        Fountainfall = new(15992)
        {
            BuffsNeed = new[] { ObjectStatus.SilkenFlow, ObjectStatus.SilkenFlow2 }
        },

        //扇舞·序
        FanDance = new(16007)
        {
            OtherCheck = b => JobGauge.Feathers > 0,
            BuffsProvide = new[] { ObjectStatus.ThreefoldFanDance },
        },

        //风车
        Windmill = new(15993)
        {
            BuffsProvide = Cascade.BuffsProvide,
        },

        //落刃雨
        Bladeshower = new(15994)
        {
            BuffsProvide = Fountain.BuffsProvide,
        },

        //升风车
        RisingWindmill = new(15995)
        {
            BuffsNeed = ReverseCascade.BuffsNeed,
        },

        //落血雨
        Bloodshower = new(15996)
        {
            BuffsNeed = Fountainfall.BuffsNeed,
        },

        //扇舞·破
        FanDance2 = new(16008)
        {
            OtherCheck = b => JobGauge.Feathers > 0,
            BuffsProvide = new[] { ObjectStatus.ThreefoldFanDance },
        },

        //扇舞·急
        FanDance3 = new(16009)
        {
            BuffsNeed = FanDance2.BuffsProvide,
        },

        //扇舞·终
        FanDance4 = new(25791)
        {
            BuffsNeed = new[] { ObjectStatus.FourfoldFanDance },
        },

        //剑舞
        SaberDance = new(16005)
        {
            OtherCheck = b => JobGauge.Esprit >= 50,
        },

        //流星舞
        StarfallDance = new(25792)
        {
            BuffsNeed = new[] { ObjectStatus.FlourishingStarfall },
        },

        //前冲步
        EnAvant = new(16010, shouldEndSpecial: true),

        //蔷薇曲脚步
        Emboite = new(15999)
        {
            OtherCheck = b => JobGauge.NextStep == 15999,
        },

        //小鸟交叠跳
        Entrechat = new(16000)
        {
            OtherCheck = b => JobGauge.NextStep == 16000,
        },

        //绿叶小踢腿
        Jete = new(16001)
        {
            OtherCheck = b => JobGauge.NextStep == 16001,
        },

        //金冠趾尖转
        Pirouette = new(16002)
        {
            OtherCheck = b => JobGauge.NextStep == 16002,
        },

        //标准舞步
        StandardStep = new(15997)
        {
            BuffsProvide = new[]
            {
                    ObjectStatus.StandardStep,
                    ObjectStatus.TechnicalStep,
            },
        },

        //技巧舞步
        TechnicalStep = new(15998)
        {
            BuffsNeed = new[]
            {
                    ObjectStatus.StandardFinish,
            },
            BuffsProvide = StandardStep.BuffsProvide,
        },

        //防守之桑巴
        ShieldSamba = new(16012, true)
        {
            BuffsProvide = new[]
            {
                    ObjectStatus.Troubadour,
                    ObjectStatus.Tactician1,
                    ObjectStatus.Tactician2,
                    ObjectStatus.ShieldSamba,
            },
        },

        //治疗之华尔兹
        CuringWaltz = new(16015, true),

        //闭式舞姿
        ClosedPosition = new(16006, true)
        {
            ChoiceTarget = Targets =>
            {
                Targets = Targets.Where(b => b.ObjectId != Player.ObjectId && b.CurrentHp != 0 &&
                //Remove Weak
                b.StatusList.Select(status => status.StatusId).Intersect(new uint[] { ObjectStatus.Weakness, ObjectStatus.BrinkofDeath }).Count() == 0 &&
                //Remove other partner.
                b.StatusList.Where(s => s.StatusId == ObjectStatus.ClosedPosition2 && s.SourceID != Player.ObjectId).Count() == 0).ToArray();

                var targets = TargetFilter.GetJobCategory(Targets, Role.近战);
                if (targets.Length > 0) return targets[0];

                targets = TargetFilter.GetJobCategory(Targets, Role.远程);
                if (targets.Length > 0) return targets[0];

                targets = Targets;
                if (targets.Length > 0) return targets[0];

                return null;
            },
        },

        //进攻之探戈
        Devilment = new(16011, true),

        //百花争艳
        Flourish = new(16013)
        {
            BuffsNeed = new[] { ObjectStatus.StandardFinish },
            BuffsProvide = new[]
            {
                    ObjectStatus.ThreefoldFanDance,
                    ObjectStatus.FourfoldFanDance,
            },
            OtherCheck = b => InCombat,
        },

        //即兴表演
        Improvisation = new(16014, true),

        //提拉纳
        Tillana = new(25790)
        {
            BuffsNeed = new[] { ObjectStatus.FlourishingFinish },
        };

    public override SortedList<DescType, string> Description => new()
    {
        {DescType.范围防御, $"{ShieldSamba}"},
        {DescType.范围治疗, $"{CuringWaltz}, {Improvisation}"},
        {DescType.移动技能, $"{EnAvant}"},
    };

    private protected override bool AttackAbility(byte abilityRemain, out IAction act)
    {
        if (SettingBreak && !TechnicalStep.EnoughLevel
    && Devilment.ShouldUse(out act, emptyOrSkipCombo: true)) return true;

        //应急换舞伴
        if (Player.HaveStatus(ObjectStatus.ClosedPosition1))
        {
            foreach (var friend in TargetUpdater.PartyMembers)
            {
                if (friend.HaveStatus(ObjectStatus.ClosedPosition2))
                {
                    if (ClosedPosition.ShouldUse(out act) && ClosedPosition.Target != friend)
                    {
                        return true;
                    }
                    break;
                }
            }
        }
        else if (ClosedPosition.ShouldUse(out act)) return true;

        //尝试爆发
        if (Player.HaveStatus(ObjectStatus.TechnicalFinish)
        && Devilment.ShouldUse(out act, emptyOrSkipCombo: true)) return true;

        //百花
        if (Flourish.ShouldUse(out act, emptyOrSkipCombo: true)) return true;

        //扇舞·急
        if (FanDance4.ShouldUse(out act, mustUse: true)) return true;
        if (FanDance3.ShouldUse(out act, mustUse: true)) return true;

        //扇舞
        if (Player.HaveStatus(ObjectStatus.Devilment) || JobGauge.Feathers > 3 || !TechnicalStep.EnoughLevel)
        {
            if (FanDance2.ShouldUse(out act)) return true;
            if (FanDance.ShouldUse(out act)) return true;
        }

        return false;
    }

    private protected override bool MoveAbility(byte abilityRemain, out IAction act)
    {
        if (EnAvant.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
        return false;
    }

    private protected override bool HealAreaAbility(byte abilityRemain, out IAction act)
    {
        if (CuringWaltz.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
        if (Improvisation.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
        return false;
    }

    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        if (ShieldSamba.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
        return false;
    }

    private protected override bool GeneralGCD(out IAction act)
    {
        if (!InCombat && !Player.HaveStatus(ObjectStatus.ClosedPosition1)
            && ClosedPosition.ShouldUse(out act)) return true;

        if (StepGCD(out act)) return true;

        if (SettingBreak)
        {
            if (TechnicalStep.ShouldUse(out act, mustUse: true)) return true;
        }

        if (AttackGCD(out act, Player.HaveStatus(ObjectStatus.Devilment))) return true;

        return false;
    }

    private bool StepGCD(out IAction act)
    {
        act = null;
        if (!Player.HaveStatus(ObjectStatus.StandardStep, ObjectStatus.TechnicalStep)) return false;

        if (Player.HaveStatus(ObjectStatus.StandardStep) && JobGauge.CompletedSteps == 2)
        {
            act = StandardStep;
            return true;
        }
        else if (Player.HaveStatus(ObjectStatus.TechnicalStep) && JobGauge.CompletedSteps == 4)
        {
            act = TechnicalStep;
            return true;
        }
        else
        {
            if (Emboite.ShouldUse(out act)) return true;
            if (Entrechat.ShouldUse(out act)) return true;
            if (Jete.ShouldUse(out act)) return true;
            if (Pirouette.ShouldUse(out act)) return true;
        }

        return false;
    }

    private bool AttackGCD(out IAction act, bool breaking)
    {

        //剑舞
        if ((breaking || JobGauge.Esprit >= 80) &&
            SaberDance.ShouldUse(out act, mustUse: true)) return true;

        //提纳拉
        if (Tillana.ShouldUse(out act, mustUse: true)) return true;
        if (StarfallDance.ShouldUse(out act, mustUse: true)) return true;

        if (JobGauge.IsDancing) return false;

        bool canstandard = !TechnicalStep.WillHaveOneChargeGCD(2);

        if (!Player.HaveStatus(ObjectStatus.TechnicalFinish))
        {
            //标准舞步
            if (StandardStep.ShouldUse(out act, mustUse: true)) return true;
        }

        //用掉Buff
        if (Bloodshower.ShouldUse(out act)) return true;
        if (Fountainfall.ShouldUse(out act)) return true;

        if (RisingWindmill.ShouldUse(out act)) return true;
        if (ReverseCascade.ShouldUse(out act)) return true;


        //标准舞步
        if (canstandard && StandardStep.ShouldUse(out act, mustUse: true)) return true;


        //aoe
        if (Bladeshower.ShouldUse(out act)) return true;
        if (Windmill.ShouldUse(out act)) return true;

        //single
        if (Fountain.ShouldUse(out act)) return true;
        if (Cascade.ShouldUse(out act)) return true;

        return false;
    }
}
