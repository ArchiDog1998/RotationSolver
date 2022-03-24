using Dalamud.Game.ClientState.JobGauge.Types;
using System.Linq;
namespace XIVComboPlus.Combos;

internal abstract class RDMCombo : CustomComboJob<RDMGauge>
{
    internal struct Actions
    {
        public static readonly BaseAction
            //震荡
            Jolt = new BaseAction(7503)
            {
                BuffsProvide = GeneralActions.Swiftcast.BuffsProvide.Union(new ushort[] { ObjectStatus.Acceleration }).ToArray(),
            },

            //回刺
            EnchantedRiposte = new BaseAction(8887),

            //赤闪雷
            Verthunder = new BaseAction(7505)
            {
                BuffsNeed = GeneralActions.Swiftcast.BuffsProvide.Union(new ushort[] {ObjectStatus.Acceleration}).ToArray(),
            },

            //短兵相接
            CorpsAcorps = new BaseAction(7506)
            {
                BuffsCantHave = new ushort[]
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
            EnchantedZwerchhau = new BaseAction(8888),

            //交剑
            Engagement = new BaseAction(16527),

            //飞剑
            Fleche = new BaseAction(7517),

            //连攻
            EnchantedRedoublement = new BaseAction(8889),

            //促进
            Acceleration = new BaseAction(7518),

            //划圆斩
            EnchantedMoulinet = new BaseAction(18944),

            //赤治疗
            Vercure = new BaseAction(10025)
            {
                BuffsProvide = GeneralActions.Swiftcast.BuffsProvide.Union(new ushort[] { ObjectStatus.Acceleration }).ToArray(),
            },

            //六分反击
            ContreSixte = new BaseAction(7519u),

            //鼓励
            Embolden = new BaseAction(7520),

            //倍增
            Manafication = new BaseAction(7521),

            //赤复活
            Verraise = new BaseAction(7523)
            {
                BuffsNeed = GeneralActions.Swiftcast.BuffsProvide,
                OtherCheck = () => TargetHelper.DeathPeople.Length > 0,
            },

            //续斩
            Reprise = new BaseAction(16529),

            //抗死
            MagickBarrier = new BaseAction(25857);
    }

    //public static class Buffs
    //{

    //    public const ushort LostChainspell = 2560;
    //}
    protected bool CanAddAbility(byte level, uint lastComboActionID, out uint act)
    {
        act = 0;

        if (CanInsertAbility)
        {
            //鼓励要放到魔回刺之后
            if (lastComboActionID == Actions.EnchantedRiposte.ActionID)
            {
                if (Actions.Embolden.TryUseAction(level, out act)) return true;
            }
            //倍增要放到魔连攻击之后
            else if (lastComboActionID == Actions.EnchantedRedoublement.ActionID)
            {
                if (Actions.Manafication.TryUseAction(level, out act)) return true;
            }

            //促进满了就用。 
            if (Actions.Acceleration.TryUseAction(level, out act)) return true;

            //攻击四个能力技。
            if (Actions.ContreSixte.TryUseAction(level, out act)) return true;
            if (Actions.Fleche.TryUseAction(level, out act)) return true;
            if (Actions.Engagement.TryUseAction(level, out act)) return true;
            if (Actions.CorpsAcorps.TryUseAction(level, out act)) return true;

            //团队减伤 
            if(Actions.MagickBarrier.TryUseAction(level, out act)) return true;

            //加个混乱
            if (GeneralActions.Addle.TryUseAction(level, out act)) return true;

            //加个醒梦
            if (GeneralActions.LucidDreaming.TryUseAction(level, out act)) return true;

        }
        return false;
    }
    public static bool CanBreak(uint lastComboActionID, byte level, out uint act, bool mustUse)
    {
        #region 远程三连
        //如果魔元结晶满了。
        if (JobGauge.ManaStacks == 3)
        {
            if (JobGauge.BlackMana > JobGauge.WhiteMana && level >= 70)
            {
                if (Actions.Veraero.TryUseAction(level, out act, mustUse: true)) return true;

            }
            if (Actions.Verthunder.TryUseAction(level, out act, mustUse: true)) return true;
        }

        //如果上一次打了赤神圣或者赤核爆了
        if (level >= 80 && (lastComboActionID == 9433 || lastComboActionID == 9434))
        {
            if (Actions.Jolt.TryUseAction(level, out act, mustUse: true)) return true;
        }

        //如果上一次打了焦热
        if (level >= 90 && lastComboActionID == 17787)
        {
            if (Actions.Jolt.TryUseAction(level, out act, mustUse: true)) return true;
        }
        #endregion

        #region 近战三连

        if (lastComboActionID == Actions.EnchantedMoulinet.ActionID && JobGauge.BlackMana >= 20 && JobGauge.WhiteMana >= 20)
        {
            if (Actions.EnchantedMoulinet.TryUseAction(level, out act)) return true;
        }
        if (Actions.EnchantedRedoublement.TryUseAction(level, out act)) return true;
        if (Actions.EnchantedZwerchhau.TryUseAction(level, out act)) return true;

        //在魔法元没有溢出的情况下，要求较小的魔元不带触发，也可以强制要求跳过判断。
        if (JobGauge.BlackMana < 100 & JobGauge.WhiteMana < 100 && !mustUse)
        {
            if (JobGauge.BlackMana == JobGauge.WhiteMana) return false;
            if (JobGauge.WhiteMana < JobGauge.BlackMana)
            {
                if (BaseAction.HaveStatus(BaseAction.FindStatusSelfFromSelf(ObjectStatus.VerstoneReady)))
                {
                    return false;
                }
            }
            if (JobGauge.WhiteMana > JobGauge.BlackMana)
            {
                if (BaseAction.HaveStatus(BaseAction.FindStatusSelfFromSelf(ObjectStatus.VerfireReady)))
                {
                    return false;
                }
            }
        }

        //要来近战范围三连了。
        if (JobGauge.BlackMana >= 60 && JobGauge.WhiteMana >= 60)
        {
            if (Actions.EnchantedMoulinet.TryUseAction(level, out act)) return true;
            if (Actions.CorpsAcorps.TryUseAction(level, out act, mustUse: true)) return true;
        }


        //要来近战单体三连了。
        if (JobGauge.BlackMana >= 50 && JobGauge.WhiteMana >= 50)
        {
            if (Actions.EnchantedRiposte.TryUseAction(level, out act)) return true;
            if (Actions.CorpsAcorps.TryUseAction(level, out act, mustUse: true)) return true;
        }
        #endregion

        return false;
    }

}
