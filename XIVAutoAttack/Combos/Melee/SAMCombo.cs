using Dalamud.Game.ClientState.JobGauge.Types;
using Lumina.Data.Parsing.Layer;
using System.Collections.Generic;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Controllers;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;

namespace XIVAutoAttack.Combos.Melee;

internal sealed class SAMCombo : JobGaugeCombo<SAMGauge>
{
    internal override uint JobID => 34;

    private static byte SenCount => (byte)((JobGauge.HasGetsu ? 1 : 0) + (JobGauge.HasSetsu ? 1 : 0) + (JobGauge.HasKa ? 1 : 0));

    internal struct Actions
    {
        public static readonly BaseAction
        #region 单体GCD
            //刃风
            Hakaze = new(7477),

            //阵风
            Jinpu = new(7478),

            //士风
            Shifu = new(7479),

            //月光
            Gekko = new(7481),

            //花车
            Kasha = new(7482),

            //雪风
            Yukikaze = new(7480),

            //燕飞
            Enpi = new(7486),
        #endregion
        #region 群体GCD

            //风雅
            Fuga = new(7483),

            //满月
            Mangetsu = new(7484),

            //樱花
            Oka = new(7485),

            //风光
            Fuko = new(2578),

        #endregion
        #region 剑气能力技

            //必杀剑·震天
            HissatsuShinten = new(7490),

            //必杀剑·晓天
            HissatsuGyoten = new(7492),

            //必杀剑·夜天
            HissatsuYaten = new(7943),

            //必杀剑·九天
            HissatsuKyuten = new(7491),

            //必杀剑·红莲
            HissatsuGuren = new(7496),

            //必杀剑·闪影
            HissatsuSenei = new(16481),
        #endregion
        #region 居合术

            //彼岸花
            Higanbana = new(7489, isDot: true)
            {
                OtherCheck = b => !IsMoving && SenCount == 1,
                TargetStatus = new[] { ObjectStatus.Higanbana },
            },

            //天下五剑
            TenkaGoken = new(7488)
            {
                OtherCheck = b => !IsMoving,
            },

            //纷乱雪月花
            MidareSetsugekka = new(7487)
            {
                OtherCheck = b => !IsMoving && SenCount == 3,
            },

            //回返彼岸花
            KaeshiHiganbana = new(17741, isDot: true),

            //回返五剑
            KaeshiGoken = new(16485),

            //回返雪月花
            KaeshiSetsugekka = new(16486),
        #endregion
        #region 其他伤害能力技

            //照破
            Shoha = new(16487),

            //无明照破
            Shoha2 = new(25779),

            //奥义斩浪
            OgiNamikiri = new(25781)
            {
                BuffsNeed = new[] { ObjectStatus.OgiNamikiriReady },
            },

            //回返斩浪
            KaeshiNamikiri = new(25782),
        #endregion
        #region 其他能力技

            //心眼
            ThirdEye = new(7498),

            //明镜止水
            MeikyoShisui = new(7499)
            {
                BuffsProvide = new[] { ObjectStatus.MeikyoShisui },
                OtherCheck = b => JobGauge.HasSetsu && !JobGauge.HasKa && !JobGauge.HasGetsu,
            },

            //叶隐
            Hagakure = new(7495)
            {
                OtherCheck = b => SenCount == 1,
            },

            //意气冲天
            Ikishoten = new(16482),

            //燕回返
            Tsubamegaeshi = new(16483),

            //真北
            TrueNorth = new(7546);
        #endregion
    }
    internal override SortedList<DescType, string> Description => new()
    {
        {DescType.单体防御, $"{Actions.ThirdEye.Action.Name}"},
        {DescType.移动技能, $"{Actions.HissatsuGyoten.Action.Name}"},
    };

