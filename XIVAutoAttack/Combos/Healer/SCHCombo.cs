using Dalamud.Game.ClientState.JobGauge.Types;
using FFXIVClientStructs.FFXIV.Common.Lua;
using System.Collections.Generic;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.Attributes;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Combos.RangedMagicial;
using XIVAutoAttack.Configuration;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Updaters;
using static XIVAutoAttack.Combos.Healer.SCHCombo;
using System.Linq;

namespace XIVAutoAttack.Combos.Healer;

[ComboDevInfo(@"https://github.com/ArchiDog1998/XIVAutoAttack/blob/main/XIVAutoAttack/Combos/Healer/SCHCombo.cs",
   ComboAuthor.NiGuangOwO)]
internal sealed class SCHCombo : JobGaugeCombo<SCHGauge, CommandType>
{
    internal enum CommandType : byte
    {
        None,
    }

    protected override SortedList<CommandType, string> CommandDescription => new SortedList<CommandType, string>()
    {
        //{CommandType.None, "" }, //写好注释啊！用来提示用户的。
    };
    public override uint[] JobIDs => new uint[] { 28 };

    private protected override BaseAction Raise => SMNCombo.Resurrection;
    protected override bool CanHealSingleSpell => base.CanHealSingleSpell && (Config.GetBoolByName("GCDHeal") || TargetUpdater.PartyHealers.Length < 2);
    protected override bool CanHealAreaSpell => base.CanHealAreaSpell && (Config.GetBoolByName("GCDHeal") || TargetUpdater.PartyHealers.Length < 2);
    private static bool HasAetherflow => JobGauge.Aetherflow > 0;
    private static bool HasSeraph => JobGauge.SeraphTimer > 0;

    public static readonly BaseAction
    #region 治疗
        //医术
        Physick = new(190, true),

        //鼓舞激励之策
        Adloquium = new(185, true)
        {
            TargetStatus = new ushort[]
            {
                    ObjectStatus.EukrasianDiagnosis,
                    ObjectStatus.EukrasianPrognosis,
                    ObjectStatus.Galvanize,
            },
        },

        //士气高扬之策
        Succor = new(186, true)
        {
            BuffsProvide = new[] { ObjectStatus.Galvanize },
        },

        //生命活性法
        Lustrate = new(189, true)
        {
            OtherCheck = b => HasAetherflow
        },

        //野战治疗阵
        SacredSoil = new(188, true)
        {
            OtherCheck = b => HasAetherflow && !IsMoving,
        },

        //不屈不挠之策
        Indomitability = new(3583, true)
        {
            OtherCheck = b => HasAetherflow
        },

        //深谋远虑之策
        Excogitation = new(7434, true)
        {
            OtherCheck = b => HasAetherflow
        },

        //慰藉
        Consolation = new(16546)
        {
            OtherCheck = b => HasSeraph,
        },

        //生命回生法
        Protraction = new(25867, true),
    #endregion
    #region 进攻
        //毒菌
        Bio = new(17864, isDot: true)//猛毒菌 17865 蛊毒法 16540
        {
            TargetStatus = new ushort[] { ObjectStatus.Bio, ObjectStatus.Bio2, ObjectStatus.Biolysis },
        },

        //毁灭
        Ruin = new(17869),//气炎法 3584 魔炎法 7435 死炎法 16541 极炎法 25865

        //毁坏
        Ruin2 = new(17870),

        //能量吸收
        EnergyDrain = new(167)
        {
            OtherCheck = b => HasAetherflow && ((Dissipation.EnoughLevel && Dissipation.WillHaveOneChargeGCD(3)) || Aetherflow.WillHaveOneChargeGCD(3))
        },

        //破阵法
        ArtofWar = new(16539),//裂阵法 25866
    #endregion
    #region 仙女
        //炽天召唤
        SummonSeraph = new(16545)
        {
            OtherCheck = b => TargetUpdater.HavePet,
        },

        //朝日召唤
        SummonEos = new(17215)//夕月召唤 17216
        {
            OtherCheck = b => !TargetUpdater.HavePet && (!Player.HaveStatus(ObjectStatus.Dissipation) || (Dissipation.WillHaveOneCharge(30) && Dissipation.EnoughLevel)),
        },

        //仙光的低语/天使的低语
        WhisperingDawn = new(16537)
        {
            OtherCheck = b => TargetUpdater.HavePet,
        },

        //异想的幻光/炽天的幻光
        FeyIllumination = new(16538)
        {
            OtherCheck = b => TargetUpdater.HavePet,
        },

        //转化
        Dissipation = new(3587)
        {
            OtherCheck = b => !HasAetherflow && !HasSeraph && InCombat && TargetUpdater.HavePet,
        },

        //以太契约-异想的融光
        Aetherpact = new(7437, true)
        {
            OtherCheck = b => JobGauge.FairyGauge >= 10 && TargetUpdater.HavePet && !HasSeraph
        },

        //异想的祥光
        FeyBlessing = new(16543)
        {
            OtherCheck = b => !HasSeraph && TargetUpdater.HavePet,
        },
    #endregion
    #region 其他
        //以太超流
        Aetherflow = new(166)
        {
            OtherCheck = b => InCombat && !HasAetherflow
        },

        //秘策
        Recitation = new(16542),

        //连环计
        ChainStratagem = new(7436)
        {
            OtherCheck = b => InCombat && IsTargetBoss
        },

        //展开战术
        DeploymentTactics = new(3585, true)
        {
            ChoiceTarget = friends =>
            {
                foreach (var friend in friends)
                {
                    if (friend.HaveStatus(ObjectStatus.Galvanize)) return friend;
                }
                return null;
            },
        },

