using Dalamud.Game.ClientState.JobGauge.Types;
using System.Collections.Generic;

namespace XIVComboPlus.Combos;

internal class NINCombo : CustomComboJob<NINGauge>
{
    internal override uint JobID => 30;

    public class NinAction : BaseAction
    {
        internal Ninjutsu[] Ninjutsus { get; }
        public NinAction(uint actionID, Ninjutsu[] ninjutsus)
            : base(actionID, false, false)
        {
            Ninjutsus = ninjutsus;
        }
    }
    internal enum Ninjutsu : byte
    {
        None,
        Ten,
        Chi,
        Jin,
    }

    private static bool _break = false;
    private static bool _TenChiJin = false;
    internal static readonly List<Ninjutsu> _ninjutsus = new List<Ninjutsu>(3);
    internal static NinAction _ninactionAim = null;

    internal struct Actions
    {
        public static readonly BaseAction
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
            Ten = new BaseAction(2259)
            {
                AfterUse = () => _ninjutsus.Add(Ninjutsu.Ten),
            },

            //地之印
            Chi = new BaseAction(2261)
            {
                AfterUse = () => _ninjutsus.Add(Ninjutsu.Chi),
            },

            //人之印
            Jin = new BaseAction(2263)
            {
                AfterUse = () => _ninjutsus.Add(Ninjutsu.Jin),
            },

            //天地人
            TenChiJin = new BaseAction(7403)
            {
                BuffsProvide = new ushort[] { ObjectStatus.Kassatsu },
                AfterUse = () => _TenChiJin = true,
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
            },

            //八卦无刃杀
            HakkeMujinsatsu = new BaseAction(16488),

            //强甲破点突
            ArmorCrush = new BaseAction(3563)
            {
                EnermyLocation = EnemyLocation.Side,
                OtherCheck = b => JobGauge.HutonTimer < 30000,
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
            DreamWithinaDream = new BaseAction(3566),

            //通灵之术
            RabbitMedium = new BaseAction(2272);

        public static readonly NinAction

            //火遁之术
            Katon = new NinAction(2266, new Ninjutsu[] { Ninjutsu.Chi, Ninjutsu.Ten })
            {
                AfterUse = ClearNinjutsus,
            },

            //雷遁之术
            Raiton = new NinAction(2267, new Ninjutsu[] { Ninjutsu.Ten, Ninjutsu.Chi })
            {
                AfterUse = ClearNinjutsus,
            },


            //冰遁之术
            //Hyoton = new NinAction(2268, new Ninjutsu[] { Ninjutsu.Ten, Ninjutsu.Jin }),

            //风遁之术
            Huton = new NinAction(2269, new Ninjutsu[] { Ninjutsu.Jin, Ninjutsu.Chi, Ninjutsu.Ten })
            {
                OtherCheck = b => JobGauge.HutonTimer == 0,
                AfterUse = ClearNinjutsus,
            },

            //土遁之术
            Doton = new NinAction(2270, new Ninjutsu[] { Ninjutsu.Jin, Ninjutsu.Ten,Ninjutsu.Chi })
            {
                BuffsProvide = new ushort[] { ObjectStatus.Doton },
                AfterUse = ClearNinjutsus,
            },

            //水遁之术
            Suiton = new NinAction(2271, new Ninjutsu[] { Ninjutsu.Ten, Ninjutsu.Chi, Ninjutsu.Jin })
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
            GokaMekkyaku = new NinAction(16491, new Ninjutsu[] { Ninjutsu.Chi, Ninjutsu.Ten })
            {
                AfterUse = ClearNinjutsus,
            },


            //冰晶乱流之术
            HyoshoRanryu = new NinAction(16492, new Ninjutsu[] { Ninjutsu.Ten, Ninjutsu.Jin })
            {
                AfterUse = ClearNinjutsus,
            };
    }

    private static void ClearNinjutsus()
    {
        _ninactionAim = null;
        _ninjutsus.Clear();
    }

    private protected override bool BreakAbility(byte abilityRemain, out BaseAction act)
    {
        _break = true;
        act = null;
        return false;
    }

