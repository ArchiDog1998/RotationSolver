using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Synthesis;
using System.Threading.Tasks;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;

namespace XIVComboPlus.Combos;

public abstract class CustomCombo
{
    //private static SpeechSynthesizer ssh = new SpeechSynthesizer() { Rate = 0 };
    private uint _lastGCDAction;

    internal static bool HaveSwift
    {
        get
        {
            foreach (var status in Service.ClientState.LocalPlayer.StatusList)
            {
                if (GeneralActions.Swiftcast.BuffsProvide.Contains((ushort)status.StatusId))
                {
                    return true;
                }
            }
            return false;
        }
    }

    #region Job

    internal abstract uint JobID { get; }

    internal abstract string JobName { get; }

    internal struct GeneralActions
    {
        internal static readonly BaseAction
            //混乱
            Addle = new BaseAction(7560u),

            //即刻咏唱
            Swiftcast = new BaseAction(7561u)
            {
                BuffsProvide = new ushort[]
            {
                ObjectStatus.Swiftcast1,
                ObjectStatus.Swiftcast2,
                ObjectStatus.Swiftcast3,
                ObjectStatus.Triplecast,
                ObjectStatus.Dualcast,
                ObjectStatus.Acceleration,
            }
            },

            //康复
            Esuna = new BaseAction(7568)
            {
                OtherCheck = () => TargetHelper.WeakenPeople.Length > 0,
            },

            //营救
            Rescue = new BaseAction(7571),

            //沉静
            Repose = new BaseAction(16560),

            //醒梦（如果MP低于6000那么使用）
            LucidDreaming = new BaseAction(7562u)
            {
                OtherCheck = () => Service.ClientState.LocalPlayer.CurrentMp < 6000,
            },

            //伤腿
            LegGraze = new BaseAction(7554)
            {
                BuffsProvide = new ushort[]
                {
                    13,564,1345,
                },
            },

            //内丹
            SecondWind = new BaseAction(7541)
            {
                OtherCheck = () => (double)Service.ClientState.LocalPlayer.CurrentHp / Service.ClientState.LocalPlayer.MaxHp < 0.6,
            },

            //伤足
            FootGraze = new BaseAction(7553),

            //亲疏自行
            ArmsLength = new BaseAction(7548),

            //铁壁
            Rampart = new BaseAction(7531)
            {
                BuffsProvide = new ushort[]
                {
                    ObjectStatus.Rampart1, ObjectStatus.Rampart2, ObjectStatus.Rampart3,
                    //原初的直觉和血气
                    ObjectStatus.RawIntuition, ObjectStatus.Bloodwhetting,
                    //复仇
                    ObjectStatus.Vengeance,
                },
            },

            //挑衅
            Provoke = new BaseAction(7533),

            //雪仇
            Reprisal = new BaseAction(7535),

            //退避
            Shirk = new BaseAction(7537),

            //浴血
            Bloodbath = new BaseAction(7542);

    }
    #endregion

    #region Combo
    protected internal abstract uint[] ActionIDs { get; }
    public abstract string ComboFancyName { get; }

    public abstract string Description { get; }

    public virtual string[] ConflictingCombos => new string[0];
    public virtual string ParentCombo => string.Empty;

    /// <summary>
    /// 优先级，越大就使用到的概率越高！
    /// </summary>
    public virtual byte Priority => 0;
    public bool IsEnabled
    {
        get => Service.Configuration.EnabledActions.Contains(ComboFancyName);
        set
        {
            if (value)
            {
                Service.Configuration.EnabledActions.Add(ComboFancyName);
            }
            else
            {
                Service.Configuration.EnabledActions.Remove(ComboFancyName);
            }
        }
    }

    #endregion

    protected static PlayerCharacter LocalPlayer => Service.ClientState.LocalPlayer;
    protected static GameObject Target => Service.TargetManager.Target;

    protected static bool IsMoving => TargetHelper.IsMoving;
    private static bool HaveTargetAngle => TargetHelper.GetObjectInRadius(TargetHelper.HostileTargets, 25).Length > 0;
    internal static byte AbilityRemainCount => (byte)(WeaponRemain / 0.67f);

    protected static float WeaponRemain => Service.IconReplacer.GetCooldown(BaseAction.GCDCooldownGroup).CooldownRemaining;

    protected virtual bool CanHealAreaAbility => TargetHelper.PartyMembersDifferHP < Service.Configuration.HealthDifference && TargetHelper.PartyMembersAverHP < Service.Configuration.HealthAreaAbility;
    protected virtual bool CanHealAreaSpell => TargetHelper.PartyMembersDifferHP < Service.Configuration.HealthDifference && TargetHelper.PartyMembersAverHP < Service.Configuration.HealthAreafSpell;

    protected virtual bool CanHealSingleAbility => TargetHelper.PartyMembersHP.Min() < Service.Configuration.HealthSingleAbility;
    protected virtual bool CanHealSingleSpell => TargetHelper.PartyMembersHP.Min() < Service.Configuration.HealthSingleSpell;

