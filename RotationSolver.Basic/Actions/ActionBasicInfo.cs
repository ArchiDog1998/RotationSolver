using ECommons.ExcelServices;
using ECommons.GameHelpers;
using FFXIVClientStructs.FFXIV.Client.Game;
using RotationSolver.Basic.Configuration;

namespace RotationSolver.Basic.Actions;

/// <summary>
/// The action info for the <see cref="Lumina.Excel.GeneratedSheets.Action"/>.
/// </summary>
public readonly struct ActionBasicInfo
{
    internal static readonly uint[] ActionsNoNeedCasting =
    [
        5,
        (uint)ActionID.PowerfulShotPvP,
        (uint)ActionID.BlastChargePvP,
    ];

    private readonly IBaseAction _action;

    /// <summary>
    /// The name of the action.
    /// </summary>
    public readonly string Name => _action.Action.Name;

    /// <summary>
    /// The ID of the action.
    /// </summary>
    public readonly uint ID => _action.Action.RowId;

    /// <summary>
    /// The icon of the action.
    /// </summary>
    public readonly uint IconID => ID == (uint)ActionID.SprintPvE ? 104u : _action.Action.Icon;

    /// <summary>
    /// The adjust id of this action.
    /// </summary>
    public readonly uint AdjustedID => (uint)Service.GetAdjustedActionId((ActionID)ID);

    /// <summary>
    /// The attack type of this action.
    /// </summary>
    public readonly AttackType AttackType => (AttackType)(_action.Action.AttackType.Value?.RowId ?? byte.MaxValue);

    /// <summary>
    /// The aspect of this action.
    /// </summary>
    public Aspect Aspect { get; }

    /// <summary>
    /// The animation lock time of this action.
    /// </summary>
    [Obsolete("Use ActionManagerHelper.GetCurrentAnimationLock()")]
    public readonly float AnimationLockTime => ActionManagerHelper.GetCurrentAnimationLock();

    /// <summary>
    /// The level of this action.
    /// </summary>
    public readonly byte Level => _action.Action.ClassJobLevel;

    /// <summary>
    /// If this action is enough level to use.
    /// </summary>
    public readonly bool EnoughLevel => Player.Level >= Level;

    /// <summary>
    /// If this action a pvp action.
    /// </summary>
    public readonly bool IsPvP => _action.Action.IsPvP;

    /// <summary>
    /// Casting time.
    /// </summary>
    public readonly unsafe float CastTime => ((ActionID)AdjustedID).GetCastTime();

    /// <summary>
    /// How many mp does this action needs.
    /// </summary>
    public readonly unsafe uint MPNeed
    {
        get
        {
            var mpOver = _action.Setting.MPOverride?.Invoke();
            if (mpOver.HasValue) return mpOver.Value;

            var mp = (uint)ActionManager.GetActionCost(ActionType.Action, AdjustedID, 0, 0, 0, 0);
            if (mp < 100) return 0;
            return mp;
        }
    }

    /// <summary>
    /// Is this action on the slot.
    /// </summary>
    public readonly bool IsOnSlot
    {
        get
        {
            if (_action.Action.ClassJob.Row == (uint)Job.BLU)
            {
                return DataCenter.BluSlots.Contains(ID);
            }

            if (IsDutyAction)
            {
                return DataCenter.DutyActions.Contains(ID);
            }

            return IsPvP == DataCenter.Territory?.IsPvpZone;
        }
    }

    /// <summary>
    /// Is this action is a lb action.
    /// </summary>
    public bool IsLimitBreak { get; }

    /// <summary>
    /// Is this action a general gcd.
    /// </summary>
    public bool IsGeneralGCD { get; }

    /// <summary>
    /// Is this action a real gcd.
    /// </summary>
    public bool IsRealGCD { get; }

    /// <summary>
    /// Is this action a duty action.
    /// </summary>
    public bool IsDutyAction { get; }

    /// <summary>
    /// The basic way to create a basic info
    /// </summary>
    /// <param name="action">the action</param>
    /// <param name="isDutyAction">if it is a duty action.</param>
    public ActionBasicInfo(IBaseAction action, bool isDutyAction)
    {
        _action = action;
        IsGeneralGCD = _action.Action.IsGeneralGCD();
        IsRealGCD = _action.Action.IsRealGCD();
        IsLimitBreak = (ActionCate?)_action.Action.ActionCategory?.Value?.RowId
            is ActionCate.LimitBreak or ActionCate.LimitBreak_15;
        IsDutyAction = isDutyAction;
        Aspect = (Aspect)_action.Action.Aspect;
    }

    internal readonly bool BasicCheck(bool skipStatusProvideCheck, bool skipComboCheck, bool skipCastingCheck)
    {
        if (!_action.Config.IsEnabled || !IsOnSlot) return false;

        if (IsLimitBreak) return true;

        //Disabled.
        if (DataCenter.DisabledActionSequencer?.Contains(ID) ?? false) return false;

        if (!EnoughLevel) return false;
        if (DataCenter.CurrentMp < MPNeed) return false;
        if (_action.Setting.UnlockedByQuestID != 0)
        {
            var isUnlockQuestComplete = QuestManager.IsQuestComplete(_action.Setting.UnlockedByQuestID);
            if (!isUnlockQuestComplete)
            {
                var warning = $"The action {Name} is locked by the quest {_action.Setting.UnlockedByQuestID}. Please complete this quest to learn this action.";
                WarningHelper.AddSystemWarning(warning);
                return false;
            }
        }

        var player = Player.Object;

        if (_action.Setting.StatusNeed != null)
        {
            if (player.WillStatusEndGCD(0, 0,
                _action.Setting.StatusFromSelf, _action.Setting.StatusNeed)) return false;
        }

        if (_action.Setting.StatusProvide != null && !skipStatusProvideCheck)
        {
            if (!player.WillStatusEndGCD(_action.Config.StatusGcdCount, 0,
                _action.Setting.StatusFromSelf, _action.Setting.StatusProvide)) return false;
        }

        if (_action.Action.ActionCategory.Row == 15)
        {
            if (CustomRotation.LimitBreakLevel <= 1) return false;
        }

        if (!skipComboCheck && IsGeneralGCD)
        {
            if (!CheckForCombo()) return false;
        }

        if (_action.Action.IsRoleAction)
        {
            if (!_action.Action.ClassJobCategory.Value?.IsJobInCategory(DataCenter.Job) ?? false) return false;
        }

        //Need casting.
        if (CastTime > 0 && !player.HasStatus(true, 
            [
                StatusID.Swiftcast,
                StatusID.Triplecast,
                StatusID.Dualcast,
            ])
            && !ActionsNoNeedCasting.Contains(ID))
        {
            //No casting.
            if(DataCenter.SpecialType == SpecialCommandType.NoCasting) return false;

            //Is knocking back.
            if (DateTime.Now > DataCenter.KnockbackStart && DateTime.Now < DataCenter.KnockbackFinished) return false;

            if (DataCenter.NoPoslock && DataCenter.IsMoving && !skipCastingCheck) return false;
        }

        if (IsGeneralGCD && _action.Setting.StatusProvide?.Length > 0 && _action.Setting.IsFriendly
            && IActionHelper.IsLastGCD(true, _action)
            && DataCenter.TimeSinceLastAction.TotalSeconds < 3) return false;

        if (!(_action.Setting.ActionCheck?.Invoke() ?? true)) return false;
        if (!IBaseAction.ForceEnable && !(_action.Setting.RotationCheck?.Invoke() ?? true)) return false;

        return true;
    }

    private readonly bool CheckForCombo()
    {
        if (_action.Setting.ComboIdsNot != null)
        {
            if (_action.Setting.ComboIdsNot.Contains(DataCenter.LastComboAction)) return false;
        }

        var comboActions = (_action.Action.ActionCombo?.Row ?? 0) != 0
            ? new ActionID[] { (ActionID)_action.Action.ActionCombo!.Row }
            : [];

        if (_action.Setting.ComboIds != null) comboActions = [.. comboActions, .. _action.Setting.ComboIds];

        if (comboActions.Length > 0)
        {
            if (comboActions.Contains(DataCenter.LastComboAction))
            {
                if (DataCenter.ComboTime < DataCenter.DefaultGCDRemain) return false;
            }
            else
            {
                return false;
            }
        }
        return true;
    }
}