    private protected override bool GeneralGCD(uint lastComboActionID, out BaseAction act)
    {
        bool haveKassatsu = BaseAction.HaveStatusSelfFromSelf(ObjectStatus.Kassatsu) && Actions.Kassatsu.RecastTimeElapsed < 2;
        bool onninjutsus = BaseAction.HaveStatusSelfFromSelf(ObjectStatus.Ninjutsu);

        //没时间释放忍术了
        var times = BaseAction.FindStatusSelfFromSelf(ObjectStatus.Ninjutsu);
        int count = _ninjutsus.Count;

        if ((_ninjutsus.Count > 0 && (times != null && times[0] < 0.1)) || (_ninjutsus.Count > 1 && times == null))
        {
            ClearNinjutsus();
        }

        //看看有没有机会调整
        if (_ninactionAim != null && count == 0)
        {
            if (haveKassatsu)
            {
                ClearNinjutsus();
                if (Actions.GokaMekkyaku.ShouldUseAction(out _)) _ninactionAim = Actions.GokaMekkyaku;
                if (Actions.HyoshoRanryu.ShouldUseAction(out _)) _ninactionAim = Actions.HyoshoRanryu;
                if (Actions.Katon.ShouldUseAction(out _)) _ninactionAim = Actions.Katon;
                if (Actions.Raiton.ShouldUseAction(out _)) _ninactionAim = Actions.Raiton;
            }
            //调整忍术！
            if (_ninactionAim.OtherCheck != null && !_ninactionAim.OtherCheck(null))
            {
                ClearNinjutsus();
            }
        }
        else
        {
            //看看能否背刺
            if (Actions.Ten.ShouldUseAction(out _, Empty: true) && Actions.TrickAttack.RecastTimeRemain < 3 && _break && !haveKassatsu)
            {
                if (Actions.Suiton.ShouldUseAction(out _)) _ninactionAim = Actions.Suiton;
            }
            //常规忍术
            else if (Actions.Ten.ShouldUseAction(out _) || haveKassatsu)
            {
                if (!IsMoving && Actions.Doton.ShouldUseAction(out _)) _ninactionAim = Actions.Doton;
                if (Actions.Katon.ShouldUseAction(out _)) _ninactionAim = Actions.Katon;
                if (Actions.Raiton.ShouldUseAction(out _)) _ninactionAim = Actions.Raiton;

                if (!TargetHelper.InBattle)
                {
                    //加状态
                    if (Actions.Huton.ShouldUseAction(out _)) _ninactionAim = Actions.Huton;
                }
            }
        }

        //释放忍术
        if (_ninactionAim != null)
        {

            if (count == _ninactionAim.Ninjutsus.Length)
            {
                bool correct = true;
                for (int i = 0; i < _ninactionAim.Ninjutsus.Length; i++)
                {
                    if (_ninactionAim.Ninjutsus[i] != _ninjutsus[i])
                    {
                        correct = false;
                        break;
                    }
                }
                //如果按对了
                if (correct)
                {
                    //天地人的话，就使用了
                    if (_TenChiJin)
                    {
                        _TenChiJin = false;
                        ClearNinjutsus();
                    }
                    else if (_ninactionAim.ShouldUseAction(out act, mustUse: true)) return true;
                    //暂时打不到，先不释普通技能。
                    else
                    {
                        act = null;
                        return true;
                    }
                }
                //如果有错误，重置
                else
                {
                    ClearNinjutsus();
                }
            }
            //释放忍术没中断
            else if(count == 0 || onninjutsus)
            {
                switch (_ninactionAim.Ninjutsus[count])
                {
                    case Ninjutsu.Ten:
                        act = Actions.Ten;
                        return true;
                    case Ninjutsu.Chi:
                        act = Actions.Chi;
                        return true;
                    case Ninjutsu.Jin:
                        act = Actions.Jin;
                        return true;
                }
            }
            //中断了，没救了。
            else
            {
                ClearNinjutsus();
                act = Actions.RabbitMedium;
                return true;
            }
        }

        if (!onninjutsus)
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
            if (Actions.TenChiJin.ShouldUseAction(out act))
            {
                if (Actions.Katon.ShouldUseAction(out _)) _ninactionAim = Actions.Doton;
                if (Actions.Raiton.ShouldUseAction(out _)) _ninactionAim = Actions.Suiton;
                return true;
            }
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

        if(Service.ClientState.LocalPlayer.Level < Actions.DreamWithinaDream.Level)
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
}
