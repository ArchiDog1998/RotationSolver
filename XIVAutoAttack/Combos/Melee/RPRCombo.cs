using Dalamud.Game.ClientState.JobGauge.Types;
using System.Collections.Generic;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Combos.CustomCombo;

namespace XIVAutoAttack.Combos.Melee;

internal class RPRCombo : JobGaugeCombo<RPRGauge>
{
    internal class PRPAction : BaseAction
    {
        internal override EnemyLocation EnermyLocation => StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.Enshrouded) 
            ? EnemyLocation.None : base.EnermyLocation;
        internal PRPAction(uint actionID, bool isFriendly = false, bool shouldEndSpecial = false) 
            : base(actionID, isFriendly, shouldEndSpecial)
        {
        }
    }
    internal override uint JobID => 39;
    internal static byte level => Service.ClientState.LocalPlayer!.Level;
    internal struct Actions
    {
        public static readonly BaseAction
            //死亡之影
            ShadowofDeath = new (24378, isDot:true)
            {
                TargetStatus = new [] { ObjectStatus.DeathsDesign },
            },

            //切割
            Slice = new (24373),

            //增盈切割
            WaxingSlice = new (24374),

            //地狱切割
            InfernalSlice = new (24375),

            //隐匿挥割
            BloodStalk = new (24389),

            //勾刃
            Harpe = new (24386) { BuffsProvide = new [] { ObjectStatus.SoulReaver } },

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
            },

            //旋转钐割
            SpinningScythe = new (24376),

            //噩梦钐割
            NightmareScythe = new (24377),

            //束缚挥割
            GrimSwathe = new (24392),

            //暴食
            Gluttony = new (24393),

            //断首
            Guillotine = new (24384),

            //灵魂钐割
            SoulScythe = new (24381),

            //夜游魂衣 变身！
            Enshroud = new (24394),

            //团契
            Communio = new(24398)
            {
                OtherCheck = b =>
                {
                    if (level < Communio.Level) return false;
                    return true;
                },               
            },

            //夜游魂切割
            LemuresSlice = new (24399),

            //夜游魂钐割
            LemuresScythe = new (24400),

            //神秘纹 加盾
            ArcaneCrest = new (24404, true),

            //神秘环 加Buff
            ArcaneCircle = new (24405, true),

            //播魂种
            Soulsow = new (24387)
            {
                BuffsProvide = new [] {ObjectStatus.Soulsow},
            },

            //收获月
            HarvestMoon = new (24388)
            {
                BuffsNeed = new [] { ObjectStatus.Soulsow },
            },

            //地狱入境
            HellsIngress = new (24401),

            //大丰收
            PlentifulHarvest = new (24385);
    }
    internal override SortedList<DescType, string> Description => new ()
    {
        {DescType.单体防御, $"{Actions.ArcaneCrest.Action.Name}"},
        {DescType.移动, $"{Actions.HellsIngress.Action.Name}"},
    };
    private protected override bool GeneralGCD(uint lastComboActionID, out IAction act)
    {
        if(!TargetHelper.InBattle && Actions.Soulsow.ShouldUseAction(out act)) return true;

        //处于变身状态。
        if (StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.Enshrouded))
        {
            if (JobGauge.LemureShroud == 1 && JobGauge.VoidShroud == 0 && level >= Actions.Communio.Level)
            {
                if (!IsMoving && Actions.Communio.ShouldUseAction(out act, mustUse: true)) return true;
                //跑机制来不及读条？补个buff混一下
                else
                {
                    if (Actions.ShadowofDeath.ShouldUseAction(out act, mustUse: IsMoving)) return true;
                }
            }

            if (JobGauge.LemureShroud == 1 && JobGauge.VoidShroud == 0 && level < Actions.Communio.Level)
            { 
                if (Actions.Gallows.ShouldUseAction(out act)) return true; 
            }

            if (JobGauge.VoidShroud >= 2)
            {
                if (Actions.LemuresSlice.ShouldUseAction(out act)) return true;
                if (Actions.LemuresScythe.ShouldUseAction(out act)) return true;
            }

            if (JobGauge.LemureShroud > 1 && StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.EnhancedVoidReaping))
            {
                if (Actions.Gibbet.ShouldUseAction(out act)) return true;
            }
            if (JobGauge.LemureShroud > 1 && StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.EnhancedCrossReaping))
            {
                if (Actions.Gallows.ShouldUseAction(out act)) return true;
            }

            if (!StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.EnhancedVoidReaping) &&
                !StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.EnhancedCrossReaping) &&
                Actions.Gallows.ShouldUseAction(out act)) return true;

        }

        //处于补蓝状态，赶紧补蓝条。
        else if (StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.SoulReaver))
        {
            if (Actions.Guillotine.ShouldUseAction(out act)) return true;

            if (StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.EnhancedGibbet))
            {
                if (Actions.Gibbet.ShouldUseAction(out act)) return true;
            }
            else
            {
                if (Actions.Gallows.ShouldUseAction(out act)) return true;
            }
        }

        //上Debuff
        if (Actions.WhorlofDeath.ShouldUseAction(out act, lastComboActionID)) return true;
        if (Actions.ShadowofDeath.ShouldUseAction(out act, lastComboActionID)) return true;

        //大丰收喜提50灵魂
        if (JobGauge.Shroud <= 50 && !StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.CircleofSacrifice)
            && StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.ImmortalSacrifice) 
            && !StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.Enshrouded) &&
             Actions.PlentifulHarvest.ShouldUseAction(out act, mustUse: true)) return true;


        //获得灵魂 50.
        if (JobGauge.Soul <= 50 && !StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.Enshrouded))
        {
            if (Actions.SoulScythe.ShouldUseAction(out act, emptyOrSkipCombo: true)) return true;
            if (Actions.SoulSlice.ShouldUseAction(out act, emptyOrSkipCombo: true)) return true;
        }


        //群体二连
        if (!StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.Enshrouded) && 
            Actions.NightmareScythe.ShouldUseAction(out act, lastComboActionID)) return true;
        if (!StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.Enshrouded) && 
            Actions.SpinningScythe.ShouldUseAction(out act, lastComboActionID)) return true;


        //单体三连
        if (!StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.Enshrouded) &&
            Actions.InfernalSlice.ShouldUseAction(out act, lastComboActionID)) return true;
        if (!StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.Enshrouded) &&
            Actions.WaxingSlice.ShouldUseAction(out act, lastComboActionID)) return true;
        if (!StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.Enshrouded) && 
            Actions.Slice.ShouldUseAction(out act, lastComboActionID)) return true;

        //够不着了
        if (IconReplacer.Move && MoveAbility(1, out act)) return true;

        if (Actions.HarvestMoon.ShouldUseAction(out act, mustUse:true)) return true;
        if (Actions.Harpe.ShouldUseAction(out act)) return true;

        return false;
    }

    private protected override bool EmergercyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        return base.EmergercyAbility(abilityRemain, nextGCD, out act);
    }

    private protected override bool ForAttachAbility(byte abilityRemain, out IAction act)
    {


        if (!StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.SoulReaver))
        {
            //蓝条够了，变身！
            if (JobGauge.Shroud >= 50 && Actions.Enshroud.ShouldUseAction(out act)) return true;

            //灵魂够了，拿蓝条状态。
            if (JobGauge.Soul >= 50)
            {
                if (Actions.Gluttony.ShouldUseAction(out act, mustUse: true)) return true;
                if (Actions.GrimSwathe.ShouldUseAction(out act)) return true;
                if (Actions.BloodStalk.ShouldUseAction(out act)) return true;
            }
        }

        act = null;
        return false;
    }

    private protected override bool BreakAbility(byte abilityRemain, out IAction act)
    {
        //究极团辅
        if (Actions.ArcaneCircle.ShouldUseAction(out act)) return true;
        return false;
    }

    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        //牵制
        if (GeneralActions.Feint.ShouldUseAction(out act)) return true;
        return false;
    }

    private protected override bool MoveAbility(byte abilityRemain, out IAction act)
    {
        //地狱入境
        if (Actions.HellsIngress.ShouldUseAction(out act) && !StatusHelper.HaveStatusSelfFromSelf(ObjectStatus.Threshold)) return true;
        return false;
    }

    private protected override bool DefenceSingleAbility(byte abilityRemain, out IAction act)
    {
        //神秘纹
        if (Actions.ArcaneCrest.ShouldUseAction(out act)) return true;
        return false;
    }


}