    private bool HPFull => TargetHelper.PartyMembersHP.Min() > 0.99f;
    /// <summary>
    /// Only one feature can set it to true!
    /// </summary>
    protected bool ShouldSayout { get; set; } = false;
    protected CustomCombo()
    {
    }

    internal bool TryInvoke(uint actionID, uint lastComboActionID, float comboTime, byte level, out uint newActionID)
    {
        newActionID = 0u;
        if (!IsEnabled)
        {
            return false;
        }
        if (!ActionIDs.Contains(actionID))
        {
            return false;
        }

        uint actNew = Invoke(actionID, lastComboActionID, comboTime, level);
        if (actionID == actNew)
        {
            return false;
        }
        else if (actNew == 0)
        {
            SortedSet<byte> validJobs = new SortedSet<byte>(ClassJob.AllJobs.Where(job => job.Type == JobType.MagicalRanged || job.Type == JobType.Healer).Select(job => job.Index));

            newActionID = TargetHelper.GetJobCategory(Service.ClientState.LocalPlayer, validJobs) ? GeneralActions.SecondWind.ActionID : GeneralActions.LucidDreaming.ActionID;
            return true;
        }
        newActionID = actNew;
        return true;
    }

    private bool CheckAction(uint actionID)
    {
        //return false;
        if (ShouldSayout && _lastGCDAction != actionID)
        {
            _lastGCDAction = actionID;
            return true;
        }
        else return false;
    }

    private uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level)
    {

        byte abilityRemain = AbilityRemainCount;
        BaseAction GCDaction = GCD(level, lastComboActionID);
        if (GCDaction == null) return 0;

        //Sayout!
        if (CheckAction(GCDaction.ActionID) && GCDaction.SayoutText != EnemyLocation.None)
        {
            Service.ChatGui.PrintChat(new Dalamud.Game.Text.XivChatEntry()
            {
                Message = GCDaction.Action.Name + " " + GCDaction.SayoutText.ToString(),
                Type = Dalamud.Game.Text.XivChatType.Notice,
            });
        }

        uint GCDact = GCDaction.ActionID;

        //return GCDact;
        switch (AbilityRemainCount)
        {
            case 0:
                return GCDact;
            default:
                BaseAction AbilityAction;
                if (FirstActionAbility(level, abilityRemain, GCDaction, out AbilityAction)) return AbilityAction.ActionID;
                if (!HPFull)
                {
                    if (CanHealAreaAbility && HealAreaAbility(level, abilityRemain, out AbilityAction)) return AbilityAction.ActionID;
                    if (CanHealSingleAbility && HealSingleAbility(level, abilityRemain, out AbilityAction)) return AbilityAction.ActionID;
                }
                if (GeneralAbility(level, abilityRemain, out AbilityAction)) return AbilityAction.ActionID;
                if (HaveTargetAngle && ForAttachAbility(level, abilityRemain, out AbilityAction)) return AbilityAction.ActionID;
                return GCDact;
        }
    }

    private BaseAction GCD(byte level, uint lastComboActionID)
    {
        if (EmergercyGCD(level, lastComboActionID, out BaseAction act)) return act;
        if (!HPFull)
        {
            if (CanHealAreaSpell && HealAreaGCD(level, lastComboActionID, out act)) return act;
            if (CanHealSingleSpell && HealSingleGCD(level, lastComboActionID, out act)) return act;
        }
        if (AttackGCD(level, lastComboActionID, out act)) return act;
        return null;
    }
    private protected abstract bool ForAttachAbility(byte level, byte abilityRemain, out BaseAction act);
    private protected virtual bool FirstActionAbility(byte level, byte abilityRemain, BaseAction nextGCD, out BaseAction act)
    {
        if (nextGCD.CastTime > 7000 && GeneralActions.Swiftcast.TryUseAction(level, out act, mustUse: true)) return true;
        act = null; return false;
    }
    private protected virtual bool GeneralAbility(byte level, byte abilityRemain, out BaseAction act)
    {
        act = null; return false;
    }
    private protected virtual bool HealSingleAbility(byte level, byte abilityRemain, out BaseAction act)
    {
        act = null; return false;
    }
    private protected virtual bool HealAreaAbility(byte level, byte abilityRemain, out BaseAction act)
    {
        act = null; return false;
    }
    private protected virtual bool EmergercyGCD(byte level, uint lastComboActionID, out BaseAction act)
    {
        act = null; return false;
    }
    private protected abstract bool AttackGCD(byte level, uint lastComboActionID, out BaseAction act);
    private protected virtual bool HealSingleGCD(byte level, uint lastComboActionID, out BaseAction act)
    {
        act = null; return false;
    }
    private protected virtual bool HealAreaGCD(byte level, uint lastComboActionID, out BaseAction act)
    {
        act = null; return false;
    }
}
