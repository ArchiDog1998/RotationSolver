using System;
using System.Linq;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Data;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.SigReplacers;
using XIVAutoAttack.Updaters;
using XIVAutoAttack.Windows;

namespace XIVAutoAttack.Combos.CustomCombo
{
    internal abstract partial class CustomCombo<TCmd> where TCmd : Enum
    {
        public bool TryInvoke(out IAction newAction)
        {
            newAction = null;
            if (!IsEnabled)
            {
                return false;
            }

            UpdateInfo();

            newAction = Invoke();
            //没获得对象
            if (newAction == null) return false;

            return true;
        }

        private IAction Invoke()
        {
            //倒计时专用能力。
            var countDown = CountDown.CountDownTime;
            if (countDown > 0) return CountDownAction(countDown);

            byte abilityRemain = ActionUpdater.AbilityRemainCount;

            //防AOE
            var helpDefenseAOE = Service.Configuration.AutoDefenseForTank && TargetUpdater.IsHostileAOE;

            //防单体
            bool helpDefenseSingle = false;
            //是个骑士或者奶妈
            if (Job.GetJobRole() == JobRole.Healer || Service.ClientState.LocalPlayer.ClassJob.Id == 19)
            {
                if (Service.Configuration.AutoDefenseForTank && TargetUpdater.PartyTanks.Any((tank) =>
                {
                    var attackingTankObj = TargetUpdater.HostileTargets.Where(t => t.TargetObjectId == tank.ObjectId);

                    if (attackingTankObj.Count() != 1) return false;

                    return TargetUpdater.IsHostileTank;
                })) helpDefenseSingle = true;
            }

            IAction act = GCD(abilityRemain, helpDefenseAOE, helpDefenseSingle);

            if (act != null && act is BaseAction GCDaction)
            {
                //Sayout!
                if (GCDaction.EnermyLocation != EnemyLocation.None && GCDaction.Target.HasLocationSide()
                     && !Player.HasStatus(true, StatusID.TrueNorth))
                {
                    if (CheckAction(GCDaction.ID))
                    {
                        string location = GCDaction.EnermyLocation.ToName();
                        if (Service.Configuration.SayingLocation) Watcher.Speak(location);
                        if (Service.Configuration.ShowLocation) Service.ToastGui.ShowQuest(" " + location, new Dalamud.Game.Gui.Toast.QuestToastOptions()
                        {
                            IconId = Service.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Action>().GetRow(
                                (uint)Service.IconReplacer.OriginalHook((ActionID)GCDaction.ID)).Icon,
                        });
                        OverlayWindow.EnemyLocationTarget = GCDaction.Target;
                        OverlayWindow.ShouldLocation = GCDaction.EnermyLocation;
                    }
                }
                else
                {
                    OverlayWindow.ShouldLocation = EnemyLocation.None;
                }

                if (abilityRemain == 0 || ActionUpdater.WeaponTotal < ActionUpdater._lastCastingTotal) return GCDaction;

                if (Ability(abilityRemain, GCDaction, out IAction ability, helpDefenseAOE, helpDefenseSingle)) return ability;

                return GCDaction;
            }
            else if (act == null)
            {
                OverlayWindow.ShouldLocation = EnemyLocation.None;
                if (Ability(abilityRemain, Addle, out IAction ability, helpDefenseAOE, helpDefenseSingle)) return ability;
                return null;
            }
            else
            {
                OverlayWindow.ShouldLocation = EnemyLocation.None;
            }
            return act;
        }

        uint _lastSayingGCDAction;
        DateTime lastTime;
        private bool CheckAction(uint actionID)
        {
            if ((_lastSayingGCDAction != actionID || DateTime.Now - lastTime > new TimeSpan(0, 0, 3)) && CommandController.AutoAttack)
            {
                _lastSayingGCDAction = actionID;
                lastTime = DateTime.Now;
                return true;
            }
            else return false;
        }


