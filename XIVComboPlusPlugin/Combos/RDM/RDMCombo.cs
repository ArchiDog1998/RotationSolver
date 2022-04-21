using Dalamud.Game.ClientState.JobGauge.Types;
using System.Linq;
namespace XIVComboPlus.Combos;

internal abstract class RDMCombo : CustomComboJob<RDMGauge>
{
    //看看现在有没有促进
    internal static bool IsBreaking => BaseAction.HaveStatusSelfFromSelf(1239);
    internal struct Actions
    {

        public static readonly BaseAction
            //震荡
            Jolt = new BaseAction(7503)
            {
                BuffsProvide = GeneralActions.Swiftcast.BuffsProvide.Union(new ushort[] { ObjectStatus.Acceleration }).ToArray(),
            },

            //回刺
            EnchantedRiposte = new BaseAction(7504),

            //赤闪雷
            Verthunder = new BaseAction(7505)
            {
                BuffsNeed = GeneralActions.Swiftcast.BuffsProvide.Union(new ushort[] { ObjectStatus.Acceleration }).ToArray(),
            },

            //短兵相接
            CorpsAcorps = new BaseAction(7506)
            {
                BuffsProvide = new ushort[]
                {
                    ObjectStatus.Bind1,
                    ObjectStatus.Bind2,
                }
            },

            //赤疾风
            Veraero = new BaseAction(7507)
            {
                BuffsNeed = GeneralActions.Swiftcast.BuffsProvide.Union(new ushort[] { ObjectStatus.Acceleration }).ToArray(),
            },

            //散碎
            Scatter = new BaseAction(7509)
            {
                BuffsNeed = GeneralActions.Swiftcast.BuffsProvide.Union(new ushort[] { ObjectStatus.Acceleration }).ToArray(),
            },

            //赤震雷
            Verthunder2 = new BaseAction(16524u)
            {
                BuffsProvide = GeneralActions.Swiftcast.BuffsProvide.Union(new ushort[] { ObjectStatus.Acceleration }).ToArray(),
            },

            //赤烈风
            Veraero2 = new BaseAction(16525u)
            {
                BuffsProvide = GeneralActions.Swiftcast.BuffsProvide.Union(new ushort[] { ObjectStatus.Acceleration }).ToArray(),
            },

            //赤火炎
            Verfire = new BaseAction(7510)
            {
                BuffsNeed = new ushort[] { ObjectStatus.VerfireReady },
                BuffsProvide = GeneralActions.Swiftcast.BuffsProvide.Union(new ushort[] { ObjectStatus.Acceleration }).ToArray(),
            },

            //赤飞石
            Verstone = new BaseAction(7511)
            {
                BuffsNeed = new ushort[] { ObjectStatus.VerstoneReady },
                BuffsProvide = GeneralActions.Swiftcast.BuffsProvide.Union(new ushort[] { ObjectStatus.Acceleration }).ToArray(),
            },

            //交击斩
            Zwerchhau = new BaseAction(7512),

            //交剑
            Engagement = new BaseAction(16527),

            //飞剑
            Fleche = new BaseAction(7517),

            //连攻
            Redoublement = new BaseAction(7516),

            //促进
            Acceleration = new BaseAction(7518)
            {
                BuffsProvide = new ushort[] {ObjectStatus.Acceleration},
            },

            //划圆斩
            Moulinet = new BaseAction(7513),

            //赤治疗
            Vercure = new BaseAction(7514, true)
            {
                BuffsProvide = GeneralActions.Swiftcast.BuffsProvide.Union(Acceleration.BuffsProvide).ToArray(),
            },

            //六分反击
            ContreSixte = new BaseAction(7519u),

            //鼓励
            Embolden = new BaseAction(7520),

            //倍增
            Manafication = new BaseAction(7521)
            {
                OtherCheck = () => JobGauge.WhiteMana <= 50 && JobGauge.BlackMana <= 50,
            },

            //赤复活
            Verraise = new BaseAction(7523, true)
            {
                BuffsNeed = GeneralActions.Swiftcast.BuffsProvide,
                OtherCheck = () => TargetHelper.DeathPeopleAll.Length > 0,
                BuffsProvide = new ushort[] { ObjectStatus.Raise },
            },

            //续斩
            Reprise = new BaseAction(16529),

            //抗死
            MagickBarrier = new BaseAction(25857);
    }

    private protected override bool FirstActionAbility(byte level, byte abilityRemain, BaseAction nextGCD, out BaseAction act)
    {

        //鼓励要放到魔回刺或者魔Z斩或魔划圆斩之后
        if (nextGCD.ActionID == Actions.Zwerchhau.ActionID || nextGCD.ActionID == Actions.Redoublement.ActionID || nextGCD.ActionID == Actions.Moulinet.ActionID)
        {
            if (Actions.Embolden.TryUseAction(level, out act, mustUse: true)) return true;
        }
        //倍增要放到魔连攻击之后
        if (JobGauge.ManaStacks == 3 || level < 68)
        {
            if (Actions.Manafication.TryUseAction(level, out act)) return true;
        }
        //开场爆发的时候释放。
        if (JobGauge.WhiteMana == 6 & JobGauge.BlackMana == 12)
        {
            if (Actions.Embolden.TryUseAction(level, out act, mustUse: true)) return true;
            if (Actions.Manafication.TryUseAction(level, out act)) return true;
        }

        act = null;
        return false;
    }

