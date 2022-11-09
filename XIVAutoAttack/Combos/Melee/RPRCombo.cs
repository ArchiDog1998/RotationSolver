using Dalamud.Game.ClientState.JobGauge.Types;
using System.Collections.Generic;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using static XIVAutoAttack.Combos.Melee.RPRCombo;

namespace XIVAutoAttack.Combos.Melee;

internal sealed class RPRCombo : JobGaugeCombo<RPRGauge, CommandType>
{
    public override ComboAuthor[] Authors => new ComboAuthor[] { ComboAuthor.NiGuangOwO };

    internal enum CommandType : byte
    {
        None,
    }

    protected override SortedList<CommandType, string> CommandDescription => new SortedList<CommandType, string>()
    {
        //{CommandType.None, "" }, //写好注释啊！用来提示用户的。
    };
    internal class PRPAction : BaseAction
    {
        internal override EnemyLocation EnermyLocation => Player.HaveStatus(ObjectStatus.Enshrouded) 
            ? EnemyLocation.None : base.EnermyLocation;
        internal PRPAction(uint actionID, bool isFriendly = false, bool shouldEndSpecial = false) 
            : base(actionID, isFriendly, shouldEndSpecial)
        {
        }
    }

    private static bool enshrouded => Player.HaveStatus(ObjectStatus.Enshrouded);
    private static bool soulReaver => Player.HaveStatus(ObjectStatus.SoulReaver);
    private static bool enhancedGibbet => Player.HaveStatus(ObjectStatus.EnhancedGibbet);
    private static bool enhancedGallows => Player.HaveStatus(ObjectStatus.EnhancedGallows);
    private static bool enhancedCrossReaping => Player.HaveStatus(ObjectStatus.EnhancedCrossReaping);
    private static bool enhancedVoidReaping => Player.HaveStatus(ObjectStatus.EnhancedVoidReaping);
    private static bool plentifulReady => Player.HaveStatus(ObjectStatus.ImmortalSacrifice) && !Player.HaveStatus(ObjectStatus.BloodsownCircle);
    private static bool haveDeathsDesign => Target.HaveStatus(ObjectStatus.DeathsDesign);
    public override uint[] JobIDs => new uint[] { 39 };

    public static readonly BaseAction
    #region 单体
        //切割
        Slice = new(24373)
        {
            OtherCheck = b => !enshrouded || !soulReaver,
        },

        //增盈切割
        WaxingSlice = new(24374)
        {
            OtherCheck = Slice.OtherCheck,
        },

        //地狱切割
        InfernalSlice = new(24375)
        {
            OtherCheck = Slice.OtherCheck,
        },

        //死亡之影
        ShadowofDeath = new(24378, isDot: true)
        {
            TargetStatus = new[] { ObjectStatus.DeathsDesign },
            OtherCheck = b => !soulReaver,
        },

        //灵魂切割
        SoulSlice = new(24380)
        {
            OtherCheck = b => !enshrouded && !soulReaver && JobGauge.Soul <= 50,
        },
    #endregion
    #region AoE
        //旋转钐割
        SpinningScythe = new(24376)
        {
            OtherCheck = Slice.OtherCheck,
        },

        //噩梦钐割
        NightmareScythe = new(24377)
        {
            OtherCheck = Slice.OtherCheck,
        },

        //死亡之涡
        WhorlofDeath = new(24379, isDot: true)
        {
            TargetStatus = new[] { ObjectStatus.DeathsDesign },
            OtherCheck = b => !soulReaver
        },

        //灵魂钐割
        SoulScythe = new(24381)
        {
            OtherCheck = SoulSlice.OtherCheck,
        },
    #endregion
    #region 妖异之镰状态
        //绞决
        Gibbet = new PRPAction(24382)
        {
            OtherCheck = b => soulReaver && enhancedGibbet,
        },

        //缢杀
        Gallows = new PRPAction(24383)
        {
            OtherCheck = b => soulReaver && (enhancedGallows || !enhancedGibbet),
        },

        //断首
        Guillotine = new(24384)
        {
            OtherCheck = b => soulReaver,
        },
    #endregion
    #region 红条50灵魂
        //隐匿挥割
        BloodStalk = new(24389)
        {
            BuffsProvide = new[] { ObjectStatus.SoulReaver },
            OtherCheck = b => !soulReaver && !enshrouded &&
                              JobGauge.Soul >= 50 && !plentifulReady &&
                              ((Gluttony.EnoughLevel && !Gluttony.WillHaveOneChargeGCD(4)) || !Gluttony.EnoughLevel),
        },

        //束缚挥割
        GrimSwathe = new(24392)
        {
            OtherCheck = BloodStalk.OtherCheck,
        },

        //暴食
        Gluttony = new(24393)
        {
            OtherCheck = b => !soulReaver && !enshrouded && JobGauge.Soul >= 50,
        },
    #endregion
    #region 大爆发
        //神秘环
        ArcaneCircle = new(24405, true)
        {
            OtherCheck = b => InCombat && haveDeathsDesign
        },

        //大丰收
        PlentifulHarvest = new(24385)
        {
            OtherCheck = b => JobGauge.Shroud <= 50 && !soulReaver && !enshrouded && plentifulReady
        },
    #endregion
    #region 蓝条50附体
        //夜游魂衣
        Enshroud = new(24394)
        {
            OtherCheck = b => !soulReaver && !enshrouded && JobGauge.Shroud >= 50,
        },