        private IAction GCD(byte abilityRemain, bool helpDefenseAOE, bool helpDefenseSingle)
        {
            IAction act = CommandController.NextAction;
            if (act is BaseAction a && a != null && a.IsRealGCD && a.ShouldUse(out _, mustUse: true, skipDisable: true)) return act;

            if (EmergencyGCD(out act)) return act;

            if (EsunaRaise(out act, abilityRemain, false)) return act;
            if (CommandController.Move && MoveGCD(out act))
            {
                if (act is BaseAction b && TargetFilter.DistanceToPlayer(b.Target) > 5) return act;
            }
            if (TargetUpdater.HPNotFull && ActionUpdater.InCombat)
            {
                if ((CommandController.HealArea || CanHealAreaSpell) && !ShouldUseHealAreaAbility(1, out _)
                    && HealAreaGCD(out act)) return act;
                if ((CommandController.HealSingle || CanHealSingleSpell) && !ShouldUseHealSingleAbility(1, out _)
                    && HealSingleGCD(out act)) return act;
            }
            if (CommandController.DefenseArea && DefenseAreaGCD(out act)) return act;
            if (CommandController.DefenseSingle && DefenseSingleGCD(out act)) return act;

            //自动防御
            if (helpDefenseAOE && DefenseAreaGCD(out act)) return act;
            if (helpDefenseSingle && DefenseSingleGCD(out act)) return act;

            if (GeneralGCD(out var action)) return action;

            //硬拉或者开始奶人
            if (Service.Configuration.RaisePlayerBySwift && (HaveSwift || !Swiftcast.IsCoolDown)
                && EsunaRaise(out act, abilityRemain, true)) return act;
            if (TargetUpdater.HPNotFull && HaveHostilesInRange && ActionUpdater.InCombat)
            {
                if (CanHealAreaSpell && HealAreaGCD(out act)) return act;
                if (CanHealSingleSpell && HealSingleGCD(out act)) return act;
            }
            if (Service.Configuration.RaisePlayerByCasting && EsunaRaise(out act, abilityRemain, true)) return act;

            return null;
        }

        private bool EsunaRaise(out IAction act, byte actabilityRemain, bool mustUse)
        {
            act = null;

            if (Raise == null) return false;

            //有某些非常危险的状态。
            if (CommandController.EsunaOrShield && TargetUpdater.WeakenPeople.Any() || TargetUpdater.DyingPeople.Any())
            {
                if (Job.GetJobRole() == JobRole.Healer && Esuna.ShouldUse(out act, mustUse: true)) return true;
            }

            //蓝量不足，就不复活了。
            if (Player.CurrentMp <= Service.Configuration.LessMPNoRaise) return false;

            //有人死了，看看能不能救。
            if (Service.Configuration.RaiseAll ? TargetUpdater.DeathPeopleAll.Any() : TargetUpdater.DeathPeopleParty.Any())
            {
                if (Service.ClientState.LocalPlayer.ClassJob.Id == 35)
                {
                    if (HaveSwift && Raise.ShouldUse(out act)) return true;
                }
                else if (CommandController.RaiseOrShirk || HaveSwift || !Swiftcast.IsCoolDown && actabilityRemain > 0 || mustUse)
                {
                    if (Raise.ShouldUse(out _))
                    {
                        if (mustUse && Swiftcast.ShouldUse(out act)) return true;
                        act = Raise;
                        return true;
                    }
                }
            }
            act = null;
            return false;
        }

        /// <summary>
        /// 在倒计时的时候返回这个函数里面的技能。
        /// </summary>
        /// <param name="remainTime">距离战斗开始的时间(s)</param>
        /// <returns>要使用的技能</returns>
        private protected virtual IAction CountDownAction(float remainTime) => null;

        /// <summary>
        /// 一些非常紧急的GCD战技，优先级最高
        /// </summary>
        /// <param name="act"></param>
        /// <returns></returns>
        private protected virtual bool EmergencyGCD(out IAction act)
        {
            act = null; return false;
        }
        /// <summary>
        /// 常规GCD技能
        /// </summary>
        /// <param name="act"></param>
        /// <returns></returns>
        private protected abstract bool GeneralGCD(out IAction act);

        /// <summary>
        /// 移动GCD技能
        /// </summary>
        /// <param name="act"></param>
        /// <returns></returns>
        private protected virtual bool MoveGCD(out IAction act)
        {
            act = null; return false;
        }

        /// <summary>
        /// 单体治疗GCD
        /// </summary>
        /// <param name="act"></param>
        /// <returns></returns>
        private protected virtual bool HealSingleGCD(out IAction act)
        {
            act = null; return false;
        }

        /// <summary>
        /// 范围治疗GCD
        /// </summary>
        /// <param name="level"></param>
        /// <param name="lastComboActionID"></param>
        /// <param name="act"></param>
        /// <returns></returns>
        private protected virtual bool HealAreaGCD(out IAction act)
        {
            act = null; return false;
        }

        /// <summary>
        /// 单体防御GCD
        /// </summary>
        /// <param name="act"></param>
        /// <returns></returns>
        private protected virtual bool DefenseSingleGCD(out IAction act)
        {
            act = null; return false;
        }
        /// <summary>
        /// 范围防御GCD
        /// </summary>
        /// <param name="act"></param>
        /// <returns></returns>
        private protected virtual bool DefenseAreaGCD(out IAction act)
        {
            act = null; return false;
        }
    }
}