    private protected override bool ForAttachAbility(byte level, byte abilityRemain, out BaseAction act)
    {
        //加个醒梦
        if (GeneralActions.LucidDreaming.TryUseAction(level, out act)) return true;

        //促进满了就用。 
        if (Actions.Acceleration.TryUseAction(level, out act, mustUse: true)) return true;
        if (GeneralActions.Swiftcast.TryUseAction(level, out act, mustUse: true)) return true;

        //攻击四个能力技。
        if (Actions.ContreSixte.TryUseAction(level, out act, mustUse: true)) return true;
        if (Actions.Fleche.TryUseAction(level, out act)) return true;
        if (Actions.Engagement.TryUseAction(level, out act, Empty: IsBreaking)) return true;
        //if (Actions.CorpsAcorps.TryUseAction(level, out act)) return true;

        return false;
    }

    private protected override bool EmergercyGCD(byte level, uint lastComboActionID, out BaseAction act)
    {
        //努力救人！
        if (Actions.Verraise.TryUseAction(level, out act)) return true;
        return false;
    }

    private protected override bool AttackGCD(byte level, uint lastComboActionID, out BaseAction act)
    {
        //如果已经在爆发了，那继续！
        if (CanBreak(lastComboActionID, level, out act)) return true;

        if (lastComboActionID == 0)
        {
            if (Actions.Verthunder2.TryUseAction(level, out act)) return true;
            if (Actions.Verthunder.TryUseAction(level, out act)) return true;
        }

        #region 常规输出
        if (Actions.Verfire.TryUseAction(level, out act)) return true;
        if (Actions.Verstone.TryUseAction(level, out act)) return true;

        //试试看散碎
        if (Actions.Scatter.TryUseAction(level, out act)) return true;
        //平衡魔元
        if (JobGauge.WhiteMana < JobGauge.BlackMana)
        {
            if (Actions.Veraero2.TryUseAction(level, out act)) return true;
            if (Actions.Veraero.TryUseAction(level, out act)) return true;
        }
        else
        {
            if (Actions.Verthunder2.TryUseAction(level, out act)) return true;
            if (Actions.Verthunder.TryUseAction(level, out act)) return true;
        }
        if (Actions.Jolt.TryUseAction(level, out act)) return true;
        #endregion
        //赤治疗，加即刻。
        if (Actions.Vercure.TryUseAction(level, out act)) return true;

        return false;
    }

    internal static bool CanBreak(uint lastComboActionID, byte level, out BaseAction act)
    {
        #region 远程三连
        //如果魔元结晶满了。
        if (JobGauge.ManaStacks == 3)
        {
            if (JobGauge.BlackMana > JobGauge.WhiteMana && level >= 70)
            {
                if (Actions.Veraero2.TryUseAction(level, out act, mustUse: true)) return true;
            }
            if (Actions.Verthunder2.TryUseAction(level, out act, mustUse: true)) return true;
        }

        //如果上一次打了赤神圣或者赤核爆了
        if (level >= 80 && (lastComboActionID == 7525 || lastComboActionID == 7526))
        {
            act = Actions.Jolt;
            return true;
        }

        //如果上一次打了焦热
        if (level >= 90 && lastComboActionID == 16530)
        {
            act = Actions.Jolt;
            return true;
        }
        #endregion

        #region 近战三连

        if (lastComboActionID == Actions.Moulinet.ActionID && JobGauge.BlackMana >= 20 && JobGauge.WhiteMana >= 20)
        {
            if (Actions.Moulinet.TryUseAction(level, out act)) return true;
            if (Actions.EnchantedRiposte.TryUseAction(level, out act)) return true;
        }
        if (Actions.Zwerchhau.TryUseAction(level, out act, lastComboActionID)) return true;
        if (Actions.Redoublement.TryUseAction(level, out act, lastComboActionID)) return true;

        //如果倍增好了，或者魔元满了，或者正在爆发，或者处于开场爆发状态，就马上用！
        bool mustStart = IsBreaking || JobGauge.BlackMana == 100 || JobGauge.WhiteMana == 100 || !Actions.Embolden.CoolDown.IsCooldown;

        //在魔法元没有溢出的情况下，要求较小的魔元不带触发，也可以强制要求跳过判断。
        if (!mustStart)
        {
            if (JobGauge.BlackMana == JobGauge.WhiteMana) return false;

            //要求较小的魔元不带触发，也可以强制要求跳过判断。
            if (JobGauge.WhiteMana < JobGauge.BlackMana)
            {
                if (BaseAction.HaveStatusSelfFromSelf(ObjectStatus.VerstoneReady))
                {
                    return false;
                }
            }
            if (JobGauge.WhiteMana > JobGauge.BlackMana)
            {
                if (BaseAction.HaveStatusSelfFromSelf(ObjectStatus.VerfireReady))
                {
                    return false;
                }
            }

            //看看有没有即刻相关的技能。
            foreach (var buff in Actions.Vercure.BuffsProvide)
            {
                if (BaseAction.HaveStatusSelfFromSelf(buff))
                {
                    return false;
                }
            }

            //如果倍增的时间快到了，但还是没好。
            float emboldenRemain = Actions.Embolden.CoolDown.CooldownRemaining;
            if (emboldenRemain < 30 && emboldenRemain > 1)
            {
                return false;
            }
        }

        #endregion

        #region 开启爆发

        //要来可以使用近战三连了。
        if (Service.Configuration.IsTargetBoss && JobGauge.BlackMana >= 50 && JobGauge.WhiteMana >= 50)
        {
            if (Actions.EnchantedRiposte.TryUseAction(level, out act)) return true;

        }
        if (JobGauge.BlackMana >= 60 && JobGauge.WhiteMana >= 60)
        {
            if (Actions.Moulinet.TryUseAction(level, out act)) return true;
            if (Actions.EnchantedRiposte.TryUseAction(level, out act)) return true;
        }
        #endregion
        return false;
    }

}