    private protected override bool GeneralGCD(uint lastComboActionID, out IAction act)
    {
        //回返斩浪
        if (Service.IconReplacer.OriginalHook(Actions.OgiNamikiri.ID) == Actions.KaeshiNamikiri.ID)
        {
            if (Actions.KaeshiNamikiri.ShouldUse(out act, mustUse: true)) return true;
        }
        //回返五剑
        if (Service.IconReplacer.OriginalHook(Actions.Tsubamegaeshi.ID) == Actions.KaeshiGoken.ID)
        {
            if (Actions.KaeshiGoken.ShouldUse(out act, mustUse: true)) return true;
        }
        //回返雪月花
        if (Service.IconReplacer.OriginalHook(Actions.Tsubamegaeshi.ID) == Actions.KaeshiSetsugekka.ID)
        {
            if (Actions.KaeshiSetsugekka.ShouldUse(out act, mustUse: true)) return true;
        }
        if (Actions.OgiNamikiri.ShouldUse(out act, mustUse: true)) return true;
        //天下五剑
        if (Actions.TenkaGoken.ShouldUse(out act))
        {
            if (SenCount == 2) return true;
        }
        else
        {
            //雪！月！花！
            if (Actions.MidareSetsugekka.ShouldUse(out act)) return true;
            //彼岸花在明镜止水中打出，同时应该消耗掉一个月闪
            if (Player.HaveStatus(ObjectStatus.MeikyoShisui) && Player.HaveStatus(ObjectStatus.Moon) && Actions.Higanbana.ShouldUse(out act)) return true;
        }
        bool haveMeikyoShisui = Player.HaveStatus(ObjectStatus.MeikyoShisui);
        if (!JobGauge.HasSetsu && !Player.HaveStatus(ObjectStatus.Moon))
        {
            if (GetsuGCD(out act, lastComboActionID, haveMeikyoShisui)) return true;
        }
        if (!Player.HaveStatus(ObjectStatus.Flower))
        {
            if (KaGCD(out act, lastComboActionID, haveMeikyoShisui)) return true;
        }
        if (!JobGauge.HasSetsu)//确定是单体且没有雪闪
        {
            if (Actions.Yukikaze.ShouldUse(out act, lastComboActionID)) return true;//打雪风
        }
        if (!JobGauge.HasGetsu) //月
        {
            if (GetsuGCD(out act, lastComboActionID, haveMeikyoShisui)) return true;
        }
        if (!JobGauge.HasKa) //花
        {
            if (KaGCD(out act, lastComboActionID, haveMeikyoShisui)) return true;
        }
        if (!JobGauge.HasSetsu) //雪
        {
            if (Actions.Yukikaze.ShouldUse(out act, lastComboActionID, emptyOrSkipCombo: haveMeikyoShisui)) return true;
        }

        //来个月？
        if (GetsuGCD(out act, lastComboActionID, haveMeikyoShisui)) return true;
        if (Actions.Fuga.ShouldUse(out act, lastComboActionID)) return true;
        if (Actions.Hakaze.ShouldUse(out act, lastComboActionID)) return true;


        if (CommandController.Move && MoveAbility(1, out act)) return true;
        if (Actions.Enpi.ShouldUse(out act)) return true;

        return false;
    }
    private bool GetsuGCD(out IAction act, uint lastComboActionID, bool haveMeikyoShisui)
    {
        if (Actions.Mangetsu.ShouldUse(out act, lastComboActionID, emptyOrSkipCombo: haveMeikyoShisui)) return true;
        if (Actions.Gekko.ShouldUse(out act, lastComboActionID, emptyOrSkipCombo: haveMeikyoShisui)) return true;
        if (Actions.Jinpu.ShouldUse(out act, lastComboActionID)) return true;

        act = null;
        return false;
    }

    private bool KaGCD(out IAction act, uint lastComboActionID, bool haveMeikyoShisui)
    {
        if (Actions.Oka.ShouldUse(out act, lastComboActionID, emptyOrSkipCombo: haveMeikyoShisui)) return true;
        if (Actions.Kasha.ShouldUse(out act, lastComboActionID, emptyOrSkipCombo: haveMeikyoShisui)) return true;
        if (Actions.Shifu.ShouldUse(out act, lastComboActionID)) return true;

        act = null;
        return false;
    }

    private protected override bool MoveAbility(byte abilityRemain, out IAction act)
    {
        if (JobGauge.Kenki >= 30 && Actions.HissatsuGyoten.ShouldUse(out act)) return true;
        act = null;
        return false;
    }

    private protected override bool AttackAbility(byte abilityRemain, out IAction act)
    {
        //有三档剑压了，那就打奥义斩浪
        if (JobGauge.MeditationStacks == 3)
        {
            if (Actions.Shoha2.ShouldUse(out act)) return true;
            if (Actions.Shoha.ShouldUse(out act)) return true;
        }

        if (JobGauge.Kenki >= 25)
        {
            if (Actions.HissatsuGuren.ShouldUse(out act)) return true;
            if (Actions.HissatsuKyuten.ShouldUse(out act)) return true;

            if (Actions.HissatsuSenei.ShouldUse(out act)) return true;
            if (Actions.HissatsuShinten.ShouldUse(out act)) return true;
        }

        if (InCombat && Actions.Ikishoten.ShouldUse(out act)) return true;

        act = null;
        return false;
    }

    private protected override bool EmergercyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        if (HaveHostileInRange &&
            !nextGCD.IsAnySameAction(false, Actions.Higanbana, Actions.OgiNamikiri, Actions.KaeshiNamikiri) &&
            Actions.MeikyoShisui.ShouldUse(out act, emptyOrSkipCombo: true)) return true;

        return base.EmergercyAbility(abilityRemain, nextGCD, out act);
    }

    private protected override bool DefenceSingleAbility(byte abilityRemain, out IAction act)
    {
        if (Actions.ThirdEye.ShouldUse(out act)) return true;
        return false;
    }

    private protected override bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        //牵制
        if (GeneralActions.Feint.ShouldUse(out act)) return true;
        return false;
    }
}
