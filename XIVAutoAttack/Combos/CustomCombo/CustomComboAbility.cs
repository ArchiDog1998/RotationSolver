using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using FFXIVClientStructs.FFXIV.Client.UI;
using ImGuiScene;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Combos.RangedPhysicial;
using XIVAutoAttack.Configuration;

namespace XIVAutoAttack.Combos.CustomCombo;

public abstract partial class CustomCombo
{
    private bool Ability(byte abilityRemain, IAction nextGCD, out IAction act, bool helpDefenseAOE, bool helpDefenseSingle)
    {
        if (Service.Configuration.OnlyGCD)
        {
            act = null;
            return false;
        }

        //有某些非常危险的状态。
        if (JobID == 23)
        {
            if (IconReplacer.EsunaOrShield && TargetHelper.WeakenPeople.Length > 0 || TargetHelper.DyingPeople.Length > 0)
            {
                if (BRDCombo.Actions.WardensPaean.ShouldUseAction(out act, mustUse: true)) return true;
            }
        }


        if (EmergercyAbility(abilityRemain, nextGCD, out act)) return true;
        Role role = (Role)XIVAutoAttackPlugin.AllJobs.First(job => job.RowId == JobID).Role;

        if (TargetHelper.CanInterruptTargets.Length > 0)
        {
            switch (role)
            {
                case Role.防护:
                    if (GeneralActions.Interject.ShouldUseAction(out act)) return true;
                    if (GeneralActions.LowBlow.ShouldUseAction(out act)) return true;
                    break;

                case Role.近战:
                    if (GeneralActions.LegSweep.ShouldUseAction(out act)) return true;
                    break;
                case Role.远程:
                    if (RangePhysicial.Contains(Service.ClientState.LocalPlayer.ClassJob.Id))
                    {
                        if (GeneralActions.HeadGraze.ShouldUseAction(out act)) return true;
                    }
                    break;
            }
        }
        if (role == Role.防护)
        {
            if (IconReplacer.RaiseOrShirk)
            {
                if (GeneralActions.Shirk.ShouldUseAction(out act)) return true;
                if (HaveShield && Shield.ShouldUseAction(out act)) return true;
            }

            if (IconReplacer.EsunaOrShield && Shield.ShouldUseAction(out act)) return true;

            var defenses = new uint[] { ObjectStatus.Grit, ObjectStatus.RoyalGuard, ObjectStatus.IronWill, ObjectStatus.Defiance };
            //Alive Tanks with shield.
            var defensesTanks = TargetHelper.AllianceTanks.Where(t => t.CurrentHp != 0 && t.StatusList.Select(s => s.StatusId).Intersect(defenses).Count() > 0);
            if (defensesTanks == null || defensesTanks.Count() == 0)
            {
                if (!HaveShield && Shield.ShouldUseAction(out act)) return true;
            }
        }

        if (IconReplacer.AntiRepulsion)
        {
            switch (role)
            {
                case Role.防护:
                case Role.近战:
                    if (GeneralActions.ArmsLength.ShouldUseAction(out act)) return true;
                    break;
                case Role.治疗:
                    if (GeneralActions.Surecast.ShouldUseAction(out act)) return true;
                    break;
                case Role.远程:
                    if (RangePhysicial.Contains(Service.ClientState.LocalPlayer.ClassJob.Id))
                    {
                        if (GeneralActions.ArmsLength.ShouldUseAction(out act)) return true;
                    }
                    else
                    {
                        if (GeneralActions.Surecast.ShouldUseAction(out act)) return true;
                    }
                    break;
            }
        }
        if (IconReplacer.EsunaOrShield && role == Role.近战)
        {
            if (GeneralActions.TrueNorth.ShouldUseAction(out act)) return true;
        }


        if (HaveTargetAngle && SettingBreak && BreakAbility(abilityRemain, out act)) return true;
        if (IconReplacer.DefenseArea && DefenceAreaAbility(abilityRemain, out act)) return true;
        if (IconReplacer.DefenseSingle && DefenceSingleAbility(abilityRemain, out act)) return true;
        if (TargetHelper.HPNotFull || Service.ClientState.LocalPlayer.ClassJob.Id == 25)
        {
            if ((IconReplacer.HealArea || CanHealAreaAbility) && HealAreaAbility(abilityRemain, out act)) return true;
            if ((IconReplacer.HealSingle || CanHealSingleAbility) && HealSingleAbility(abilityRemain, out act)) return true;
        }
        if (IconReplacer.Move && MoveAbility(abilityRemain, out act)) return true;

        //防御
        if (HaveTargetAngle)
        {
            //防AOE
            if (helpDefenseAOE)
            {
                if (DefenceAreaAbility(abilityRemain, out act)) return true;
                if (role == Role.近战 || role == Role.远程)
                {
                    //防卫
                    if (DefenceSingleAbility(abilityRemain, out act)) return true;
                }
            }

            //防单体
            if (role == Role.防护)
            {
                var haveTargets = TargetFilter.ProvokeTarget(TargetHelper.HostileTargets);
                if ((Service.Configuration.AutoProvokeForTank || TargetHelper.AllianceTanks.Length < 2) && haveTargets != TargetHelper.HostileTargets
                    || IconReplacer.BreakorProvoke)

                {
                    //开盾挑衅
                    if (!HaveShield && Shield.ShouldUseAction(out act)) return true;
                    if (GeneralActions.Provoke.ShouldUseAction(out act, mustUse: true)) return true;
                }

                if (Service.Configuration.AutoDefenseForTank && HaveShield)
                {
                    //被群殴呢
                    if (TargetHelper.TarOnMeTargets.Length > 1 && !IsMoving)
                    {
                        if (GeneralActions.ArmsLength.ShouldUseAction(out act)) return true;
                        if (DefenceSingleAbility(abilityRemain, out act)) return true;
                    }

                    //就一个打我，需要正在对我搞事情。
                    if (TargetHelper.TarOnMeTargets.Length == 1)
                    {
                        var tar = TargetHelper.TarOnMeTargets[0];
                        if (TargetHelper.IsHostileTank)
                        {
                            //防卫
                            if (DefenceSingleAbility(abilityRemain, out act)) return true;
                        }
                    }
                }
            }

            //辅助防卫
            if (helpDefenseSingle && DefenceSingleAbility(abilityRemain, out act)) return true;
        }

        //恢复
        if (role == Role.近战)
        {
            if (GeneralActions.SecondWind.ShouldUseAction(out act)) return true;
            if (GeneralActions.Bloodbath.ShouldUseAction(out act)) return true;
        }
        else if (role == Role.远程)
        {
            if (RangePhysicial.Contains(Service.ClientState.LocalPlayer.ClassJob.Id))
            {
                if (GeneralActions.SecondWind.ShouldUseAction(out act)) return true;
            }
            else
            {
                if (Service.ClientState.LocalPlayer.ClassJob.Id != 25 && GeneralActions.LucidDreaming.ShouldUseAction(out act)) return true;
            }
        }
        else if (role == Role.治疗)
        {
            if (GeneralActions.LucidDreaming.ShouldUseAction(out act)) return true;
        }

        if (GeneralAbility(abilityRemain, out act)) return true;
        if (HaveTargetAngle && ForAttachAbility(abilityRemain, out act)) return true;
        return false;
    }

