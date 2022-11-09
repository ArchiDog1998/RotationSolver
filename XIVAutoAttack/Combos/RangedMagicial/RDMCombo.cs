using Dalamud.Game.ClientState.JobGauge.Types;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Configuration;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Updaters;
using static XIVAutoAttack.Combos.RangedMagicial.RDMCombo;

namespace XIVAutoAttack.Combos.RangedMagicial;

internal sealed class RDMCombo : JobGaugeCombo<RDMGauge, CommandType>
{
    public override ComboAuthor[] Authors => new ComboAuthor[] { ComboAuthor.None };

    internal enum CommandType : byte
    {
        None,
    }

    protected override SortedList<CommandType, string> CommandDescription => new SortedList<CommandType, string>()
    {
        //{CommandType.None, "" }, //写好注释啊！用来提示用户的。
    };
    public override uint[] JobIDs => new uint[] { 35 };
    protected override bool CanHealSingleSpell => TargetUpdater.PartyMembers.Length == 1 && base.CanHealSingleSpell;
    //看看现在有没有促进

    private protected override BaseAction Raise => Verraise;

    private static bool _startLong = false;

    public class RDMAction : BaseAction
    {
        internal override ushort[] BuffsNeed 
        {
            get => NeedBuffNotCast ? base.BuffsNeed : null;
            set => base.BuffsNeed = value; 
        }
        public bool NeedBuffNotCast => !_startLong || InCombat;

        internal RDMAction(uint actionID, bool isFriendly = false, bool shouldEndSpecial = false) : base(actionID, isFriendly, shouldEndSpecial)
        {
            BuffsNeed = GeneralActions.Swiftcast.BuffsProvide.Union(new[] { ObjectStatus.Acceleration }).ToArray();
        }
    }
    public static readonly BaseAction
        //赤复活
        Verraise = new(7523, true),

        //震荡
        Jolt = new(7503)
        {
            BuffsProvide = GeneralActions.Swiftcast.BuffsProvide.Union(new[] { ObjectStatus.Acceleration }).ToArray(),
        },

        //回刺
        Riposte = new(7504)
        {
            OtherCheck = b => JobGauge.BlackMana >= 20 && JobGauge.WhiteMana >= 20,
        },

        //赤闪雷
        Verthunder = new RDMAction(7505),

        //短兵相接
        CorpsAcorps = new(7506, shouldEndSpecial: true)
        {
            BuffsProvide = new[]
            {
                    ObjectStatus.Bind1,
                    ObjectStatus.Bind2,
            }
        },

        //赤疾风
        Veraero = new RDMAction(7507),

        //散碎
        Scatter = new RDMAction(7509),

        //赤震雷
        Verthunder2 = new(16524u)
        {
            BuffsProvide = Jolt.BuffsProvide,
        },

        //赤烈风
        Veraero2 = new(16525u)
        {
            BuffsProvide = Jolt.BuffsProvide,
        },

        //赤火炎
        Verfire = new(7510)
        {
            BuffsNeed = new[] { ObjectStatus.VerfireReady },
            BuffsProvide = Jolt.BuffsProvide,
        },

        //赤飞石
        Verstone = new(7511)
        {
            BuffsNeed = new[] { ObjectStatus.VerstoneReady },
            BuffsProvide = Jolt.BuffsProvide,
        },

        //交击斩
        Zwerchhau = new(7512)
        {
            OtherCheck = b => JobGauge.BlackMana >= 15 && JobGauge.WhiteMana >= 15,
        },

        //交剑
        Engagement = new(16527),

        //飞剑
        Fleche = new(7517),

        //连攻
        Redoublement = new(7516)
        {
            OtherCheck = b => JobGauge.BlackMana >= 15 && JobGauge.WhiteMana >= 15,
        },


        //促进
        Acceleration = new(7518)
        {
            BuffsProvide = new[] { ObjectStatus.Acceleration },
        },

        //划圆斩
        Moulinet = new(7513)
        {
            OtherCheck = b => JobGauge.BlackMana >= 20 && JobGauge.WhiteMana >= 20,
        },

