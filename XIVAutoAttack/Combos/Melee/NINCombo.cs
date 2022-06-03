using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboPlus.Combos;

internal class NINCombo : CustomComboJob<NINGauge>
{
    internal override uint JobID => 30;
    protected override bool ShouldSayout => true;

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
            Hide = new BaseAction(2245),
            //双刃旋
            SpinningEdge = new BaseAction(2240),

            //残影
            ShadeShift = new BaseAction(2241),

            //绝风
            GustSlash = new BaseAction(2242),

            //飞刀
            ThrowingDagger = new BaseAction(2247),

            //夺取
            Mug = new BaseAction(2248)
            {
                OtherCheck = b => JobGauge.Ninki <= 50,
            },

            //攻其不备
            TrickAttack = new BaseAction(2258)
            {
                BuffsNeed = new ushort[] { ObjectStatus.Suiton, ObjectStatus.Hidden },
                EnermyLocation = EnemyLocation.Back,
            },

            //旋风刃
            AeolianEdge = new BaseAction(2255)
            {
                EnermyLocation = EnemyLocation.Back,
            },

            //血雨飞花
            DeathBlossom = new BaseAction(2254),

            //天之印
            Ten = new BaseAction(2259),

            //地之印
            Chi = new BaseAction(2261),

            //人之印
            Jin = new BaseAction(2263),

            //天地人
            TenChiJin = new BaseAction(7403)
            {
                BuffsProvide = new ushort[] { ObjectStatus.Kassatsu , ObjectStatus.TenChiJin},
            },

            //缩地
            Shukuchi = new BaseAction(2262, true),

            //断绝
            Assassinate = new BaseAction(2246),

            //命水
            Meisui = new BaseAction(16489)
            {
                BuffsNeed = new ushort[] { ObjectStatus.Suiton },
                OtherCheck = b => JobGauge.Ninki < 50
            },

            //生杀予夺
            Kassatsu = new BaseAction(2264)
            {
                OtherCheck = b => Ten.IsCoolDown,
                BuffsProvide = new ushort[] { ObjectStatus.Kassatsu, ObjectStatus.TenChiJin },
            },

            //八卦无刃杀
            HakkeMujinsatsu = new BaseAction(16488),

            //强甲破点突
            ArmorCrush = new BaseAction(3563)
            {
                EnermyLocation = EnemyLocation.Side,
                OtherCheck = b => JobGauge.HutonTimer < 30000 && JobGauge.HutonTimer > 0,
            },

            //通灵之术·大虾蟆
            HellfrogMedium = new BaseAction(7401),

            //六道轮回
            Bhavacakra = new BaseAction(7402),

            //分身之术
            Bunshin = new BaseAction(16493),

            //残影镰鼬
            PhantomKamaitachi = new BaseAction(25774)
            {
                BuffsNeed = new ushort[] { ObjectStatus.PhantomKamaitachiReady },
            },

            //月影雷兽牙
            FleetingRaiju = new BaseAction(25778)
            {
                BuffsNeed = new ushort[] { ObjectStatus.RaijuReady },
            },

            //月影雷兽爪
            ForkedRaiju = new BaseAction(25777)
            {
                BuffsNeed = new ushort[] { ObjectStatus.RaijuReady },
            },

            //风来刃
            Huraijin = new BaseAction(25876)
            {
                OtherCheck = b => JobGauge.HutonTimer == 0,
            },

            //梦幻三段
            DreamWithinaDream = new BaseAction(3566);

        public static readonly NinAction

            //通灵之术
            RabbitMedium = new NinAction(2272),


            //风魔手里剑
            FumaShuriken = new NinAction(2265, Ten )
            {
                OtherCheck = b => Service.ClientState.LocalPlayer.Level < 35,
                AfterUse = ClearNinjutsus,
            },

            //火遁之术
            Katon = new NinAction(2266, Chi, Ten )
            {
                AfterUse = ClearNinjutsus,
            },

            //雷遁之术
            Raiton = new NinAction(2267,  Ten, Chi )
            {
                AfterUse = ClearNinjutsus,
            },


            //冰遁之术
            Hyoton = new NinAction(2268,Ten, Jin ),

            //风遁之术
            Huton = new NinAction(2269,  Jin, Chi, Ten )
            {
                OtherCheck = b => JobGauge.HutonTimer == 0,
                AfterUse = ClearNinjutsus,
            },

