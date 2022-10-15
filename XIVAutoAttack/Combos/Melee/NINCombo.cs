using Dalamud.Game.ClientState.JobGauge.Types;
using System.Collections.Generic;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Configuration;

namespace XIVAutoAttack.Combos.Melee;

internal class NINCombo : JobGaugeCombo<NINGauge>
{
    internal override uint JobID => 30;
    public class NinAction : BaseAction
    {
        internal BaseAction[] Ninjutsus { get; }
        public NinAction(uint actionID, params BaseAction[] ninjutsus)
            : base(actionID, false, false)
        {
            Ninjutsus = ninjutsus;
        }
    }

    private static bool _break = false;
    internal static NinAction _ninactionAim = null;

    internal struct Actions
    {
        public static readonly BaseAction

            //隐遁
            Hide = new (2245),

            //双刃旋
            SpinningEdge = new (2240),

            //残影
            ShadeShift = new (2241),

            //绝风
            GustSlash = new (2242),

            //飞刀
            ThrowingDagger = new (2247),

            //夺取
            Mug = new (2248)
            {
                OtherCheck = b => JobGauge.Ninki <= 50,
            },

            //攻其不备
            TrickAttack = new (2258)
            {
                BuffsNeed = new ushort[] { ObjectStatus.Suiton, ObjectStatus.Hidden },
            },

            //旋风刃
            AeolianEdge = new (2255),

            //血雨飞花
            DeathBlossom = new (2254),

            //天之印
            Ten = new (2259),

            //地之印
            Chi = new (2261),

            //人之印
            Jin = new (2263),

            //天地人
            TenChiJin = new (7403)
            {
                BuffsProvide = new [] { ObjectStatus.Kassatsu, ObjectStatus.TenChiJin },
                OtherCheck = b => JobGauge.HutonTimer > 0,
            },

            //缩地
            Shukuchi = new (2262, true),

            //断绝
            Assassinate = new (2246),

            //命水
            Meisui = new (16489)
            {
                BuffsNeed = new [] { ObjectStatus.Suiton },
                OtherCheck = b => JobGauge.Ninki <= 50
            },

            //生杀予夺
            Kassatsu = new (2264, isFriendly: true)
            {
                //OtherCheck = b => Ten.IsCoolDown,
                BuffsProvide = new [] { ObjectStatus.Kassatsu, ObjectStatus.TenChiJin },
            },

            //八卦无刃杀
            HakkeMujinsatsu = new (16488),

            //强甲破点突
            ArmorCrush = new (3563)
            {
                OtherCheck = b => JobGauge.HutonTimer < 30000 && JobGauge.HutonTimer > 0,
            },

            //通灵之术·大虾蟆
            HellfrogMedium = new (7401),

            //六道轮回
            Bhavacakra = new (7402),

            //分身之术
            Bunshin = new (16493),

            //残影镰鼬
            PhantomKamaitachi = new (25774)
            {
                BuffsNeed = new [] { ObjectStatus.PhantomKamaitachiReady },
            },

            //月影雷兽牙
            FleetingRaiju = new (25778)
            {
                BuffsNeed = new [] { ObjectStatus.RaijuReady },
            },

            //月影雷兽爪
            ForkedRaiju = new (25777)
            {
                BuffsNeed = new [] { ObjectStatus.RaijuReady },
            },

            //风来刃
            Huraijin = new (25876)
            {
                OtherCheck = b => JobGauge.HutonTimer == 0,
            },

            //梦幻三段
            DreamWithinaDream = new (3566),

            //风魔手里剑天
            FumaShurikenTen = new (18873),

            //风魔手里剑人
            FumaShurikenJin = new (18875),

            //火遁之术天
            KatonTen = new (18876),

            //雷遁之术地
            RaitonChi = new (18877),

            //土遁之术地
            DotonChi = new (18880),

            //水遁之术人
            SuitonJin = new (18881);

        public static readonly NinAction

            //通灵之术
            RabbitMedium = new (2272),


            //风魔手里剑
            FumaShuriken = new (2265, Ten)
            {
                AfterUse = ClearNinjutsus,
            },