    /// <summary>
    /// 覆盖写一些用于攻击的能力技，只有附近有敌人的时候才会有效。
    /// </summary>
    /// <param name="level"></param>
    /// <param name="abilityRemain"></param>
    /// <param name="act"></param>
    /// <returns></returns>
    private protected abstract bool ForAttachAbility(byte abilityRemain, out IAction act);
    /// <summary>
    /// 覆盖写一些用于因为后面的GCD技能而要适应的能力技能
    /// </summary>
    /// <param name="level"></param>
    /// <param name="abilityRemain"></param>
    /// <param name="nextGCD"></param>
    /// <param name="act"></param>
    /// <returns></returns>
    private protected virtual bool EmergercyAbility(byte abilityRemain, IAction nextGCD, out IAction act)
    {
        if (nextGCD is BaseAction action)
        {
            if (action.Cast100 >= 50 && GeneralActions.Swiftcast.ShouldUseAction(out act, mustUse: true)) return true;

            if (abilityRemain == 1 && action.EnermyLocation != EnemyLocation.None && action.Target != null)
            {
                if (action.EnermyLocation != FindEnemyLocation(action.Target))
                {
                    if (GeneralActions.TrueNorth.ShouldUseAction(out act, emptyOrSkipCombo: true)) return true;
                }
            }
        }

        act = null;
        return false;
    }

    internal static EnemyLocation FindEnemyLocation(BattleChara enemy)
    {
        Vector3 pPosition = enemy.Position;
        float rotation = enemy.Rotation;
        Vector2 faceVec = new Vector2((float)Math.Cos(rotation), (float)Math.Sin(rotation));

        Vector3 dir = Service.ClientState.LocalPlayer.Position - pPosition;
        Vector2 dirVec = new Vector2(dir.Z, dir.X);

        double angle = Math.Acos(Vector2.Dot(dirVec, faceVec) / dirVec.Length() / faceVec.Length());

        if (angle < Math.PI / 4) return EnemyLocation.Front;
        else if (angle > Math.PI * 3 / 4) return EnemyLocation.Back;
        return EnemyLocation.Side;
    }

    /// <summary>
    /// 常规的能力技，啥时候都能使用。
    /// </summary>
    /// <param name="level"></param>
    /// <param name="abilityRemain"></param>
    /// <param name="act"></param>
    /// <returns></returns>
    private protected virtual bool GeneralAbility(byte abilityRemain, out IAction act)
    {
        act = null; return false;
    }

    private protected virtual bool MoveAbility(byte abilityRemain, out IAction act)
    {
        act = null; return false;
    }

    /// <summary>
    /// 单体治疗的能力技
    /// </summary>
    /// <param name="level"></param>
    /// <param name="abilityRemain"></param>
    /// <param name="act"></param>
    /// <returns></returns>
    private protected virtual bool HealSingleAbility(byte abilityRemain, out IAction act)
    {
        act = null; return false;
    }

    private protected virtual bool DefenceSingleAbility(byte abilityRemain, out IAction act)
    {
        act = null; return false;
    }
    private protected virtual bool DefenceAreaAbility(byte abilityRemain, out IAction act)
    {
        act = null; return false;
    }
    /// <summary>
    /// 范围治疗的能力技
    /// </summary>
    /// <param name="level"></param>
    /// <param name="abilityRemain"></param>
    /// <param name="act"></param>
    /// <returns></returns>
    private protected virtual bool HealAreaAbility(byte abilityRemain, out IAction act)
    {
        act = null; return false;
    }

    private protected virtual bool BreakAbility(byte abilityRemain, out IAction act)
    {
        act = null; return false;
    }
}