        //赤治疗
        Vercure = new(7514, true)
        {
            BuffsProvide = GeneralActions.Swiftcast.BuffsProvide.Union(Acceleration.BuffsProvide).ToArray(),
        },

        //六分反击
        ContreSixte = new(7519u),

        //鼓励
        Embolden = new(7520, true),

        //续斩
        Reprise = new(16529),

        //抗死
        MagickBarrier = new(25857),

        //赤核爆
        Verflare = new(7525),

        //赤神圣
        Verholy = new(7526),

        //焦热
        Scorch = new(16530)
        {
            OtherIDsCombo = new uint[] { Verholy.ID },
        },

        //决断
        Resolution = new(25858),

        //魔元化
        Manafication = new(7521)
        {
            OtherCheck = b => JobGauge.WhiteMana <= 50 && JobGauge.BlackMana <= 50 && InCombat && JobGauge.ManaStacks == 0,
            OtherIDsNot = new uint[] { Riposte.ID, Zwerchhau.ID, Scorch.ID, Verflare.ID, Verholy.ID },
        };
    public override SortedList<DescType, string> Description => new ()
    {
        {DescType.单体治疗, $"{Vercure}"},
        {DescType.范围防御, $"{MagickBarrier}"},
        {DescType.移动技能, $"{CorpsAcorps}"},
    };

    private protected override ActionConfiguration CreateConfiguration()
    {
        return base.CreateConfiguration()
            .SetBool("StartLong", false, "长读条起手")
            .SetBool("UseVercure", true, "使用赤治疗获得即刻");
    }

    private protected override bool EmergercyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        //鼓励要放到魔回刺或者魔Z斩或魔划圆斩之后
        if (nextGCD.IsAnySameAction(true, Zwerchhau, Redoublement, Moulinet))
        {
            if (Service.Configuration.AutoBreak && Embolden.ShouldUse(out act, mustUse: true)) return true;
        }
        //开场爆发的时候释放。
        if (Service.Configuration.AutoBreak && GetRightValue(JobGauge.WhiteMana) && GetRightValue(JobGauge.BlackMana))
        {
            if (Manafication.ShouldUse(out act)) return true;
            if (Embolden.ShouldUse(out act, mustUse: true)) return true;
        }
        //倍增要放到魔连攻击之后
        if (JobGauge.ManaStacks == 3 || Level < 68 && !nextGCD.IsAnySameAction(true, Zwerchhau, Riposte))
        {
            if (Manafication.ShouldUse(out act)) return true;
        }