        //团契
        Communio = new(24398)
        {
            OtherCheck = b => enshrouded && JobGauge.LemureShroud == 1,
        },

        //夜游魂切割
        LemuresSlice = new(24399)
        {
            OtherCheck = b => enshrouded && JobGauge.VoidShroud >= 2,
        },

        //夜游魂钐割
        LemuresScythe = new(24400)
        {
            OtherCheck = LemuresSlice.OtherCheck,
        },

        //虚无收割
        VoidReaping = new(24395)
        {
            OtherCheck = b => enshrouded && JobGauge.LemureShroud > 1 && enhancedVoidReaping,
        },

        //交错收割
        CrossReaping = new(24396)
        {
            OtherCheck = b =>
            {
                if (enshrouded)
                {
                    if (JobGauge.LemureShroud > 1 && (enhancedCrossReaping || !enhancedVoidReaping))
                    {
                        return true;
                    }
                    if (JobGauge.LemureShroud == 1 && !Communio.EnoughLevel && enhancedCrossReaping)
                    {
                        return true;
                    }
                }
                return false;
            }
        },

        //阴冷收割
        GrimReaping = new(24397)
        {
            OtherCheck = b => enshrouded,
        },
    #endregion
    #region 杂项
        //勾刃
        Harpe = new(24386),

        //播魂种
        Soulsow = new(24387)
        {
            BuffsProvide = new[] { ObjectStatus.Soulsow },
            OtherCheck = b => !InCombat,
        },

        //收获月
        HarvestMoon = new(24388)
        {
            BuffsNeed = new[] { ObjectStatus.Soulsow },
            OtherCheck = b => InCombat,
        },

        //神秘纹 加盾
        ArcaneCrest = new(24404, true)
        {
            OtherCheck = b => !enshrouded && !soulReaver
        };
    #endregion

    public override SortedList<DescType, string> Description => new ()
    {
        {DescType.单体防御, $"{ArcaneCrest}"},
    };
    private protected override bool GeneralGCD(out IAction act)
    {
        //开场获得收获月
        if (Soulsow.ShouldUse(out act)) return true;

        //处于变身状态。
        if (enshrouded)
        {
            if (ShadowofDeath.ShouldUse(out act)) return true;

            //夜游魂衣-虚无/交错收割 阴冷收割
            if (CrossReaping.ShouldUse(out act)) return true;
            if (VoidReaping.ShouldUse(out act)) return true;
            if (GrimReaping.ShouldUse(out act)) return true;

            if (JobGauge.LemureShroud == 1 && Communio.EnoughLevel)
            {
                if (!IsMoving && Communio.ShouldUse(out act, mustUse: true))
                {
                    return true;
                }
                //跑机制来不及读条？补个buff混一下
                else
                {
                    if (ShadowofDeath.ShouldUse(out act, mustUse: IsMoving)) return true;
                    if (WhorlofDeath.ShouldUse(out act, mustUse: IsMoving)) return true;
                }
            }
        }

        //处于补蓝状态，赶紧补蓝条。
        if (Guillotine.ShouldUse(out act)) return true;
        if (Gibbet.ShouldUse(out act)) return true;
        if (Gallows.ShouldUse(out act)) return true;

        //上Debuff
        if (WhorlofDeath.ShouldUse(out act)) return true;
        if (ShadowofDeath.ShouldUse(out act)) return true;

        //大丰收
        if (PlentifulHarvest.ShouldUse(out act, mustUse: true)) return true;

        //灵魂切割
        if (SoulSlice.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
        //灵魂钐割
        if (SoulScythe.ShouldUse(out act, emptyOrSkipCombo: true)) return true;

        //群体二连
        if (NightmareScythe.ShouldUse(out act)) return true;
        if (SpinningScythe.ShouldUse(out act)) return true;

        //单体三连
        if (InfernalSlice.ShouldUse(out act)) return true;
        if (WaxingSlice.ShouldUse(out act)) return true;
        if (Slice.ShouldUse(out act)) return true;

        //摸不到怪 先花掉收获月
        if (HarvestMoon.ShouldUse(out act, mustUse:true)) return true;
        if (Harpe.ShouldUse(out act)) return true;

        return false;
    }

    private protected override bool AttackAbility(byte abilityRemain, out IAction act)
    {
        if (SettingBreak)
        {
            //神秘环
            if (ArcaneCircle.ShouldUse(out act)) return true;
            //夜游魂衣
            if (Enshroud.ShouldUse(out act)) return true;
        }

        if (enshrouded)
        {
            //夜游魂衣-夜游魂切割 夜游魂钐割
            if (LemuresSlice.ShouldUse(out act)) return true;
            if (LemuresScythe.ShouldUse(out act)) return true;
        }

        //暴食
        if (Gluttony.ShouldUse(out act, mustUse: true)) return true;
        //AOE
        if (GrimSwathe.ShouldUse(out act)) return true;
        //单体
        if (BloodStalk.ShouldUse(out act)) return true;
        act = null;
        return false;
    }

    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        //牵制
        if (!enshrouded && !soulReaver)
        {
            if (GeneralActions.Feint.ShouldUse(out act)) return true;
        }
        act = null;
        return false;
    }

    private protected override bool DefenceSingleAbility(byte abilityRemain, out IAction act)
    {
        //神秘纹
        if (ArcaneCrest.ShouldUse(out act)) return true;
        return false;
    }
}
