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

namespace XIVAutoAttack.Combos.Tank;
internal abstract class DRKCombo<TCmd> : JobGaugeCombo<DRKGauge, TCmd> where TCmd : Enum
{ 
    public sealed override uint[] JobIDs => new uint[] { 32 };
    internal sealed override bool HaveShield => Player.HaveStatus(StatusIDs.Grit);
    private protected override BaseAction Shield => Grit;


    public static readonly BaseAction
        //重斩
        HardSlash = new(3617),

        //吸收斩
        SyphonStrike = new(3623),

        //释放
        Unleash = new(3621),

        //深恶痛绝
        Grit = new(3629, shouldEndSpecial: true),

        //伤残
        Unmend = new(3624)
        {
            FilterForTarget = b => TargetFilter.ProvokeTarget(b),
        },

        //噬魂斩
        Souleater = new(3632),

        //暗黑波动
        FloodofDarkness = new(16466),

        //暗黑锋
        EdgeofDarkness = new(16467)
        {
            OtherCheck = b => !IsLastAction(true, EdgeofDarkness, FloodofDarkness) && Player.CurrentMp >= 3000,
        },

        //嗜血
        BloodWeapon = new(3625)
        {
            OtherCheck = b => JobGauge.DarksideTimeRemaining > 0,
        },

        //暗影墙
        ShadowWall = new(3636)
        {
            BuffsProvide = new[] { StatusIDs.ShadowWall },
            OtherCheck = BaseAction.TankDefenseSelf,
        },

        //弃明投暗
        DarkMind = new(3634)
        {
            OtherCheck = BaseAction.TankDefenseSelf,
        },

        //行尸走肉
        LivingDead = new(3638)
        {
            OtherCheck = BaseAction.TankBreakOtherCheck,
        },

        //腐秽大地
        SaltedEarth = new(3639),

        //跳斩
        Plunge = new(3640, shouldEndSpecial: true)
        {
            ChoiceTarget = TargetFilter.FindTargetForMoving
        },

        //吸血深渊
        AbyssalDrain = new(3641),

        //精雕怒斩
        CarveandSpit = new(3643),

        //血溅
        Bloodspiller = new(7392)
        {
            OtherCheck = b => JobGauge.Blood >= 50 || Player.HaveStatus(StatusIDs.Delirium),
        },

        //寂灭
        Quietus = new(7391),

        //血乱
        Delirium = new(7390)
        {
            OtherCheck = b => JobGauge.DarksideTimeRemaining > 0,
        },

        //至黑之夜
        TheBlackestNight = new(7393)
        {
            ChoiceTarget = TargetFilter.FindAttackedTarget,
        },

        //刚魂
        StalwartSoul = new(16468),

        //暗黑布道
        DarkMissionary = new(16471, true),

        //掠影示现
        LivingShadow = new(16472)
        {
            OtherCheck = b => JobGauge.Blood >= 50 && JobGauge.DarksideTimeRemaining > 1,
        },

        //献奉
        Oblation = new(25754, true)
        {
            ChoiceTarget = TargetFilter.FindAttackedTarget,
        },

        //暗影使者
        Shadowbringer = new(25757)
        {
            OtherCheck = b => JobGauge.DarksideTimeRemaining > 1 && !IsLastAction(true, Shadowbringer),
        },

        //腐秽黑暗
        SaltandDarkness = new(25755)
        {
            BuffsNeed = new[] { StatusIDs.SaltedEarth },
        };
}