        act = null;
        return false;
    }

    private bool GetRightValue(byte value)
    {
        return value >= 6 && value <= 12;
    }

    private protected override bool AttackAbility(byte abilityRemain, out IAction act)
    {
        if (SettingBreak)
        {
            if (Manafication.ShouldUse(out act)) return true;
            if (Embolden.ShouldUse(out act, mustUse: true)) return true;
        }

        if (JobGauge.ManaStacks == 0 && (JobGauge.BlackMana < 50 || JobGauge.WhiteMana < 50) && !Manafication.WillHaveOneChargeGCD(1, 1))
        {
            //促进满了就用。 
            if (abilityRemain == 2 && Acceleration.ShouldUse(out act, emptyOrSkipCombo: true)) return true;

            //即刻咏唱
            if (!Player.HaveStatus(ObjectStatus.Acceleration)
                && GeneralActions.Swiftcast.ShouldUse(out act, mustUse: true)) return true;
        }

        //攻击四个能力技。
        if (ContreSixte.ShouldUse(out act, mustUse: true)) return true;
        if (Fleche.ShouldUse(out act)) return true;
        //Empty: BaseAction.HaveStatusSelfFromSelf(1239)
        if (Engagement.ShouldUse(out act, emptyOrSkipCombo: true)) return true;

        if (CorpsAcorps.ShouldUse(out act) && !IsMoving)
        {
            if (CorpsAcorps.Target.DistanceToPlayer() < 1)
            {
                return true;
            }
        }

        return false;
    }

    private protected override bool GeneralGCD(out IAction act)
    {
        _startLong = Config.GetBoolByName("StartLong");

        act = null;
        if (JobGauge.ManaStacks == 3) return false;

        #region 常规输出
        if (!Verthunder2.ShouldUse(out _))
        {
            if (Verfire.ShouldUse(out act)) return true;
            if (Verstone.ShouldUse(out act)) return true;
        }

        //试试看散碎
        if (Scatter.ShouldUse(out act)) return true;
        //平衡魔元
        if (JobGauge.WhiteMana < JobGauge.BlackMana)
        {
            if (Veraero2.ShouldUse(out act)) return true;
            if (Veraero.ShouldUse(out act)) return true;
        }
        else
        {
            if (Verthunder2.ShouldUse(out act)) return true;
            if (Verthunder.ShouldUse(out act)) return true;
        }
        if (Jolt.ShouldUse(out act)) return true;
        #endregion

        //赤治疗，加即刻。
        if (Config.GetBoolByName("UseVercure") && Vercure.ShouldUse(out act)) return true;

        return false;
    }

    private protected override bool HealSingleGCD(out IAction act)
    {
        if (Vercure.ShouldUse(out act, mustUse: true)) return true;
        return false;
    }

    private protected override bool MoveAbility(byte abilityRemain, out IAction act)
    {
        if (CorpsAcorps.ShouldUse(out act, mustUse: true)) return true;
        return false;
    }
    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        //混乱
        if (GeneralActions.Addle.ShouldUse(out act)) return true;
        if (MagickBarrier.ShouldUse(out act, mustUse:true)) return true;
        return false;
    }

    private protected override bool EmergercyGCD(out IAction act)
    {
        byte level = Level;
        #region 远程三连
        //如果魔元结晶满了。
        if (JobGauge.ManaStacks == 3)
        {
            if (JobGauge.BlackMana > JobGauge.WhiteMana && level >= 70)
            {
                if (Verholy.ShouldUse(out act, mustUse: true)) return true;
            }
            if (Verflare.ShouldUse(out act, mustUse: true)) return true;
        }

        //焦热
        if (Scorch.ShouldUse(out act, mustUse: true)) return true;

        //决断
        if (Resolution.ShouldUse(out act, mustUse: true)) return true;
        #endregion

        #region 近战三连


        if (Moulinet.ShouldUse(out act)) return true;
        if (Zwerchhau.ShouldUse(out act)) return true;
        if (Redoublement.ShouldUse(out act)) return true;

        //如果倍增好了，或者魔元满了，或者正在爆发，或者处于开场爆发状态，就马上用！
        bool mustStart = Player.HaveStatus(1971) || JobGauge.BlackMana == 100 || JobGauge.WhiteMana == 100 || !Embolden.IsCoolDown;

        //在魔法元没有溢出的情况下，要求较小的魔元不带触发，也可以强制要求跳过判断。
        if (!mustStart)
        {
            if (JobGauge.BlackMana == JobGauge.WhiteMana) return false;

            //要求较小的魔元不带触发，也可以强制要求跳过判断。
            if (JobGauge.WhiteMana < JobGauge.BlackMana)
            {
                if (Player.HaveStatus(ObjectStatus.VerstoneReady))
                {
                    return false;
                }
            }
            if (JobGauge.WhiteMana > JobGauge.BlackMana)
            {
                if (Player.HaveStatus(ObjectStatus.VerfireReady))
                {
                    return false;
                }
            }

            //看看有没有即刻相关的技能。
            foreach (var buff in Vercure.BuffsProvide)
            {
                if (Player.HaveStatus(buff))
                {
                    return false;
                }
            }

            //如果倍增的时间快到了，但还是没好。
            if (Embolden.WillHaveOneChargeGCD(10))
            {
                return false;
            }
        }
        #endregion

        #region 开启爆发
        //要来可以使用近战三连了。
        if (Moulinet.ShouldUse(out act))
        {
            if (JobGauge.BlackMana >= 60 && JobGauge.WhiteMana >= 60) return true;
        }
        else
        {
            if (JobGauge.BlackMana >= 50 && JobGauge.WhiteMana >= 50 && Riposte.ShouldUse(out act)) return true;
        }
        if(JobGauge.ManaStacks > 0 && Riposte.ShouldUse(out act)) return true;
        #endregion

        return false;
    }
}