        //应急战术
        EmergencyTactics = new(3586),

        //疾风怒涛之计
        Expedient = new(25868);
    #endregion

    private protected override ActionConfiguration CreateConfiguration()
    {
        return base.CreateConfiguration().SetBool("GCDHeal", false, "自动用GCD奶");
    }
    public override SortedList<DescType, string> DescriptionDict => new()
    {
        {DescType.范围治疗, $"GCD: {Succor}\n                     能力: {SacredSoil}, {SummonSeraph}, {WhisperingDawn}, {FeyBlessing}, {Indomitability}"},
        {DescType.单体治疗, $"GCD: {Adloquium}, {Physick}\n                     能力: {SacredSoil}, {Aetherpact}, {Protraction}, {Excogitation}, {Lustrate}"},
        {DescType.范围防御, $"GCD: {Succor}\n                     能力: {SacredSoil}, {Adloquium}, {SummonSeraph}, {FeyIllumination}, {Expedient}"},
        {DescType.单体防御, $"{Adloquium}"},
    };

    private protected override bool EmergercyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        //秘策绑定单盾群盾
        if (nextGCD.IsAnySameAction(true, Succor, Adloquium))
        {
            if (Recitation.ShouldUse(out act)) return true;
        }

        //以太契约
        foreach (var item in TargetUpdater.PartyMembers)
        {
            if (item.GetHealthRatio() < 0.9) continue;
            foreach (var status in item.StatusList)
            {
                if (status.StatusId == 1223 && status.SourceObject != null
                    && status.SourceObject.OwnerId == Service.ClientState.LocalPlayer.ObjectId)
                {
                    act = Aetherpact;
                    return true;
                }
            }
        }

        return base.EmergercyAbility(abilityRemain, nextGCD, out act);
    }

    private protected override bool GeneralGCD(out IAction act)
    {
        //召唤小仙女
        if (SummonEos.ShouldUse(out act)) return true;

        //DoT
        if (Bio.ShouldUse(out act)) return true;


        //AOE
        if (ArtofWar.ShouldUse(out act)) return true;

        //单体
        if (Ruin.ShouldUse(out act)) return true;
        if (Ruin2.ShouldUse(out act)) return true;

        return false;
    }

    private protected override bool HealSingleGCD(out IAction act)
    {
        //鼓舞激励之策
        if (Adloquium.ShouldUse(out act)) return true;

        //医术
        if (Physick.ShouldUse(out act)) return true;

        return false;
    }

    private protected override bool HealSingleAbility(byte abilityRemain, out IAction act)
    {
        //判断是否有人有线
        var haveLink = TargetUpdater.PartyMembers.Any(p =>
        p.StatusList.Any(
            status => (status.StatusId == 1223 && status.SourceObject != null
            && status.SourceObject.OwnerId == Service.ClientState.LocalPlayer.ObjectId))
     );
        //以太契约
        if (Aetherpact.ShouldUse(out act) && JobGauge.FairyGauge >= 70&&!haveLink) return true;
        
        //生命回生法
        if (Protraction.ShouldUse(out act)) return true;

        //野战治疗阵
        if (SacredSoil.ShouldUse(out act)) return true;

        //深谋远虑之策
        if (Excogitation.ShouldUse(out act)) return true;

        //生命活性法
        if (Lustrate.ShouldUse(out act)) return true;

        //以太契约
        if (Aetherpact.ShouldUse(out act)&&!haveLink) return true;
        
        return false;
    }

    private protected override bool DefenceSingleAbility(byte abilityRemain, out IAction act)
    {

        //深谋远虑之策
        if (Excogitation.ShouldUse(out act)) return true;

        return false;
    }

    private protected override bool HealAreaGCD(out IAction act)
    {
        //士气高扬之策
        if (Succor.ShouldUse(out act)) return true;

        return false;
    }

    private protected override bool HealAreaAbility(byte abilityRemain, out IAction act)
    {
        //慰藉
        if (SummonSeraph.ShouldUse(out act)) return true;
        if (Consolation.ShouldUse(out act, emptyOrSkipCombo: true)) return true;

        //异想的祥光
        if (FeyBlessing.ShouldUse(out act)) return true;

        //仙光的低语
        if (WhisperingDawn.ShouldUse(out act)) return true;

        //野战治疗阵
        if (SacredSoil.ShouldUse(out act)) return true;

        //不屈不挠之策
        if (Indomitability.ShouldUse(out act)) return true;

        act = null;
        return false;
    }

    private protected override bool DefenseAreaGCD(out IAction act)
    {
        //士气高扬之策
        if (Succor.ShouldUse(out act)) return true;

        act = null;
        return false;
    }

    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        //异想的幻光
        if (FeyIllumination.ShouldUse(out act)) return true;

        //疾风怒涛之计
        if (Expedient.ShouldUse(out act)) return true;

        //慰藉
        if (SummonSeraph.ShouldUse(out act)) return true;
        if (Consolation.ShouldUse(out act, emptyOrSkipCombo: true)) return true;

        //野战治疗阵
        if (SacredSoil.ShouldUse(out act)) return true;

        return false;
    }

    private protected override bool AttackAbility(byte abilityRemain, out IAction act)
    {
        if (SettingBreak)
        {
            //连环计
            if (ChainStratagem.ShouldUse(out act)) return true;
        }

        //能量吸收
        if (EnergyDrain.ShouldUse(out act)) return true;

        //转化
        if (Dissipation.ShouldUse(out act)) return true;

        //以太超流
        if (Aetherflow.ShouldUse(out act)) return true;

        act = null;
        return false;
    }
}