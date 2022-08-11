using Dalamud.Game.ClientState.JobGauge.Types;
using System.Collections.Generic;

namespace XIVAutoAttack.Combos.Melee;

internal class SAMCombo : CustomComboJob<SAMGauge>
{
    internal override uint JobID => 34;
    //private static bool _shouldUseGoken = false;
    //private static bool _shouldUseSetsugekka = false;
    //private static bool _shouldUseOgiNamikiri = false;
    protected override bool ShouldSayout => true;

    private static byte SenCount => (byte)((JobGauge.HasGetsu ? 1 : 0) + (JobGauge.HasSetsu ? 1 : 0) + (JobGauge.HasKa ? 1 : 0));
    internal struct Actions
    {
        public static readonly BaseAction
            //刃风
            Hakaze = new BaseAction(7477),

            //阵风
            Jinpu = new BaseAction(7478),

            //心眼
            ThirdEye = new BaseAction(7498),

            //燕飞
            Enpi = new BaseAction(7486),

            //士风
            Shifu = new BaseAction(7479),

            //风雅
            Fuga = new BaseAction(7483),

            //月光
            Gekko = new BaseAction(7481)
            {
                EnermyLocation = EnemyLocation.Back,
            },

            //彼岸花
            Higanbana = new BaseAction(7489)
            {
                OtherCheck = b => !TargetHelper.IsMoving,
                TargetStatus = new ushort[] { ObjectStatus.Higanbana },
            },

            //天下五剑
            TenkaGoken = new BaseAction(7488)
            {
                OtherCheck = b => !TargetHelper.IsMoving,
            },

            //纷乱雪月花
            MidareSetsugekka = new BaseAction(7487)
            {
                OtherCheck = b => !TargetHelper.IsMoving,
            },

            //满月
            Mangetsu = new BaseAction(7484),

            //花车
            Kasha = new BaseAction(7482)
            {
                EnermyLocation = EnemyLocation.Side,
            },

            //樱花
            Oka = new BaseAction(7485),

            //明镜止水
            MeikyoShisui = new BaseAction(7499)
            {
                BuffsProvide = new ushort[] { ObjectStatus.MeikyoShisui },
            },

            //雪风
            Yukikaze = new BaseAction(7480),

            ////必杀剑·回天
            //HissatsuKaiten = new BaseAction(7494),

            //必杀剑·晓天
            HissatsuGyoten = new BaseAction(7492),

            //必杀剑·震天
            HissatsuShinten = new BaseAction(7490),

            //必杀剑·九天
            HissatsuKyuten = new BaseAction(7491),

            //意气冲天
            Ikishoten = new BaseAction(16482),

            //必杀剑·红莲
            HissatsuGuren = new BaseAction(7496),

            //必杀剑·闪影
            HissatsuSenei = new BaseAction(16481),

            //回返五剑
            KaeshiGoken = new BaseAction(16485),

            //回返雪月花
            KaeshiSetsugekka = new BaseAction(16486),

            //照破
            Shoha = new BaseAction(16487),

            //无明照破
            Shoha2 = new BaseAction(25779),

            //奥义斩浪
            OgiNamikiri = new BaseAction(25781)
            {
                BuffsNeed = new ushort[] { ObjectStatus.OgiNamikiriReady },
            },

