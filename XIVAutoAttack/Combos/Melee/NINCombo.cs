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

namespace XIVAutoAttack.Combos.Melee;

internal sealed class NINCombo : JobGaugeCombo<NINGauge>
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

    private static NinAction _ninactionAim = null;

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
                OtherCheck = b => TrickAttack.WillHaveOneChargeGCD(1,1),
            },


            //劫火灭却之术
            GokaMekkyaku = new (16491, Chi, Ten),


            //冰晶乱流之术
            HyoshoRanryu = new (16492, Ten, Jin);
    }

    private protected override ActionConfiguration CreateConfiguration()
    {
        return base.CreateConfiguration().SetBool("UseHide", true, "脱战隐身恢复忍术").SetBool("AutoUnhide", true, "自动取消隐身");
    }

    internal override SortedList<DescType, string> Description => new ()
    {
        {DescType.单体防御, $"{Actions.ShadeShift}"},
        {DescType.移动技能, $"{Actions.Shukuchi}，目标为面向夹角小于30°内最远目标。"},
    };

    private static void SetNinjustus(NinAction act)
    {
        if (_ninactionAim != null && IsLastAction(false, Actions.Ten, Actions.Jin, Actions.Chi, Actions.FumaShurikenTen, Actions.FumaShurikenJin)) return;
        _ninactionAim = act;
    }

    private  bool ChoiceNinjutsus(out IAction act)
    {
        act = null;
        if (Service.IconReplacer.OriginalHook(2260) != 2260) return false;

        //在GCD快转完的时候再判断是否调整非空忍术
        if (_ninactionAim != null && ActionUpdater.AbilityRemainCount != 0) return false;

        //有生杀予夺
        if (Player.HaveStatus(ObjectStatus.Kassatsu))
        {
            if (Actions.GokaMekkyaku.ShouldUse(out _))
            {
                SetNinjustus(Actions.GokaMekkyaku);
                return false;
            }
            if (Actions.HyoshoRanryu.ShouldUse(out _))
            {
                SetNinjustus(Actions.HyoshoRanryu);
                return false;
            }

            if (Actions.Katon.ShouldUse(out _))
            {
                SetNinjustus(Actions.Katon);
                return false;
            }

            if (Actions.Raiton.ShouldUse(out _))
            {
                SetNinjustus(Actions.Raiton);
                return false;
            }
        }
        else
        {
            bool empty = Actions.Ten.ShouldUse(out _, mustUse: true);
            bool haveDoton = Player.HaveStatus(ObjectStatus.Doton);

            //加状态
            if (Actions.Huraijin.ShouldUse(out act)) return true;

            if (JobGauge.HutonTimer > 0 && _ninactionAim?.ID == Actions.Huton.ID)
            {
                ClearNinjutsus();
                return false;
            }

            if (empty && (!InCombat || !Actions.Huraijin.EnoughLevel) && Actions.Huton.ShouldUse(out _))
            {
                SetNinjustus(Actions.Huton);
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
                if(!haveDoton && !IsMoving && Actions.TenChiJin.WillHaveOneChargeGCD(0, 1)) _ninactionAim = Actions.Doton;
                else SetNinjustus(Actions.Katon);
                return true;
            }
            //背刺
            if (SettingBreak && Actions.Suiton.ShouldUse(out _))
            {
                SetNinjustus(Actions.Suiton);
            }
        }
        //常规单体忍术
        if (Actions.Ten.ShouldUse(out _) && (!Actions.TenChiJin.EnoughLevel || Actions.TenChiJin.IsCoolDown))
        {
            if (Actions.Raiton.ShouldUse(out _))
            {
                SetNinjustus(Actions.Raiton);
                return true;
            }

            if (!Actions.Chi.EnoughLevel && Actions.FumaShuriken.ShouldUse(out _))
            {
                SetNinjustus(Actions.FumaShuriken);
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
            if(!Player.HaveStatus(ObjectStatus.Kassatsu, ObjectStatus.TenChiJin) 
                && !Actions.Ten.ShouldUse(out _, mustUse: true))
            {
                return false;
            }
            act = _ninactionAim.Ninjutsus[0];
            return true;
        }
        //失败了
        else if (id == Actions.RabbitMedium.ID)
        {
            ClearNinjutsus();
            act = null;
            return false;
        }
        //结束了
        else if (id == _ninactionAim.ID)
        {
            if (_ninactionAim.ShouldUse(out act, mustUse: true)) return true;
            if (_ninactionAim.ID == Actions.Doton.ID && !InCombat)
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
        //ClearNinjutsus();
        return false;
    }

    private protected override bool GeneralGCD(uint lastComboActionID, out IAction act)
    {
        if (IsLastAction(false, Actions.DotonChi, Actions.SuitonJin,
            Actions.RabbitMedium, Actions.FumaShuriken, Actions.Katon, Actions.Raiton,
            Actions.Hyoton, Actions.Huton, Actions.Doton, Actions.Suiton, Actions.GokaMekkyaku, Actions.HyoshoRanryu))
        {
            ClearNinjutsus();
        }
        if (ChoiceNinjutsus(out act)) return true;
        if (DoNinjutsus(out act)) return true;

        //用真北取消隐匿
        if (Config.GetBoolByName("AutoUnhide") && Player.HaveStatus(ObjectStatus.Hidden))
        {
            //Service.ChatGui.Print($"/statusoff {StatusHelper.GetStatusName(ObjectStatus.Hidden)}");
            CommandController.SubmitToChat($"/statusoff {StatusHelper.GetStatusName(ObjectStatus.Hidden)}");
        }
        //用隐匿恢复忍术数量
        if (!InCombat && _ninactionAim == null && Config.GetBoolByName("UseHide")
            && Actions.Ten.IsCoolDown && Actions.Hide.ShouldUse(out act)) return true;

        var replace = Service.IconReplacer.OriginalHook(2260);
        //无忍术或者忍术中途停了
        if (_ninactionAim == null || (replace != 2260 && replace != _ninactionAim.ID))
        {
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
            if (CommandController.Move && MoveAbility(1, out act)) return true;
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

    private protected override bool AttackAbility(byte abilityRemain, out IAction act)
    {
        act = null;
        if (!InCombat || Service.IconReplacer.OriginalHook(2260) != 2260) return false;

        //夺取
        if (SettingBreak && Actions.Mug.ShouldUse(out act)) return true;


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

        if (!Actions.DreamWithinaDream.EnoughLevel)
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
