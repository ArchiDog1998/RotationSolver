using Dalamud.Game.ClientState.JobGauge.Types;
using System.Collections.Generic;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Configuration;
using XIVAutoAttack.Helpers;

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

    //private static bool _break = false;
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
            Mug = new(2248)
            {
                OtherCheck = b => JobGauge.Ninki <= 50,
            },

            //攻其不备
            TrickAttack = new(2258)
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
                OtherCheck = b => JobGauge.Ninki <= 50,
            },

            //生杀予夺
            Kassatsu = new (2264, isFriendly: true)
            {
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
            FumaShuriken = new (2265, Ten),

            //火遁之术
            Katon = new (2266, Chi, Ten),

            //雷遁之术
            Raiton = new (2267, Ten, Chi),


            //冰遁之术
            Hyoton = new (2268, Ten, Jin),

            //风遁之术
            Huton = new (2269, Jin, Chi, Ten)
            {
                OtherCheck = b => JobGauge.HutonTimer == 0,
            },

            //土遁之术
            Doton = new (2270, Jin, Ten, Chi)
            {
                BuffsProvide = new [] { ObjectStatus.Doton },
            },

            //水遁之术
            Suiton = new (2271, Ten, Chi, Jin)
            {
                BuffsProvide = new [] { ObjectStatus.Suiton },
                OtherCheck = b => TrickAttack.WillHaveOneCharge(2,2),
            },


            //劫火灭却之术
            GokaMekkyaku = new (16491, Chi, Ten),


            //冰晶乱流之术
            HyoshoRanryu = new (16492, Ten, Jin);
    }

    private protected override ActionConfiguration CreateConfiguration()
    {
        return base.CreateConfiguration().SetBool("UseHide", true, "脱战隐身恢复忍术");
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
        if (Weaponelapsed is < 0.2f and > 0) return false;

        if (!LocalPlayer.HaveStatus(ObjectStatus.Ninjutsu))
        {
            ClearNinjutsus();
        }
        //有生杀予夺
        if (LocalPlayer.HaveStatus(ObjectStatus.Kassatsu))
        {
            if (Actions.GokaMekkyaku.ShouldUse(out _))
            {
                _ninactionAim = Actions.GokaMekkyaku;
                return false;
            }
            if (Actions.HyoshoRanryu.ShouldUse(out _))
            {
                _ninactionAim = Actions.HyoshoRanryu;
                return false;
            }

            if (Actions.Katon.ShouldUse(out _))
            {
                _ninactionAim = Actions.Katon;
                return false;
            }

            if (Actions.Raiton.ShouldUse(out _))
            {
                _ninactionAim = Actions.Raiton;
                return false;
            }
        }
        else
        {
            bool empty = Actions.Ten.ShouldUse(out _, mustUse: true);
            bool haveDoton = LocalPlayer.HaveStatus(ObjectStatus.Doton);

            //加状态
            if (Actions.Huraijin.ShouldUse(out act)) return true;

            if (JobGauge.HutonTimer > 0 && _ninactionAim?.ID == Actions.Huton.ID)
            {
                _ninactionAim = null;
                return false;
            }

            if (empty && (!InBattle || Level < Actions.Huraijin.Level) && Actions.Huton.ShouldUse(out _))
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
            if (Actions.Katon.ShouldUse(out _))
            {
                if(!haveDoton && !IsMoving && Actions.TenChiJin.WillHaveOneCharge(0, 1)) _ninactionAim = Actions.Doton;
                else _ninactionAim = Actions.Katon;
                return true;
            }
        }
        //常规单体忍术
        if (Actions.Ten.ShouldUse(out _) && (Level < Actions.TenChiJin.Level || Actions.TenChiJin.IsCoolDown))
        {
            if (Actions.Raiton.ShouldUse(out _))
            {
                _ninactionAim = Actions.Raiton;
                return true;
            }

            if (Level < Actions.Chi.Level && Actions.FumaShuriken.ShouldUse(out _))
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
        if (LocalPlayer.HaveStatus(ObjectStatus.TenChiJin))
        {
            uint tenId = Service.IconReplacer.OriginalHook(Actions.Ten.ID);
            uint chiId = Service.IconReplacer.OriginalHook(Actions.Chi.ID);
            uint jinId = Service.IconReplacer.OriginalHook(Actions.Jin.ID);

            //第一个
            if (tenId == Actions.FumaShurikenTen.ID)
            {
                //AOE
                if (Actions.Katon.ShouldUse(out _))
                {
                    if (Actions.FumaShurikenJin.ShouldUse(out act)) return true;
                }
                //Single
                if (Actions.FumaShurikenTen.ShouldUse(out act)) return true;
            }

            //第二击杀AOE
            else if (tenId == Actions.KatonTen.ID)
            {
                if (Actions.KatonTen.ShouldUse(out act, mustUse: true)) return true;
            }
            //其他几击
            else if (chiId == Actions.RaitonChi.ID)
            {
                if (Actions.RaitonChi.ShouldUse(out act, mustUse: true)) return true;
            }
            else if (chiId == Actions.DotonChi.ID)
            {
                if (Actions.DotonChi.ShouldUse(out act, mustUse: true)) return true;
            }
            else if (jinId == Actions.SuitonJin.ID)
            {
                if (Actions.SuitonJin.ShouldUse(out act, mustUse: true)) return true;
            }
        }

        if (_ninactionAim == null) return false;

        uint id = Service.IconReplacer.OriginalHook(2260);

        //没开始，释放第一个
        if (id == 2260)
        {
            //重置
            if(!LocalPlayer.HaveStatus(ObjectStatus.Kassatsu, ObjectStatus.TenChiJin) 
                && !Actions.Ten.ShouldUse(out _, mustUse: true))
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
            if (_ninactionAim.ShouldUse(out act, mustUse: true)) return true;
            if (_ninactionAim.ID == Actions.Doton.ID && !InBattle)
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
        //夺取
        if (Actions.Mug.ShouldUse(out act)) return true;

        //背刺
        if (Actions.Ten.ShouldUse(out _, mustUse: true) && Actions.Suiton.ShouldUse(out _))
        {
            _ninactionAim = Actions.Suiton;
            return true;
        }
        act = null;
        return false;
    }

    private protected override bool GeneralGCD(uint lastComboActionID, out IAction act)
    {
        if(Level >= Actions.Ten.Level)
        {
            if (ChoiceNinjutsus(out act)) return true;
            if (DoNinjutsus(out act)) return true;
        }

        //用真北取消隐匿
        if (LocalPlayer.HaveStatus(ObjectStatus.Hidden)
            && GeneralActions.TrueNorth.ShouldUse(out act, emptyOrSkipCombo: true)) return true;

        //用隐匿恢复忍术数量
        if (!InBattle && _ninactionAim == null && Config.GetBoolByName("UseHide")
            && Actions.Ten.IsCoolDown && Actions.Hide.ShouldUse(out act)) return true;

        var replace = Service.IconReplacer.OriginalHook(2260);
        //无忍术或者忍术中途停了
        if (_ninactionAim == null || (replace != 2260 && replace != _ninactionAim.ID))
        {
            _ninactionAim = null;

            //大招
            if (Actions.FleetingRaiju.ShouldUse(out act, lastComboActionID)) return true;
            if (Actions.ForkedRaiju.ShouldUse(out act, lastComboActionID))
            {
                if (TargetFilter.DistanceToPlayer(Actions.ForkedRaiju.Target) < 2)
                {
                    return true;
                }
            }

            if (Actions.PhantomKamaitachi.ShouldUse(out act, lastComboActionID)) return true;

            if (Actions.Huraijin.ShouldUse(out act)) return true;

            //AOE
            if (Actions.HakkeMujinsatsu.ShouldUse(out act, lastComboActionID)) return true;
            if (Actions.DeathBlossom.ShouldUse(out act, lastComboActionID)) return true;

            //Single
            if (Actions.ArmorCrush.ShouldUse(out act, lastComboActionID)) return true;
            if (Actions.AeolianEdge.ShouldUse(out act, lastComboActionID)) return true;
            if (Actions.GustSlash.ShouldUse(out act, lastComboActionID)) return true;
            if (Actions.SpinningEdge.ShouldUse(out act, lastComboActionID)) return true;

            //飞刀
            if (IconReplacer.Move && MoveAbility(1, out act)) return true;
            if (Actions.ThrowingDagger.ShouldUse(out act)) return true;
        }

        act = null;
        return false;
    }

    private protected override bool MoveGCD(uint lastComboActionID, out IAction act)
    {
        if (Actions.ForkedRaiju.ShouldUse(out act, lastComboActionID)) return true;
        return base.MoveGCD(lastComboActionID, out act);
    }

    private protected override bool DefenceSingleAbility(byte abilityRemain, out IAction act)
    {
        if (Actions.ShadeShift.ShouldUse(out act)) return true;

        return false;
    }

    private protected override bool ForAttachAbility(byte abilityRemain, out IAction act)
    {
        act = null;
        if (!InBattle || Service.IconReplacer.OriginalHook(2260) != 2260) return false;

        //解决Buff
        if (Actions.TrickAttack.ShouldUse(out act)) return true;
        if (Actions.Meisui.ShouldUse(out act)) return true;

        if (!IsMoving && Actions.TenChiJin.ShouldUse(out act)) return true;
        if (Actions.Kassatsu.ShouldUse(out act)) return true;
        if (UseBreakItem(out act)) return true;

        if (JobGauge.Ninki >= 50)
        {
            if (Actions.Bunshin.ShouldUse(out act)) return true;
            if (Actions.HellfrogMedium.ShouldUse(out act)) return true;
            if (Actions.Bhavacakra.ShouldUse(out act)) return true;
        }

        if (Service.ClientState.LocalPlayer.Level < Actions.DreamWithinaDream.Level)
        {
            if (Actions.Assassinate.ShouldUse(out act)) return true;
        }
        else
        {
            if (Actions.DreamWithinaDream.ShouldUse(out act)) return true;
        }
        return false;
    }

    private protected override bool MoveAbility(byte abilityRemain, out IAction act)
    {
        if (Actions.Shukuchi.ShouldUse(out act)) return true;

        return false;
    }
    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        //牵制
        if (GeneralActions.Feint.ShouldUse(out act)) return true;
        return false;
    }
}