            //回返斩浪
            KaeshiNamikiri = new BaseAction(25782);
    }
    internal override SortedList<DescType, string> Description => new SortedList<DescType, string>()
    {
        {DescType.单体防御, $"{Actions.ThirdEye.Action.Name}"},
        {DescType.移动, $"{Actions.HissatsuGyoten.Action.Name}"},
    };
    private protected override bool GeneralGCD(uint lastComboActionID, out IAction act)
    {
        //赶紧回返！
        if (Service.IconReplacer.OriginalHook(Actions.OgiNamikiri.ID) == Actions.KaeshiNamikiri.ID)
        {
            if (Actions.KaeshiNamikiri.ShouldUseAction(out act, mustUse: true)) return true;
        }
        if (Service.IconReplacer.OriginalHook(16483) == Actions.KaeshiGoken.ID)
        {
            if (Actions.KaeshiGoken.ShouldUseAction(out act, mustUse: true)) return true;
        }
        if (Service.IconReplacer.OriginalHook(16483) == Actions.KaeshiSetsugekka.ID)
        {
            if (Actions.KaeshiSetsugekka.ShouldUseAction(out act, mustUse: true)) return true;
        }

        if (Actions.OgiNamikiri.ShouldUseAction(out act, mustUse: true)) return true;
        if (Actions.TenkaGoken.ShouldUseAction(out act))
        {
            if (SenCount > 1) return true;
        }
        else if (SenCount > 0)
        {
            if (SenCount == 3 && Actions.MidareSetsugekka.ShouldUseAction(out act)) return true;
            if (Actions.Higanbana.ShouldUseAction(out act)) return true;
        }


        //123
        bool haveMeikyoShisui = BaseAction.HaveStatusSelfFromSelf(ObjectStatus.MeikyoShisui);
        //如果是单体，且明镜止水的冷却时间小于3秒。
        if (!JobGauge.HasSetsu && !Actions.Fuga.ShouldUseAction(out _) && Actions.MeikyoShisui.RecastTimeRemain < 3)
        {
            if (Actions.Yukikaze.ShouldUseAction(out act, lastComboActionID)) return true;
        }
        if (!BaseAction.HaveStatusSelfFromSelf(ObjectStatus.Moon))
        {
            if (Actions.Mangetsu.ShouldUseAction(out act, lastComboActionID, emptyOrSkipCombo: haveMeikyoShisui)) return true;
            if (Actions.Gekko.ShouldUseAction(out act, lastComboActionID, emptyOrSkipCombo: haveMeikyoShisui)) return true;
            if (Actions.Jinpu.ShouldUseAction(out act, lastComboActionID)) return true;
        }
        if (!BaseAction.HaveStatusSelfFromSelf(ObjectStatus.Flower))
        {
            if (Actions.Oka.ShouldUseAction(out act, lastComboActionID, emptyOrSkipCombo: haveMeikyoShisui)) return true;
            if (Actions.Kasha.ShouldUseAction(out act, lastComboActionID, emptyOrSkipCombo: haveMeikyoShisui)) return true;
            if (Actions.Shifu.ShouldUseAction(out act, lastComboActionID)) return true;
        }
        if (!JobGauge.HasGetsu)
        {
            if (Actions.Mangetsu.ShouldUseAction(out act, lastComboActionID, emptyOrSkipCombo: haveMeikyoShisui)) return true;
            if (Actions.Gekko.ShouldUseAction(out act, lastComboActionID, emptyOrSkipCombo: haveMeikyoShisui)) return true;
            if (Actions.Jinpu.ShouldUseAction(out act, lastComboActionID)) return true;
        }
        if (!JobGauge.HasKa)
        {
            if (Actions.Oka.ShouldUseAction(out act, lastComboActionID, emptyOrSkipCombo: haveMeikyoShisui)) return true;
            if (Actions.Kasha.ShouldUseAction(out act, lastComboActionID, emptyOrSkipCombo: haveMeikyoShisui)) return true;
            if (Actions.Shifu.ShouldUseAction(out act, lastComboActionID)) return true;
        }
        if (!JobGauge.HasSetsu)
        {
            if (Actions.Yukikaze.ShouldUseAction(out act, lastComboActionID, emptyOrSkipCombo: haveMeikyoShisui)) return true;
        }
        if (Actions.Fuga.ShouldUseAction(out act, lastComboActionID)) return true;
        if (Actions.Hakaze.ShouldUseAction(out act, lastComboActionID)) return true;



        if (IconReplacer.Move && MoveAbility(1, out act)) return true;
        if (Actions.Enpi.ShouldUseAction(out act)) return true;

        return false;
    }

    private protected override bool MoveAbility(byte abilityRemain, out IAction act)
    {
        if (JobGauge.Kenki >= 30 && Actions.HissatsuGyoten.ShouldUseAction(out act)) return true;
        act = null;
        return false;
    }

    private protected override bool ForAttachAbility(byte abilityRemain, out IAction act)
    {
        if (JobGauge.MeditationStacks == 3)
        {
            if (Actions.Shoha2.ShouldUseAction(out act)) return true;
            if (Actions.Shoha.ShouldUseAction(out act)) return true;
        }

        if (JobGauge.Kenki >= 25)
        {
            if (Actions.HissatsuGuren.ShouldUseAction(out act)) return true;
            if (Actions.HissatsuKyuten.ShouldUseAction(out act)) return true;

            if (Actions.HissatsuSenei.ShouldUseAction(out act)) return true;
            if (Actions.HissatsuShinten.ShouldUseAction(out act)) return true;
        }

        if (TargetHelper.InBattle && Actions.Ikishoten.ShouldUseAction(out act)) return true;

        act = null;
        return false;
    }

    private protected override bool EmergercyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        if (HaveTargetAngle && Actions.MeikyoShisui.ShouldUseAction(out act, emptyOrSkipCombo: true)) return true;

        //if (nextGCD.ID == Actions.TenkaGoken.ID || nextGCD.ID == Actions.Higanbana.ID || nextGCD.ID == Actions.MidareSetsugekka.ID || nextGCD.ID == Actions.OgiNamikiri.ID)
        //{
        //    if (JobGauge.Kenki >= 20 && !IsMoving && Actions.HissatsuKaiten.ShouldUseAction(out act)) return true;
        //}

        act = null;
        return false;
    }

    private protected override bool DefenceSingleAbility(byte abilityRemain, out IAction act)
    {
        if (Actions.ThirdEye.ShouldUseAction(out act)) return true;
        return false;
    }

    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        //牵制
        if (GeneralActions.Feint.ShouldUseAction(out act)) return true;
        return false;
    }
}
