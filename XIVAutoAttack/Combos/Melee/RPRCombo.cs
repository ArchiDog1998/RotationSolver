using Dalamud.Game.ClientState.JobGauge.Types;
using System.Collections.Generic;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Helpers;

namespace XIVAutoAttack.Combos.Melee;

internal sealed class RPRCombo : JobGaugeCombo<RPRGauge>
{
    internal class PRPAction : BaseAction
    {
        internal override EnemyLocation EnermyLocation => LocalPlayer.HaveStatus(ObjectStatus.Enshrouded) 
            ? EnemyLocation.None : base.EnermyLocation;
        internal PRPAction(uint actionID, bool isFriendly = false, bool shouldEndSpecial = false) 
            : base(actionID, isFriendly, shouldEndSpecial)
        {
        }
    }

    private static bool enshrouded => LocalPlayer.HaveStatus(ObjectStatus.Enshrouded);
    private static bool soulReaver => LocalPlayer.HaveStatus(ObjectStatus.SoulReaver);
    private static bool enhancedGibbet => LocalPlayer.HaveStatus(ObjectStatus.EnhancedGibbet);
    private static bool enhancedGallows => LocalPlayer.HaveStatus(ObjectStatus.EnhancedGallows);
    private static bool enhancedCrossReaping => LocalPlayer.HaveStatus(ObjectStatus.EnhancedCrossReaping);
    private static bool enhancedVoidReaping => LocalPlayer.HaveStatus(ObjectStatus.EnhancedVoidReaping);
    private static bool arcaneCircle => LocalPlayer.HaveStatus(ObjectStatus.ArcaneCircle);
    private static bool plentifulReady => LocalPlayer.HaveStatus(ObjectStatus.ImmortalSacrifice) && !LocalPlayer.HaveStatus(ObjectStatus.BloodsownCircle);
    private static bool haveDeathsDesign => Target.HaveStatus(ObjectStatus.DeathsDesign);
    internal override uint JobID => 39;
    internal struct Actions
    {
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
                OtherCheck = b => InBattle && haveDeathsDesign
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
                OtherCheck = b => enshrouded && JobGauge.LemureShroud ==1,
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
                OtherCheck = b => !InBattle,
            },

            //收获月
            HarvestMoon = new(24388)
            {
                BuffsNeed = new[] { ObjectStatus.Soulsow },
                OtherCheck = b => InBattle,
            },

            //神秘纹 加盾
            ArcaneCrest = new(24404, true)
            {
                OtherCheck = b => !enshrouded && !soulReaver
            };
        #endregion
    }
    internal override SortedList<DescType, string> Description => new ()
    {
        {DescType.单体防御, $"{Actions.ArcaneCrest.Action.Name}"},
    };
    private protected override bool GeneralGCD(uint lastComboActionID, out IAction act)
    {
        //开场获得收获月
        if (Actions.Soulsow.ShouldUse(out act)) return true;

        //处于变身状态。
        if (enshrouded)
        {
            if (Actions.ShadowofDeath.ShouldUse(out act)) return true;

            //夜游魂衣-虚无/交错收割 阴冷收割
            if (Actions.CrossReaping.ShouldUse(out act)) return true;
            if (Actions.VoidReaping.ShouldUse(out act)) return true;
            if (Actions.GrimReaping.ShouldUse(out act)) return true;

            if (JobGauge.LemureShroud == 1 && Actions.Communio.EnoughLevel)
            {
                if (!IsMoving && Actions.Communio.ShouldUse(out act, mustUse: true))
                {
                    return true;
                }
                //跑机制来不及读条？补个buff混一下
                else
                {
                    if (Actions.ShadowofDeath.ShouldUse(out act, mustUse: IsMoving)) return true;
                    if (Actions.WhorlofDeath.ShouldUse(out act, mustUse: IsMoving)) return true;
                }
            }
        }

        //处于补蓝状态，赶紧补蓝条。
        if (Actions.Guillotine.ShouldUse(out act)) return true;
        if (Actions.Gibbet.ShouldUse(out act)) return true;
        if (Actions.Gallows.ShouldUse(out act)) return true;

        //上Debuff
        if (Actions.WhorlofDeath.ShouldUse(out act)) return true;
        if (Actions.ShadowofDeath.ShouldUse(out act)) return true;

        //大丰收
        if (Actions.PlentifulHarvest.ShouldUse(out act, mustUse: true)) return true;

        //灵魂切割
        if (Actions.SoulSlice.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
        //灵魂钐割
        if (Actions.SoulScythe.ShouldUse(out act, emptyOrSkipCombo: true)) return true;

        //群体二连
        if (Actions.NightmareScythe.ShouldUse(out act, lastComboActionID)) return true;
        if (Actions.SpinningScythe.ShouldUse(out act, lastComboActionID)) return true;

        //单体三连
        if (Actions.InfernalSlice.ShouldUse(out act, lastComboActionID)) return true;
        if (Actions.WaxingSlice.ShouldUse(out act, lastComboActionID)) return true;
        if (Actions.Slice.ShouldUse(out act, lastComboActionID)) return true;

        //摸不到怪 先花掉收获月
        if (Actions.HarvestMoon.ShouldUse(out act, mustUse:true)) return true;
        if (Actions.Harpe.ShouldUse(out act)) return true;

        return false;
    }

    private protected override bool ForAttachAbility(byte abilityRemain, out IAction act)
    {
        if (enshrouded)
        {
            //夜游魂衣-夜游魂切割 夜游魂钐割
            if (Actions.LemuresSlice.ShouldUse(out act)) return true;
            if (Actions.LemuresScythe.ShouldUse(out act)) return true;
        }

        //暴食
        if (Actions.Gluttony.ShouldUse(out act, mustUse: true)) return true;
        //AOE
        if (Actions.GrimSwathe.ShouldUse(out act)) return true;
        //单体
        if (Actions.BloodStalk.ShouldUse(out act)) return true;
        act = null;
        return false;
    }

    private protected override bool BreakAbility(byte abilityRemain, out IAction act)
    {
        //神秘环
        if (Actions.ArcaneCircle.ShouldUse(out act)) return true;
        //夜游魂衣
        if (Actions.Enshroud.ShouldUse(out act)) return true;
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
        if (Actions.ArcaneCrest.ShouldUse(out act)) return true;
        return false;
    }
}