            //火遁之术
            Katon = new (2266, Chi, Ten)
            {
                AfterUse = ClearNinjutsus,
            },

            //雷遁之术
            Raiton = new (2267, Ten, Chi)
            {
                AfterUse = ClearNinjutsus,
            },


            //冰遁之术
            Hyoton = new (2268, Ten, Jin),

            //风遁之术
            Huton = new (2269, Jin, Chi, Ten)
            {
                OtherCheck = b => JobGauge.HutonTimer == 0,
                AfterUse = ClearNinjutsus,
            },

            //土遁之术
            Doton = new (2270, Jin, Ten, Chi)
            {
                BuffsProvide = new [] { ObjectStatus.Doton },
                AfterUse = ClearNinjutsus,
            },

            //水遁之术
            Suiton = new (2271, Ten, Chi, Jin)
            {
                BuffsProvide = new [] { ObjectStatus.Suiton },
                AfterUse = () =>
                {
                    ClearNinjutsus();
                    _break = false;
                },
            },


            //劫火灭却之术
            GokaMekkyaku = new (16491, Chi, Ten)
            {
                AfterUse = ClearNinjutsus,
            },


            //冰晶乱流之术
            HyoshoRanryu = new (16492, Ten, Jin)
            {
                AfterUse = ClearNinjutsus,
            };
    }

    private protected override ActionConfiguration CreateConfiguration()
    {
        return base.CreateConfiguration();
    }

    internal override SortedList<DescType, string> Description => new ()
    {
        {DescType.单体防御, $"{Actions.ShadeShift.Action.Name}"},
        {DescType.移动, $"{Actions.Shukuchi.Action.Name}，目标为面向夹角小于30°内最远目标。"},
    };
    private bool ChoiceNinjutsus(out IAction act)
    {
        act = null;
        if (Service.IconReplacer.OriginalHook(2260) != 2260) return false;
        if (TargetHelper.Weaponelapsed < 0.2 && TargetHelper.Weaponelapsed > 0) return false;
        //有生杀予夺
        if (StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.Kassatsu))
        {
            if (Actions.GokaMekkyaku.ShouldUseAction(out _))
            {
                _ninactionAim = Actions.GokaMekkyaku;
                return false;
            }
            if (Actions.HyoshoRanryu.ShouldUseAction(out _))
            {
                _ninactionAim = Actions.HyoshoRanryu;
                return false;
            }

            if (Actions.Katon.ShouldUseAction(out _))
            {
                _ninactionAim = Actions.Katon;
                return false;
            }

            if (Actions.Raiton.ShouldUseAction(out _))
            {
                _ninactionAim = Actions.Raiton;
                return false;
            }
        }
        else
        {
            bool empty = Actions.Ten.ShouldUseAction(out _, mustUse: true);
            bool haveDoton = StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.Doton);

            //加状态
            if (Actions.Huraijin.ShouldUseAction(out act)) return true;

            if (JobGauge.HutonTimer > 0 && _ninactionAim?.ID == Actions.Huton.ID)
            {
                _ninactionAim = null;
                return false;
            }

            if (empty && (!TargetHelper.InBattle || Service.ClientState.LocalPlayer.Level < Actions.Huraijin.Level) && Actions.Huton.ShouldUseAction(out _))
            {
                _ninactionAim = Actions.Huton;
                return false;
            }

