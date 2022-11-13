using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;

namespace XIVAutoAttack.Combos.Basic;

//[ComboDevInfo(@"https://github.com/ArchiDog1998/XIVAutoAttack/blob/main/XIVAutoAttack/Combos/Melee/SAMCombo.cs",
//   ComboAuthor.fatinghenji)]
internal abstract class SAMCombo_Base<TCmd> : JobGaugeCombo<SAMGauge, TCmd> where TCmd : Enum
{
    public sealed override ClassJobID[] JobIDs => new ClassJobID[] { ClassJobID.Samurai };

    protected static byte SenCount => (byte)((JobGauge.HasGetsu ? 1 : 0) + (JobGauge.HasSetsu ? 1 : 0) + (JobGauge.HasKa ? 1 : 0));

    protected static bool HaveMoon => Player.HaveStatus(true, StatusID.Moon);
    protected static bool HaveFlower => Player.HaveStatus(true, StatusID.Flower);


    public static readonly BaseAction
        //刃风
        Hakaze = new(7477),

        //阵风
        Jinpu = new(7478),

        //心眼
        ThirdEye = new(7498),

        //燕飞
        Enpi = new(7486),

        //士风
        Shifu = new(7479),

        //风雅
        Fuga = new(7483),

        //月光
        Gekko = new(ActionIDs.Gekko),

        //彼岸花
        Higanbana = new(7489, isEot: true)
        {
            OtherCheck = b => !IsMoving && SenCount == 1 && HaveMoon && HaveFlower,
            TargetStatus = new[] { StatusID.Higanbana },
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

        //满月
        Mangetsu = new(7484),

        //花车
        Kasha = new(ActionIDs.Kasha),

        //樱花
        Oka = new(7485),

        //明镜止水
        MeikyoShisui = new(7499)
        {
            BuffsProvide = new[] { StatusID.MeikyoShisui },
            OtherCheck = b => JobGauge.HasSetsu && !JobGauge.HasKa && !JobGauge.HasGetsu,
        },

        //雪风
        Yukikaze = new(7480),

        //必杀剑·晓天
        HissatsuGyoten = new(7492),

        //必杀剑·震天
        HissatsuShinten = new(7490),

        //必杀剑·九天
        HissatsuKyuten = new(7491),

        //意气冲天
        Ikishoten = new(16482),

        //必杀剑·红莲
        HissatsuGuren = new(7496),

        //必杀剑·闪影
        HissatsuSenei = new(16481),

        //回返五剑
        KaeshiGoken = new(16485),

        //回返雪月花
        KaeshiSetsugekka = new(16486),

        //照破
        Shoha = new(16487),

        //无明照破
        Shoha2 = new(25779),

        //奥义斩浪
        OgiNamikiri = new(25781)
        {
            OtherCheck = b => HaveFlower && HaveMoon,
            BuffsNeed = new[] { StatusID.OgiNamikiriReady },
        },

        //回返斩浪
        KaeshiNamikiri = new(25782);

}