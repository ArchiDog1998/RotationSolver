using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using System.Collections.Generic;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.CustomCombo;
using XIVAutoAttack.Configuration;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Updaters;

namespace XIVAutoAttack.Combos.Healer;

//[ComboDevInfo(@"https://github.com/ArchiDog1998/XIVAutoAttack/blob/main/XIVAutoAttack/Combos/Healer/WHMCombo.cs",
//   ComboAuthor.NiGuangOwO)]
public abstract class WHMCombo<TCmd> : JobGaugeCombo<WHMGauge, TCmd> where TCmd : Enum
{
    public sealed override uint[] JobIDs => new uint[] { 24, 6 };
    private protected override BaseAction Raise => Raise1;

    public static readonly BaseAction
    #region 治疗
        //治疗
        Cure = new(120, true),

        //医治
        Medica = new(124, true),

        //复活
        Raise1 = new(125, true),

        //救疗
        Cure2 = new(135, true),

        //医济
        Medica2 = new(133, true)
        {
            BuffsProvide = new[] { ObjectStatus.Medica2, ObjectStatus.TrueMedica2 },
            OtherCheck = b => !IsLastSpell(true, Medica2)
        },

        //再生
        Regen = new(137, true)
        {
            TargetStatus = new[]
            {
                    ObjectStatus.Regen1,
                    ObjectStatus.Regen2,
                    ObjectStatus.Regen3,
            }
        },

        //愈疗
        Cure3 = new(131, true),

        //天赐祝福
        Benediction = new(140, true)
        {
            OtherCheck = b => TargetUpdater.PartyMembersMinHP < 0.15f,
        },

        //庇护所
        Asylum = new(3569, true)
        {
            OtherCheck = b => !IsMoving
        },

        //安慰之心
        AfflatusSolace = new(16531, true)
        {
            OtherCheck = b => JobGauge.Lily > 0,
        },

        //神名
        Tetragrammaton = new(3570, true),

        //神祝祷
        DivineBenison = new(7432, true),

        //狂喜之心
        AfflatusRapture = new(16534, true)
        {
            OtherCheck = b => JobGauge.Lily > 0,
        },

        //水流幕
        Aquaveil = new(25861, true),

        //礼仪之铃
        LiturgyoftheBell = new(25862, true),
    #endregion
    #region 输出
        //飞石 
        Stone = new(119),//坚石127 垒石3568 崩石7431 闪耀16533 闪灼25859

        //疾风 Dot
        Aero = new(121, isDot: true)//烈风132 天辉16532
        {
            TargetStatus = new ushort[]
            {
                    ObjectStatus.Aero,
                    ObjectStatus.Aero2,
                    ObjectStatus.Dia,
            }
        },

        //神圣
        Holy = new(139),//豪圣 25860

        //法令
        Assize = new(3571, true),

        //苦难之心
        AfflatusMisery = new(16535)
        {
            OtherCheck = b => JobGauge.BloodLily == 3,
        },
    #endregion
    #region buff
        //神速咏唱
        PresenseOfMind = new(136, true)
        {
            OtherCheck = b => !IsMoving
        },

        //无中生有
        ThinAir = new(7430, true),

        //全大赦
        PlenaryIndulgence = new(7433, true),

        //节制
        Temperance = new(16536, true);
    #endregion

}