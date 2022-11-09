using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;
using System.Collections.Generic;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Configuration;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Updaters;
using static XIVAutoAttack.Combos.Melee.NINCombo;

namespace XIVAutoAttack.Combos.Melee;

internal sealed class NINCombo : JobGaugeCombo<NINGauge, CommandType>
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
    public override uint[] JobIDs => new uint[] { 30, 29 };
    public class NinAction : BaseAction
    {
        internal BaseAction[] Ninjutsus { get; }
        public NinAction(uint actionID, params BaseAction[] ninjutsus)
            : base(actionID, false, false)
        {
            Ninjutsus = ninjutsus;
        }
    }

    private static NinAction _ninactionAim = null;

    public static readonly BaseAction

        //隐遁
        Hide = new(2245),

        //双刃旋
        SpinningEdge = new(2240),

        //残影
        ShadeShift = new(2241),

        //绝风
        GustSlash = new(2242),

        //飞刀
        ThrowingDagger = new(2247),

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
        AeolianEdge = new(2255),

        //血雨飞花
        DeathBlossom = new(2254),

        //天之印
        Ten = new(2259),

        //地之印
        Chi = new(2261),

        //人之印
        Jin = new(2263),

        //天地人
        TenChiJin = new(7403)
        {
            BuffsProvide = new[] { ObjectStatus.Kassatsu, ObjectStatus.TenChiJin },
            OtherCheck = b => JobGauge.HutonTimer > 0,
        },

        //缩地
        Shukuchi = new(2262, true),

        //断绝
        Assassinate = new(2246),

        //命水
        Meisui = new(16489)
        {
            BuffsNeed = new[] { ObjectStatus.Suiton },
            OtherCheck = b => JobGauge.Ninki <= 50,
        },

        //生杀予夺
        Kassatsu = new(2264, isFriendly: true)
        {
            BuffsProvide = new[] { ObjectStatus.Kassatsu, ObjectStatus.TenChiJin },
        },

        //八卦无刃杀
        HakkeMujinsatsu = new(16488),

        //强甲破点突
        ArmorCrush = new(3563)
        {
            OtherCheck = b => RecastAfter(JobGauge.HutonTimer / 1000f, 29) && JobGauge.HutonTimer > 0,
        },

        //通灵之术·大虾蟆
        HellfrogMedium = new(7401),

        //六道轮回
        Bhavacakra = new(7402),

        //分身之术
        Bunshin = new(16493),

        //残影镰鼬
        PhantomKamaitachi = new(25774)
        {
            BuffsNeed = new[] { ObjectStatus.PhantomKamaitachiReady },
        },

        //月影雷兽牙
        FleetingRaiju = new(25778)
        {
            BuffsNeed = new[] { ObjectStatus.RaijuReady },
        },

        //月影雷兽爪
        ForkedRaiju = new(25777)
        {
            BuffsNeed = new[] { ObjectStatus.RaijuReady },
        },

        //风来刃
        Huraijin = new(25876)
        {
            OtherCheck = b => JobGauge.HutonTimer == 0,
        },

        //梦幻三段
        DreamWithinaDream = new(3566),

        //风魔手里剑天
        FumaShurikenTen = new(18873),

        //风魔手里剑人
        FumaShurikenJin = new(18875),

        //火遁之术天
        KatonTen = new(18876),

        //雷遁之术地
        RaitonChi = new(18877),

        //土遁之术地
        DotonChi = new(18880),

        //水遁之术人
        SuitonJin = new(18881);

    public static readonly NinAction

        //通灵之术
        RabbitMedium = new(2272),

        //风魔手里剑
        FumaShuriken = new(2265, Ten),

        //火遁之术
        Katon = new(2266, Chi, Ten),

        //雷遁之术
        Raiton = new(2267, Ten, Chi),

        //冰遁之术
        Hyoton = new(2268, Ten, Jin),

        //风遁之术
        Huton = new(2269, Jin, Chi, Ten)
        {
            OtherCheck = b => JobGauge.HutonTimer == 0,
        },

        //土遁之术
        Doton = new(2270, Jin, Ten, Chi)
        {
            BuffsProvide = new[] { ObjectStatus.Doton },
        },

        //水遁之术
        Suiton = new(2271, Ten, Chi, Jin)
        {
            BuffsProvide = new[] { ObjectStatus.Suiton },
            OtherCheck = b => TrickAttack.WillHaveOneChargeGCD(1, 1),
        },

        //劫火灭却之术
        GokaMekkyaku = new(16491, Chi, Ten),

        //冰晶乱流之术
        HyoshoRanryu = new(16492, Ten, Jin);

    private protected override ActionConfiguration CreateConfiguration()
    {
        return base.CreateConfiguration().SetBool("UseHide", true, "脱战隐身恢复忍术").SetBool("AutoUnhide", true, "自动取消隐身");
    }

    public override SortedList<DescType, string> Description => new()
    {
        {DescType.单体防御, $"{ShadeShift}"},
        {DescType.移动技能, $"{Shukuchi}，目标为面向夹角小于30°内最远目标。"},
    };

    private static void SetNinjustus(NinAction act)
    {
        if (_ninactionAim != null && IsLastAction(false, Ten, Jin, Chi, FumaShurikenTen, FumaShurikenJin)) return;
        _ninactionAim = act;
    }

    private bool ChoiceNinjutsus(out IAction act)
    {
        act = null;
        if (Service.IconReplacer.OriginalHook(2260) != 2260) return false;

        //在GCD快转完的时候再判断是否调整非空忍术
        if (_ninactionAim != null && ActionUpdater.WeaponRemain < 0.2) return false;

        //有生杀予夺
        if (Player.HaveStatus(ObjectStatus.Kassatsu))
        {
            if (GokaMekkyaku.ShouldUse(out _))
            {
                SetNinjustus(GokaMekkyaku);
                return false;
            }
            if (HyoshoRanryu.ShouldUse(out _))
            {
                SetNinjustus(HyoshoRanryu);
                return false;
            }

            if (Katon.ShouldUse(out _))
            {
                SetNinjustus(Katon);
                return false;
            }

            if (Raiton.ShouldUse(out _))
            {
                SetNinjustus(Raiton);
                return false;
            }
        }
        else
        {
            bool empty = Ten.ShouldUse(out _, mustUse: true);
            bool haveDoton = Player.HaveStatus(ObjectStatus.Doton);

            //加状态
            if (Huraijin.ShouldUse(out act)) return true;

            if (JobGauge.HutonTimer > 0 && _ninactionAim?.ID == Huton.ID)
            {
                ClearNinjutsus();
                return false;
            }

            if (empty && (!InCombat || !Huraijin.EnoughLevel) && Huton.ShouldUse(out _))
            {
                SetNinjustus(Huton);
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
            if (Katon.ShouldUse(out _))
            {
                if (!haveDoton && !IsMoving && TenChiJin.WillHaveOneChargeGCD(0, 1)) _ninactionAim = Doton;
                else SetNinjustus(Katon);
                return true;
            }
            //背刺
            if (SettingBreak && Suiton.ShouldUse(out _))
            {
                SetNinjustus(Suiton);
            }
        }
        //常规单体忍术
        if (Ten.ShouldUse(out _) && (!TenChiJin.EnoughLevel || TenChiJin.IsCoolDown))
        {
            if (Raiton.ShouldUse(out _))
            {
                SetNinjustus(Raiton);
                return true;
            }

            if (!Chi.EnoughLevel && FumaShuriken.ShouldUse(out _))
            {
                SetNinjustus(FumaShuriken);
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
        if (Player.HaveStatus(ObjectStatus.TenChiJin))
        {
            uint tenId = Service.IconReplacer.OriginalHook(Ten.ID);
            uint chiId = Service.IconReplacer.OriginalHook(Chi.ID);
            uint jinId = Service.IconReplacer.OriginalHook(Jin.ID);

            //第一个
            if (tenId == FumaShurikenTen.ID)
            {
                //AOE
                if (Katon.ShouldUse(out _))
                {
                    if (FumaShurikenJin.ShouldUse(out act)) return true;
                }
                //Single
                if (FumaShurikenTen.ShouldUse(out act)) return true;
            }

            //第二击杀AOE
            else if (tenId == KatonTen.ID)
            {
                if (KatonTen.ShouldUse(out act, mustUse: true)) return true;
            }
            //其他几击
            else if (chiId == RaitonChi.ID)
            {
                if (RaitonChi.ShouldUse(out act, mustUse: true)) return true;
            }
            else if (chiId == DotonChi.ID)
            {
                if (DotonChi.ShouldUse(out act, mustUse: true)) return true;
            }
            else if (jinId == SuitonJin.ID)
            {
                if (SuitonJin.ShouldUse(out act, mustUse: true)) return true;
            }
        }

        if (_ninactionAim == null) return false;

        uint id = Service.IconReplacer.OriginalHook(2260);

        //没开始，释放第一个
        if (id == 2260)
        {
            //重置
            if (!Player.HaveStatus(ObjectStatus.Kassatsu, ObjectStatus.TenChiJin)
                && !Ten.ShouldUse(out _, mustUse: true))
            {
                return false;
            }
            act = _ninactionAim.Ninjutsus[0];
            return true;
        }
        //失败了
        else if (id == RabbitMedium.ID)
        {
            ClearNinjutsus();
            act = null;
            return false;
        }
        //结束了
        else if (id == _ninactionAim.ID)
        {
            if (_ninactionAim.ShouldUse(out act, mustUse: true)) return true;
            if (_ninactionAim.ID == Doton.ID && !InCombat)
            {
                act = _ninactionAim;
                return true;
            }
        }
        //释放第二个
        else if (id == FumaShuriken.ID)
        {
            if (_ninactionAim.Ninjutsus.Length > 1)
            {
                act = _ninactionAim.Ninjutsus[1];
                return true;
            }
        }
        //释放第三个
        else if (id == Katon.ID || id == Raiton.ID || id == Hyoton.ID)
        {
            if (_ninactionAim.Ninjutsus.Length > 2)
            {
                act = _ninactionAim.Ninjutsus[2];
                return true;
            }
        }
        //ClearNinjutsus();
        return false;
    }

    private protected override bool GeneralGCD(out IAction act)
    {
        if (IsLastAction(false, DotonChi, SuitonJin,
            RabbitMedium, FumaShuriken, Katon, Raiton,
            Hyoton, Huton, Doton, Suiton, GokaMekkyaku, HyoshoRanryu))
        {
            ClearNinjutsus();
        }
        if (ChoiceNinjutsus(out act)) return true;
        if (DoNinjutsus(out act)) return true;

        //用真北取消隐匿
        if (Config.GetBoolByName("AutoUnhide") && Player.HaveStatus(ObjectStatus.Hidden))
        {
            CommandController.SubmitToChat($"/statusoff {StatusHelper.GetStatusName(ObjectStatus.Hidden)}");
        }
        //用隐匿恢复忍术数量
        if (!InCombat && _ninactionAim == null && Config.GetBoolByName("UseHide")
            && Ten.IsCoolDown && Hide.ShouldUse(out act)) return true;

        var replace = Service.IconReplacer.OriginalHook(2260);
        //无忍术或者忍术中途停了
        if (_ninactionAim == null || (replace != 2260 && replace != _ninactionAim.ID))
        {
            //大招
            if (FleetingRaiju.ShouldUse(out act)) return true;
            if (ForkedRaiju.ShouldUse(out act))
            {
                if (TargetFilter.DistanceToPlayer(ForkedRaiju.Target) < 2)
                {
                    return true;
                }
            }

            if (PhantomKamaitachi.ShouldUse(out act)) return true;

            if (Huraijin.ShouldUse(out act)) return true;

            //AOE
            if (HakkeMujinsatsu.ShouldUse(out act)) return true;
            if (DeathBlossom.ShouldUse(out act)) return true;

            //Single
            if (ArmorCrush.ShouldUse(out act)) return true;
            if (AeolianEdge.ShouldUse(out act)) return true;
            if (GustSlash.ShouldUse(out act)) return true;
            if (SpinningEdge.ShouldUse(out act)) return true;

            //飞刀
            if (CommandController.Move && MoveAbility(1, out act)) return true;
            if (ThrowingDagger.ShouldUse(out act)) return true;
        }

        act = null;
        return false;
    }

    private protected override bool MoveGCD(out IAction act)
    {
        if (ForkedRaiju.ShouldUse(out act)) return true;
        return base.MoveGCD(out act);
    }

    private protected override bool DefenceSingleAbility(byte abilityRemain, out IAction act)
    {
        if (ShadeShift.ShouldUse(out act)) return true;

        return false;
    }

    private protected override bool AttackAbility(byte abilityRemain, out IAction act)
    {
        act = null;
        if (!InCombat || Service.IconReplacer.OriginalHook(2260) != 2260) return false;

        //夺取
        if (SettingBreak && Mug.ShouldUse(out act)) return true;


        //解决Buff
        if (TrickAttack.ShouldUse(out act)) return true;
        if (Meisui.ShouldUse(out act)) return true;

        if (!IsMoving && TenChiJin.ShouldUse(out act)) return true;
        if (Kassatsu.ShouldUse(out act)) return true;
        if (UseBreakItem(out act)) return true;

        if (JobGauge.Ninki >= 50)
        {
            if (Bunshin.ShouldUse(out act)) return true;
            if (HellfrogMedium.ShouldUse(out act)) return true;
            if (Bhavacakra.ShouldUse(out act)) return true;
        }

        if (!DreamWithinaDream.EnoughLevel)
        {
            if (Assassinate.ShouldUse(out act)) return true;
        }
        else
        {
            if (DreamWithinaDream.ShouldUse(out act)) return true;
        }
        return false;
    }

    private protected override bool MoveAbility(byte abilityRemain, out IAction act)
    {
        if (Shukuchi.ShouldUse(out act)) return true;

        return false;
    }
    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        //牵制
        if (GeneralActions.Feint.ShouldUse(out act)) return true;
        return false;
    }
}