            //土遁之术
            Doton = new NinAction(2270,  Jin, Ten, Chi )
            {
                BuffsProvide = new ushort[] { ObjectStatus.Doton },
                AfterUse = ClearNinjutsus,
            },

            //水遁之术
            Suiton = new NinAction(2271,  Ten, Chi, Jin )
            {
                EnermyLocation = EnemyLocation.Back,
                BuffsProvide = new ushort[] { ObjectStatus.Suiton },
                AfterUse = () => 
                {
                    ClearNinjutsus();
                    _break = false;
                },
            },


            //劫火灭却之术
            GokaMekkyaku = new NinAction(16491,  Chi, Ten )
            {
                AfterUse = ClearNinjutsus,
            },


            //冰晶乱流之术
            HyoshoRanryu = new NinAction(16492,  Ten, Jin )
            {
                AfterUse = ClearNinjutsus,
            };
    }

    private static void ChoiceNinjutsus()
    {
        if (Service.IconReplacer.OriginalHook(2260) != 2260) return;
        //有生杀予夺
        if (BaseAction.HaveStatusSelfFromSelf(ObjectStatus.Kassatsu) && Actions.Kassatsu.RecastTimeElapsed < 2)
        {
            if (Actions.GokaMekkyaku.ShouldUseAction(out _)) _ninactionAim = Actions.GokaMekkyaku;
            if (Actions.HyoshoRanryu.ShouldUseAction(out _)) _ninactionAim = Actions.HyoshoRanryu;
            if (Actions.Katon.ShouldUseAction(out _)) _ninactionAim = Actions.Katon;
            if (Actions.Raiton.ShouldUseAction(out _)) _ninactionAim = Actions.Raiton;
        }

        else
        {
            //看看能否背刺
            if (Actions.Ten.RecastTimeRemain <= 20 && Actions.TrickAttack.RecastTimeRemain < 3 && _break)
            {
                if (Actions.Suiton.ShouldUseAction(out _)) _ninactionAim = Actions.Suiton;
            }
            //常规忍术
            else if (Actions.Ten.ShouldUseAction(out _))
            {
                if (!IsMoving && Actions.Doton.ShouldUseAction(out _)) _ninactionAim = Actions.Doton;
                if (Actions.Katon.ShouldUseAction(out _)) _ninactionAim = Actions.Katon;
                if (Actions.Raiton.ShouldUseAction(out _)) _ninactionAim = Actions.Raiton;
                if (Actions.FumaShuriken.ShouldUseAction(out _)) _ninactionAim = Actions.FumaShuriken;

                if (!TargetHelper.InBattle)
                {
                    //加状态
                    if (Actions.Huton.ShouldUseAction(out _)) _ninactionAim = Actions.Huton;
                }
            }
        }
    }

    private static void ClearNinjutsus()
    {
        _ninactionAim = null;
    }

    private static bool DoNinjutsus(out BaseAction act)
    {
        act = null;

        uint tenId = Service.IconReplacer.OriginalHook(Actions.Ten.ActionID);
        //有天地人
        if (tenId != Actions.Ten.ActionID)
        {
            uint chiId = Service.IconReplacer.OriginalHook(Actions.Chi.ActionID);
            uint jinId = Service.IconReplacer.OriginalHook(Actions.Jin.ActionID);

            //第一个
            if (tenId == Actions.FumaShuriken.ActionID)
            {
                //AOE
                if (Actions.Katon.ShouldUseAction(out _))
                {
                    act = Actions.Jin;
                    return true;
                }
                //Single
                if (Actions.Raiton.ShouldUseAction(out _))
                {
                    act = Actions.Ten;
                    return true;
                }
            }

            //第二击杀AOE
            else if(tenId == Actions.Katon.ActionID)
            {
                Actions.Katon.ShouldUseAction(out _, mustUse: true);
                Actions.Ten.Target = Actions.Katon.Target;
                act = Actions.Ten;
                return true;
            }
            //其他几击
            else if (chiId == Actions.Raiton.ActionID)
            {
                Actions.Raiton.ShouldUseAction(out _, mustUse:true);
                Actions.Chi.Target = Actions.Raiton.Target;
                act = Actions.Chi;
                return true;
            }
            else if (chiId == Actions.Doton.ActionID)
            {
                Actions.Doton.ShouldUseAction(out _, mustUse: true);
                Actions.Chi.Target = Actions.Doton.Target;
                act = Actions.Chi;
                return true;
            }
            else if (jinId == Actions.Suiton.ActionID)
            {
                Actions.Suiton.ShouldUseAction(out _, mustUse: true);
                Actions.Jin.Target = Actions.Suiton.Target;
                act = Actions.Jin;
                return true;
            }
        }

        if (_ninactionAim == null) return false;

        uint id = Service.IconReplacer.OriginalHook(2260);

        //没开始，释放第一个
        if (id == 2260)
        {
            if(_ninactionAim.Ninjutsus[0].ShouldUseAction(out act)) return true;
        }
        //失败了
        else if (id == Actions.RabbitMedium.ActionID)
        {
            act = Actions.RabbitMedium;
            return true;
        }
        //结束了
        else if (id == _ninactionAim.ActionID)
        {
            if (_ninactionAim.ShouldUseAction(out act, mustUse:true)) return true;
        }
        //释放第二个
        else if (id == Actions.FumaShuriken.ActionID )
        {
            if (_ninactionAim.Ninjutsus.Length > 1)
            {
                act = _ninactionAim.Ninjutsus[1];
                return true;
            }
        }
        //释放第三个
        else if (id == Actions.Katon.ActionID || id == Actions.Raiton.ActionID || id == Actions.Hyoton.ActionID)
        {
            if (_ninactionAim.Ninjutsus.Length > 2)
            {
                act = _ninactionAim.Ninjutsus[2];
                return true;
            }
        }
        return false;
    }

    private protected override bool BreakAbility(byte abilityRemain, out BaseAction act)
    {
        _break = true;
        act = null;
        return false;
    }

    private protected override bool GeneralGCD(uint lastComboActionID, out BaseAction act)
    {
        ChoiceNinjutsus();
        if(DoNinjutsus(out act)) return true;
        if (!TargetHelper.InBattle && Actions.Ten.IsCoolDown && Actions.Hide.ShouldUseAction(out act)) return true;


        if (!BaseAction.HaveStatusSelfFromSelf(ObjectStatus.Ninjutsu))
        {
            //大招
            if (Actions.FleetingRaiju.ShouldUseAction(out act, lastComboActionID)) return true;
            if (Actions.ForkedRaiju.ShouldUseAction(out act, lastComboActionID)) return true;
            if (Actions.PhantomKamaitachi.ShouldUseAction(out act, lastComboActionID)) return true;

            //加状态
            if (Actions.Huraijin.ShouldUseAction(out act, lastComboActionID)) return true;

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
        else
        {
            ClearNinjutsus();
        }

        act = null;
        return false;
    }



    private protected override bool DefenceSingleAbility(byte abilityRemain, out BaseAction act)
    {
        if (Actions.ShadeShift.ShouldUseAction(out act)) return true;

        return false;
    }

    private protected override bool ForAttachAbility(byte abilityRemain, out BaseAction act)
    {
        if(Actions.TrickAttack.RecastTimeElapsed <= 12 && Actions.TrickAttack.IsCoolDown)
        {
            if (Actions.TenChiJin.ShouldUseAction(out act)) return true;
            if (Actions.Kassatsu.ShouldUseAction(out act)) return true;
        }
        if (Actions.TrickAttack.ShouldUseAction(out act)) return true;

        if (JobGauge.Ninki >= 50)
        {
            if (Actions.Bunshin.ShouldUseAction(out act)) return true;
            if (Actions.HellfrogMedium.ShouldUseAction(out act)) return true;
            if (Actions.Bhavacakra.ShouldUseAction(out act)) return true;
        }
        if (Actions.Meisui.ShouldUseAction(out act)) return true;


        if (Actions.Mug.ShouldUseAction(out act)) return true;


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

    private protected override bool MoveAbility(byte abilityRemain, out BaseAction act)
    {
        if (Actions.Shukuchi.ShouldUseAction(out act)) return true;

        return false;
    }
    private protected override bool DefenceAreaAbility(byte abilityRemain, out BaseAction act)
    {
        //牵制
        if (GeneralActions.Feint.ShouldUseAction(out act)) return true;
        return false;
    }
}