            if (GeneralNinjutsus(empty, haveDoton)) return false;
        }
        return false;
    }

    private bool GeneralNinjutsus(bool empty, bool haveDoton)
    {
        //清空忍术
        if (empty)
        {
            if (Actions.Katon.ShouldUseAction(out _))
            {
                if(!haveDoton && !IsMoving && Actions.TenChiJin.RecastTimeRemain < 2) _ninactionAim = Actions.Doton;
                else _ninactionAim = Actions.Katon;
                return true;
            }
            //背刺
            if (Actions.Suiton.ShouldUseAction(out _) && Actions.TrickAttack.RecastTimeRemain < 2 && _break)
            {
                _ninactionAim = Actions.Suiton;
                return true;
            }
        }
        //常规单体忍术
        if (Actions.Ten.ShouldUseAction(out _) && (Service.ClientState.LocalPlayer.Level < Actions.TenChiJin.Level || Actions.TenChiJin.IsCoolDown))
        {
            if (Actions.Raiton.ShouldUseAction(out _))
            {
                _ninactionAim = Actions.Raiton;
                return true;
            }

            if (Service.ClientState.LocalPlayer.Level < Actions.Chi.Level && Actions.FumaShuriken.ShouldUseAction(out _))
            {
                _ninactionAim = Actions.FumaShuriken;
                return true;
            }
        }
        return false;
    }

    private static void ClearNinjutsus()
    {
        _ninactionAim = null;
    }

    private bool DoNinjutsus(out IAction act)
    {
        act = null;

        //有天地人
        if (StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.TenChiJin))
        {
            uint tenId = Service.IconReplacer.OriginalHook(Actions.Ten.ID);
            uint chiId = Service.IconReplacer.OriginalHook(Actions.Chi.ID);
            uint jinId = Service.IconReplacer.OriginalHook(Actions.Jin.ID);

            //第一个
            if (tenId == Actions.FumaShurikenTen.ID)
            {
                //AOE
                if (Actions.Katon.ShouldUseAction(out _))
                {
                    if (Actions.FumaShurikenJin.ShouldUseAction(out act)) return true;
                }
                //Single
                if (Actions.FumaShurikenTen.ShouldUseAction(out act)) return true;
            }

            //第二击杀AOE
            else if (tenId == Actions.KatonTen.ID)
            {
                if (Actions.KatonTen.ShouldUseAction(out act, mustUse: true)) return true;
            }
            //其他几击
            else if (chiId == Actions.RaitonChi.ID)
            {
                if (Actions.RaitonChi.ShouldUseAction(out act, mustUse: true)) return true;
            }
            else if (chiId == Actions.DotonChi.ID)
            {
                if (Actions.DotonChi.ShouldUseAction(out act, mustUse: true)) return true;
            }
            else if (jinId == Actions.SuitonJin.ID)
            {
                if (Actions.SuitonJin.ShouldUseAction(out act, mustUse: true)) return true;
            }
        }

        if (_ninactionAim == null) return false;

        uint id = Service.IconReplacer.OriginalHook(2260);

        //没开始，释放第一个
        if (id == 2260)
        {
            //重置
            if(!StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.Kassatsu) 
                && !StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.TenChiJin)
                && !Actions.Ten.ShouldUseAction(out _, mustUse: true))
            {
                //_ninactionAim = null;
                return false;
            }
            act = _ninactionAim.Ninjutsus[0];
            return true;
        }
        //失败了
        else if (id == Actions.RabbitMedium.ID)
        {
            act = null;
            return false;
        }
        //结束了
        else if (id == _ninactionAim.ID)
        {
            if (_ninactionAim.ShouldUseAction(out act, mustUse: true)) return true;
            if (_ninactionAim.ID == Actions.Doton.ID && !TargetHelper.InBattle)
            {
                act = _ninactionAim;
                return true;
            }
        }
        //释放第二个
        else if (id == Actions.FumaShuriken.ID)
        {
            if (_ninactionAim.Ninjutsus.Length > 1)
            {
                act = _ninactionAim.Ninjutsus[1];
                return true;
            }
        }
        //释放第三个
        else if (id == Actions.Katon.ID || id == Actions.Raiton.ID || id == Actions.Hyoton.ID)
        {
            if (_ninactionAim.Ninjutsus.Length > 2)
            {
                act = _ninactionAim.Ninjutsus[2];
                return true;
            }
        }
        _ninactionAim = null;
        return false;
    }

    private protected override bool BreakAbility(byte abilityRemain, out IAction act)
    {
        _break = true;
        act = null;
        return false;
    }

    private protected override bool GeneralGCD(uint lastComboActionID, out IAction act)
    {
        if(Service.ClientState.LocalPlayer.Level >= Actions.Ten.Level)
        {
            if (ChoiceNinjutsus(out act)) return true;
            if (DoNinjutsus(out act)) return true;
        }

        //用真北取消隐匿
        if (StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.Hidden) && GeneralActions.TrueNorth.ShouldUseAction(out act, emptyOrSkipCombo: true)) return true;

        //用隐匿恢复忍术数量
        if (!TargetHelper.InBattle && _ninactionAim == null && Actions.Ten.IsCoolDown && Actions.Hide.ShouldUseAction(out act)) return true;

        var replace = Service.IconReplacer.OriginalHook(2260);
        //无忍术或者忍术中途停了
        if (_ninactionAim == null || (replace != 2260 && replace != _ninactionAim.ID))
        {
            _ninactionAim = null;

            //大招
            if (Actions.FleetingRaiju.ShouldUseAction(out act, lastComboActionID)) return true;
            if (Actions.ForkedRaiju.ShouldUseAction(out act, lastComboActionID))
            {
                if (TargetFilter.DistanceToPlayer(Actions.ForkedRaiju.Target) < 2)
                {
                    return true;
                }
            }

            if (Actions.PhantomKamaitachi.ShouldUseAction(out act, lastComboActionID)) return true;

            if (Actions.Huraijin.ShouldUseAction(out act)) return true;

            //AOE
            if (Actions.HakkeMujinsatsu.ShouldUseAction(out act, lastComboActionID)) return true;
            if (Actions.DeathBlossom.ShouldUseAction(out act, lastComboActionID)) return true;

            //Single
            if (Actions.ArmorCrush.ShouldUseAction(out act, lastComboActionID)) return true;
            if (Actions.AeolianEdge.ShouldUseAction(out act, lastComboActionID)) return true;
            if (Actions.GustSlash.ShouldUseAction(out act, lastComboActionID)) return true;
            if (Actions.SpinningEdge.ShouldUseAction(out act, lastComboActionID)) return true;

            //飞刀
            if (IconReplacer.Move && MoveAbility(1, out act)) return true;
            if (Actions.ThrowingDagger.ShouldUseAction(out act)) return true;
        }

        act = null;
        return false;
    }

    private protected override bool MoveGCD(uint lastComboActionID, out IAction act)
    {
        if (Actions.ForkedRaiju.ShouldUseAction(out act, lastComboActionID)) return true;
        return base.MoveGCD(lastComboActionID, out act);
    }

    private protected override bool DefenceSingleAbility(byte abilityRemain, out IAction act)
    {
        if (Actions.ShadeShift.ShouldUseAction(out act)) return true;

        return false;
    }

    private protected override bool ForAttachAbility(byte abilityRemain, out IAction act)
    {
        act = null;
        if (!TargetHelper.InBattle || Service.IconReplacer.OriginalHook(2260) != 2260) return false;

        if (Actions.Mug.ShouldUseAction(out act)) return true;

        //解决Buff
        if (Actions.TrickAttack.ShouldUseAction(out act)) return true;
        if (Actions.Meisui.ShouldUseAction(out act)) return true;

        //if ((Actions.TrickAttack.RecastTimeElapsed <= 20 && Actions.TrickAttack.IsCoolDown)
        //    || Actions.Katon.ShouldUseAction(out _))
        {
            if (!XIVAutoAttackPlugin.movingController.IsMoving && Actions.TenChiJin.ShouldUseAction(out act)) return true;
            if (Actions.Kassatsu.ShouldUseAction(out act)) return true;
            if (UseBreakItem(out act)) return true;
        }

        if (JobGauge.Ninki >= 50)
        {
            if (Actions.Bunshin.ShouldUseAction(out act)) return true;
            if (Actions.HellfrogMedium.ShouldUseAction(out act)) return true;
            if (Actions.Bhavacakra.ShouldUseAction(out act)) return true;
        }

        if (Service.ClientState.LocalPlayer.Level < Actions.DreamWithinaDream.Level)
        {
            if (Actions.Assassinate.ShouldUseAction(out act)) return true;
        }
        else
        {
            if (Actions.DreamWithinaDream.ShouldUseAction(out act)) return true;
        }
        return false;
    }

    private protected override bool MoveAbility(byte abilityRemain, out IAction act)
    {
        if (Actions.Shukuchi.ShouldUseAction(out act)) return true;

        return false;
    }
    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        //牵制
        if (GeneralActions.Feint.ShouldUseAction(out act)) return true;
        return false;
    }
}
