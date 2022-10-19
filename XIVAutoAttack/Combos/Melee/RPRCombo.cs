using Dalamud.Game.ClientState.JobGauge.Types;
using System.Collections.Generic;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Combos.CustomCombo;

namespace XIVAutoAttack.Combos.Melee;

internal class RPRCombo : JobGaugeCombo<RPRGauge>
{
    internal class PRPAction : PVEAction
    {
        internal override EnemyLocation EnermyLocation => StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.Enshrouded) 
            ? EnemyLocation.None : base.EnermyLocation;
        internal PRPAction(uint actionID, bool isFriendly = false, bool shouldEndSpecial = false) 
            : base(actionID, isFriendly, shouldEndSpecial)
        {
        }
    }
    internal override uint JobID => 39;
    internal struct Actions
    {
        public static readonly PVEAction
            //死亡之影
            ShadowofDeath = new (24378, isDot:true)
            {
                TargetStatus = new [] { ObjectStatus.DeathsDesign },
                OtherCheck = b =>
                {
                    if (StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.SoulReaver)) return false;
                    if ((StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.Enshrouded) && ArcaneCircle.RecastTimeRemain < 9 &&
                       ((JobGauge.LemureShroud == 4 && StatusHelper.FindStatusTimeFromSelf(b, ObjectStatus.DeathsDesign) < 30) || 
                       (JobGauge.LemureShroud == 3 && StatusHelper.FindStatusTimeFromSelf(b, ObjectStatus.DeathsDesign) < 50))) ||
                        ArcaneCircle.IsCoolDown || !ArcaneCircle.IsCoolDown) return true;
                    return false;
                }
            },

            //切割
            Slice = new (24373),

            //增盈切割
            WaxingSlice = new (24374),

            //地狱切割
            InfernalSlice = new (24375),

            //隐匿挥割
            BloodStalk = new (24389)
            {
                BuffsProvide = new[] { ObjectStatus.SoulReaver },
                OtherCheck = b =>
                {
                    if (!StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.SoulReaver) &&
                        !StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.Enshrouded)) 
                    {
                        if (Level >= Gluttony.Level && Gluttony.RecastTimeRemain < 8) return false;
                    }
                    return true;
                }
            },

            //勾刃
            Harpe = new (24386),

            //绞决
            Gibbet = new PRPAction(24382),

            //缢杀
            Gallows = new PRPAction(24383),

            //灵魂切割
            SoulSlice = new (24380),

            //死亡之涡
            WhorlofDeath = new (24379, isDot: true)
            {
                TargetStatus = new [] { ObjectStatus.DeathsDesign },
                OtherCheck = b =>
                {
                    if (StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.SoulReaver)) return false;
                    return true;
                }
            },

            //旋转钐割
            SpinningScythe = new (24376),

            //噩梦钐割
            NightmareScythe = new (24377),

            //束缚挥割
            GrimSwathe = new(24392)
            {
                OtherCheck = b =>
                {
                    if (!StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.SoulReaver) &&
                        !StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.Enshrouded))
                    {
                        if (Level >= Gluttony.Level && Gluttony.RecastTimeRemain < 8) return false;
                    }
                    return true;
                }
            },

            //暴食
            Gluttony = new(24393)
            {
                OtherCheck = b =>
                {
                    if (!StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.SoulReaver) &&
                        !StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.Enshrouded)) return true;
                    return false;
                },
            },

            //断首
            Guillotine = new (24384),

            //灵魂钐割
            SoulScythe = new (24381),

            //夜游魂衣 变身！
            Enshroud = new(24394)
            {
                OtherCheck = b =>
                {
                    if (!StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.SoulReaver) && !StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.Enshrouded))
                    {
                        if ((StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.ArcaneCircle) && JobGauge.Shroud >= 50)) return true;
                        if (!StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.ArcaneCircle) && JobGauge.Shroud >= 90) return true;
                        if (ArcaneCircle.RecastTimeRemain < 5 && JobGauge.Shroud >= 50) return true;
                    }
                    return false;
                },
            },

            //阴冷收割
            GrimReaping = new (24397),

            //团契
            Communio = new(24398),

            //夜游魂切割
            LemuresSlice = new (24399),

            //夜游魂钐割
            LemuresScythe = new (24400),

            //神秘纹 加盾
            ArcaneCrest = new (24404, true),

            //神秘环 加Buff
            ArcaneCircle = new (24405, true)
            {
                OtherCheck = b =>
                {
                    if (TargetHelper.InBattle && JobGauge.VoidShroud < 2 &&
                    b.FindStatusTimeFromSelf(ObjectStatus.DeathsDesign) > 0) return true;
                    return false;
                },
            },

            //播魂种
            Soulsow = new (24387)
            {
                BuffsProvide = new [] { ObjectStatus.Soulsow },
                OtherCheck = b =>
                {
                    if (!TargetHelper.InBattle) return true;
                    return false;
                }
            },

            //收获月
            HarvestMoon = new (24388)
            {
                BuffsNeed = new [] { ObjectStatus.Soulsow },
                OtherCheck = b =>
                {
                    if (TargetHelper.InBattle) return true;
                    return false;
                }
            },

            //地狱入境
            HellsIngress = new(24401)
            {
                BuffsProvide = new [] { ObjectStatus.EnhancedHarpe },
            },

            //地狱出境
            HellsEgress = new(24402)
            {
                BuffsProvide = new[] { ObjectStatus.EnhancedHarpe },
            },

            //大丰收
            PlentifulHarvest = new(24385)
            {
                OtherCheck = b =>
                {
                    if (JobGauge.Shroud <= 50
                     && !StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.CircleofSacrifice)
                     && StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.ImmortalSacrifice)
                     && !StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.SoulReaver)
                     && !StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.Enshrouded)) 
                        return true;
                    return false;
                }
            };
    }
    internal override SortedList<DescType, string> Description => new ()
    {
        {DescType.单体防御, $"{Actions.ArcaneCrest.Action.Name}"},
        {DescType.移动, $"{Actions.HellsIngress.Action.Name}"},
    };
    private protected override bool GeneralGCD(uint lastComboActionID, out IAction act)
    {
        //开场获得收获月
        if(Actions.Soulsow.ShouldUse(out act)) return true;

        //处于变身状态。
        if (StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.Enshrouded))
        {
            if (Actions.ShadowofDeath.ShouldUse(out act)) return true;

            if (JobGauge.LemureShroud == 1 && JobGauge.VoidShroud == 0 && Level >= Actions.Communio.Level)
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

            if (JobGauge.LemureShroud == 1 && JobGauge.VoidShroud == 0 && Level < Actions.Communio.Level)
            {
                if (Actions.Gallows.ShouldUse(out act)) return true;
            }

            if (JobGauge.VoidShroud >= 2)
            {
                if (Actions.LemuresSlice.ShouldUse(out act)) return true;
                if (Actions.LemuresScythe.ShouldUse(out act)) return true;
            }

            if (StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.EnhancedVoidReaping))
            {
                if (Actions.Gibbet.ShouldUse(out act)) return true;
            }
            if (StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.EnhancedCrossReaping))
            {
                if (Actions.Gallows.ShouldUse(out act)) return true;
            }

            if ((!StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.EnhancedVoidReaping) &&
                !StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.EnhancedCrossReaping)))
            {
                if (Actions.Gallows.ShouldUse(out act)) return true;
                if (Actions.GrimReaping.ShouldUse(out act)) return true;
            }
        }

        //处于补蓝状态，赶紧补蓝条。
        else if (StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.SoulReaver))
        {
            if (Actions.Guillotine.ShouldUse(out act)) return true;

            if (StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.EnhancedGibbet))
            {
                if (Actions.Gibbet.ShouldUse(out act)) return true;
            }
            else
            {
                if (Actions.Gallows.ShouldUse(out act)) return true;
            }
        }

        //上Debuff
        if (Actions.WhorlofDeath.ShouldUse(out act, lastComboActionID)) return true;
        if (Actions.ShadowofDeath.ShouldUse(out act, lastComboActionID)) return true;

        //大丰收
        if (Actions.PlentifulHarvest.ShouldUse(out act, mustUse: true)) return true;

        if (!StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.Enshrouded) && 
            !StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.SoulReaver))
        {
            //灵魂不够，v我50！
            if (JobGauge.Soul <= 50)
            {
                if (Actions.SoulScythe.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
                if (Actions.SoulSlice.ShouldUse(out act, emptyOrSkipCombo: true)) return true;
            }

            //群体二连
            if (Actions.NightmareScythe.ShouldUse(out act, lastComboActionID)) return true;
            if (Actions.SpinningScythe.ShouldUse(out act, lastComboActionID)) return true;


            //单体三连
            if (Actions.InfernalSlice.ShouldUse(out act, lastComboActionID)) return true;
            if (Actions.WaxingSlice.ShouldUse(out act, lastComboActionID)) return true;
            if (Actions.Slice.ShouldUse(out act, lastComboActionID)) return true;
        }
        
        //摸不到怪 先花掉收获月
        if (Actions.HarvestMoon.ShouldUse(out act, mustUse:true)) return true;
        if (Actions.Harpe.ShouldUse(out act)) return true;

        return false;
    }

    private protected override bool ForAttachAbility(byte abilityRemain, out IAction act)
    {
        //灵魂够了，攒蓝条。
        if (JobGauge.Soul >= 50)
        {
            //暴食
            if (Actions.Gluttony.ShouldUse(out act, mustUse: true)) return true;
            //AOE
            if (Actions.GrimSwathe.ShouldUse(out act)) return true;
            //单体
            if (Actions.BloodStalk.ShouldUse(out act)) return true;
        }
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
        if (GeneralActions.Feint.ShouldUse(out act)) return true;
        return false;
    }

    private protected override bool MoveAbility(byte abilityRemain, out IAction act)
    {
        //地狱入境
        if (Actions.HellsIngress.ShouldUse(out act) && 
            !StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.Threshold)) return true;
        return false;
    }

    private protected override bool DefenceSingleAbility(byte abilityRemain, out IAction act)
    {
        //神秘纹
        if (Actions.ArcaneCrest.ShouldUse(out act)) return true;
        return false;
    }
}
