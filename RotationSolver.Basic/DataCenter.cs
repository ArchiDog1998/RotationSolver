using Dalamud.Game.ClientState.Conditions;
using ECommons.DalamudServices;
using ECommons.ExcelServices;
using ECommons.GameHelpers;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Fate;
using Lumina.Excel.GeneratedSheets;
using RotationSolver.Basic.Configuration.Conditions;
using RotationSolver.Basic.Rotations.Duties;
using Action = Lumina.Excel.GeneratedSheets.Action;
using CharacterManager = FFXIVClientStructs.FFXIV.Client.Game.Character.CharacterManager;

namespace RotationSolver.Basic;

internal static class DataCenter
{
    private static uint _hostileTargetId = GameObject.InvalidGameObjectId;

    public static bool IsActivated() => State || IsManual || Service.Config.TeachingMode;
    internal static BattleChara? HostileTarget
    {
        get
        {
            return Svc.Objects.SearchById(_hostileTargetId) as BattleChara;
        }
        set
        {
            _hostileTargetId = value?.ObjectId ?? GameObject.InvalidGameObjectId;
        }
    }

    internal static Queue<MapEffectData> MapEffects { get; } = new(64);
    internal static Queue<ObjectEffectData> ObjectEffects { get; } = new(64);
    internal static Queue<VfxNewData> VfxNewData { get; } = new(64);

    /// <summary>
    /// This one never be null.
    /// </summary>
    public static MajorConditionSet RightSet
    {
        get
        {
            if (ConditionSets == null || !ConditionSets.Any())
            {
                ConditionSets = [new MajorConditionSet()];
            }

            var index = Service.Config.ActionSequencerIndex;
            if (index < 0 || index >= ConditionSets.Length)
            {
                Service.Config.ActionSequencerIndex = index = 0;
            }

            return ConditionSets[index];
        }
    }

    internal static MajorConditionSet[] ConditionSets { get; set; } = [];

    /// <summary>
    /// Only recorded 15s hps.
    /// </summary>
    public const int HP_RECORD_TIME = 240;
    internal static Queue<(DateTime time, SortedList<uint, float> hpRatios)> RecordedHP { get; } = new(HP_RECORD_TIME + 1);

    public static ICustomRotation? RightNowRotation { get; internal set; }
    public static DutyRotation? RightNowDutyRotation { get; internal set; }

    public static Dictionary<string, DateTime> SystemWarnings { get; set; } = new();

    internal static bool NoPoslock => Svc.Condition[ConditionFlag.OccupiedInEvent]
        || !Service.Config.PoslockCasting
        //Key cancel.
        || Svc.KeyState[Service.Config.PoslockModifier.ToVirtual()]
        //Gamepad cancel.
        || Svc.GamepadState.Raw(Dalamud.Game.ClientState.GamePad.GamepadButtons.R1) >= 0.5f;

    internal static DateTime EffectTime { private get; set; } = DateTime.Now;
    internal static DateTime EffectEndTime { private get; set; } = DateTime.Now;

    internal const int ATTACKED_TARGETS_COUNT = 48;
    internal static Queue<(ulong id, DateTime time)> AttackedTargets { get; } = new(ATTACKED_TARGETS_COUNT);

    internal static bool InEffectTime => DateTime.Now >= EffectTime && DateTime.Now <= EffectEndTime;
    internal static Dictionary<ulong, uint> HealHP { get; set; } = [];
    internal static Dictionary<ulong, uint> ApplyStatus { get; set; } = [];
    internal static uint MPGain { get; set; }
    internal static bool HasApplyStatus(uint id, StatusID[] ids)
    {
        if (InEffectTime && ApplyStatus.TryGetValue(id, out var statusId))
        {
            if (ids.Any(s => (ushort)s == statusId)) return true;
        }
        return false;
    }

    public static TerritoryType? Territory { get; set; }

    public static string TerritoryName => Territory?.PlaceName?.Value?.Name?.RawString ?? "Territory";

    public static bool IsPvP => Territory?.IsPvpZone ?? false;

    public static ContentFinderCondition? ContentFinder => Territory?.ContentFinderCondition?.Value;

    public static string ContentFinderName => ContentFinder?.Name?.RawString ?? "Duty";

    public static bool IsInHighEndDuty => ContentFinder?.HighEndDuty ?? false;
    public static TerritoryContentType TerritoryContentType => (TerritoryContentType)(ContentFinder?.ContentType?.Value?.RowId ?? 0);

    public static AutoStatus MergedStatus => AutoStatus | CommandStatus;

    public static AutoStatus AutoStatus { get; set; } = AutoStatus.None;
    public static AutoStatus CommandStatus { get; set; } = AutoStatus.None;

    public static HashSet<uint> DisabledActionSequencer { get; set; } = [];

    private static List<NextAct> NextActs = [];
    public static IAction? ActionSequencerAction { private get; set; }
    public static IAction? CommandNextAction
    {
        get
        {
            var next = NextActs.FirstOrDefault();

            while (next != null && NextActs.Count > 0 && (next.DeadTime < DateTime.Now || IActionHelper.IsLastAction(true, next.Act)))
            {
                NextActs.RemoveAt(0);
                next = NextActs.FirstOrDefault();
            }
            return next?.Act ?? ActionSequencerAction;
        }
    }
    public static Job Job => Player.Job;

    public static JobRole Role => Service.GetSheet<ClassJob>().GetRow((uint)Job)?.GetJobRole() ?? JobRole.None;

    internal static void AddCommandAction(IAction act, double time)
    {
        var index = NextActs.FindIndex(i => i.Act.ID == act.ID);
        var newItem = new NextAct(act, DateTime.Now.AddSeconds(time));
        if (index < 0)
        {
            NextActs.Add(newItem);
        }
        else
        {
            NextActs[index] = newItem;
        }
        NextActs = [.. NextActs.OrderBy(i => i.DeadTime)];
    }

    public static TargetHostileType RightNowTargetToHostileType => Service.Config.HostileType;

    public static unsafe ActionID LastComboAction => (ActionID)ActionManager.Instance()->Combo.Action;
    public static unsafe float ComboTime => ActionManager.Instance()->Combo.Timer;
    public static TargetingType TargetingType
    {
        get
        {
            if (Service.Config.TargetingTypes.Count == 0)
            {
                Service.Config.TargetingTypes.Add(TargetingType.Big);
                Service.Config.TargetingTypes.Add(TargetingType.Small);
                Service.Config.Save();
            }

            return Service.Config.TargetingTypes[Service.Config.TargetingIndex % Service.Config.TargetingTypes.Count];
        }
    }

    public static bool IsMoving { get; internal set; }

    internal static float StopMovingRaw { get; set; }

    public static unsafe ushort FateId
    {
        get
        {
            try
            {
                if (Service.Config.ChangeTargetForFate && (IntPtr)FateManager.Instance() != IntPtr.Zero
                    && (IntPtr)FateManager.Instance()->CurrentFate != IntPtr.Zero
                    && Player.Level <= FateManager.Instance()->CurrentFate->MaxLevel)
                {
                    return FateManager.Instance()->CurrentFate->FateId;
                }
            }
            catch (Exception ex)
            {
                Svc.Log.Error(ex.StackTrace ?? ex.Message);
            }
            return 0;
        }
    }

    #region GCD
    public static float WeaponRemain { get; internal set; }

    public static float WeaponTotal { get; internal set; }

    public static float WeaponElapsed { get; internal set; }

    public static float GCDTime(uint gcdCount = 0, float offset = 0)
        => WeaponTotal * gcdCount + offset;

    /// <summary>
    /// Time to the next action
    /// </summary>
    public static unsafe float ActionRemain => ActionManagerHelper.GetCurrentAnimationLock();

    public static float AbilityRemain => ActionManagerHelper.GetCurrentAnimationLock();
    //{
     //   get
     //   {
     //       var gcdRemain = WeaponRemain;
     //       // Check if we should account for the animation lock and ping.
     //       if (gcdRemain - MinAnimationLock - Ping <= ActionRemain)
     //       {
     //           return gcdRemain + MinAnimationLock + Ping;
     //       }
     //       return ActionRemain;
     //   }
    //}

    // Update the property to conditionally use AbilityRemain based on the NoPingCheck setting.
    public static float NextAbilityToNextGCD => WeaponRemain - ActionRemain;

    public static float CastingTotal { get; internal set; }
    #endregion


    public static uint[] BluSlots { get; internal set; } = new uint[24];

    public static uint[] DutyActions { get; internal set; } = new uint[2];

    static DateTime _specialStateStartTime = DateTime.MinValue;
    private static double SpecialTimeElapsed => (DateTime.Now - _specialStateStartTime).TotalSeconds;
    public static double SpecialTimeLeft => WeaponTotal == 0 || WeaponElapsed == 0 ? Service.Config.SpecialDuration - SpecialTimeElapsed :
        Math.Ceiling((Service.Config.SpecialDuration + WeaponElapsed - SpecialTimeElapsed) / WeaponTotal) * WeaponTotal - WeaponElapsed;

    static SpecialCommandType _specialType = SpecialCommandType.EndSpecial;
    internal static SpecialCommandType SpecialType
    {
        get
        {
            return SpecialTimeLeft < 0 ? SpecialCommandType.EndSpecial : _specialType;
        }
        set
        {
            _specialType = value;
            _specialStateStartTime = value == SpecialCommandType.EndSpecial ? DateTime.MinValue : DateTime.Now;
        }
    }

    public static bool State { get; set; } = false;

    public static bool IsManual { get; set; } = false;

    public static bool InCombat { get; set; }

    static RandomDelay _notInCombatDelay = new(() => Service.Config.NotInCombatDelay);

    /// <summary>
    /// Is out of combat.
    /// </summary>
    public static bool NotInCombatDelay => _notInCombatDelay.Delay(!InCombat);

    internal static float CombatTimeRaw { get; set; }
    private static DateTime _startRaidTime = DateTime.MinValue;
    internal static float RaidTimeRaw
    {
        get
        {
            if (_startRaidTime == DateTime.MinValue) return 0;
            return (float)(DateTime.Now - _startRaidTime).TotalSeconds;    
        }
        set
        {
            if (value < 0)
            {
                _startRaidTime = DateTime.MinValue;
                foreach (var item in TimelineItems)
                {
                    item.LastActionID = 0;
                }
            }
            else
            {
                _startRaidTime = DateTime.Now - TimeSpan.FromSeconds(value);
            }
        }
    }

    internal static TimelineItem[] TimelineItems { get; set; } = [];

    public static BattleChara[] PartyMembers { get; internal set; } = [];
    public static BattleChara[] AllianceMembers { get; internal set; } = [];
    public static BattleChara[] AllHostileTargets { get; internal set; } = [];

    public static BattleChara? InterruptTarget { get; internal set; }

    public static BattleChara? ProvokeTarget { get; internal set; }
    public static BattleChara? DeathTarget { get; internal set; }
    public static BattleChara? DispelTarget { get; internal set; }

    public static ObjectListDelay<BattleChara> AllTargets { get; } = new(() => Service.Config.TargetDelay);

    public static uint[] TreasureCharas { get; internal set; } = [];
    public static bool HasHostilesInRange => NumberOfHostilesInRange > 0;
    public static bool HasHostilesInMaxRange => NumberOfHostilesInMaxRange > 0;
    public static int NumberOfHostilesInRange { get; internal set; }
    public static int NumberOfHostilesInMaxRange { get; internal set; }
    public static int NumberOfAllHostilesInRange { get; internal set; }
    public static int NumberOfAllHostilesInMaxRange { get; internal set; }
    public static bool MobsTime { get; internal set; }

    private static float _averageTimeToKill;
    public static float AverageTimeToKill 
    {
        get => _averageTimeToKill;
        internal set
        {
            _averageTimeToKill = value;

            foreach (var item in TimelineItems)
            {
                if (item.Time < RaidTimeRaw) continue;
                if (item.Name is not "--untargetable--") continue;

                var time = item.Time - RaidTimeRaw;
                TimeToUntargetable = MathF.Min(time, _averageTimeToKill);
                return;
            }

            TimeToUntargetable = _averageTimeToKill;
        }
    }

    public static float TimeToUntargetable { get; private set; }

    public static bool IsHostileCastingAOE { get; internal set; }

    public static bool IsHostileCastingToTank { get; internal set; }

    public static bool IsHostileCastingKnockback { get; internal set; }

    public static bool HasPet { get; internal set; }

    public static unsafe bool HasCompanion => (IntPtr)Player.BattleChara != IntPtr.Zero
                                           && (IntPtr)CharacterManager.Instance()->LookupBuddyByOwnerObject(Player.BattleChara) != IntPtr.Zero;

    #region HP
    public static Dictionary<uint, float> RefinedHP { get; internal set; } = [];

    public static IEnumerable<float> PartyMembersHP { get; internal set; } = [];
    public static float PartyMembersMinHP { get; internal set; }
    public static float PartyMembersAverHP { get; internal set; }
    public static float PartyMembersDifferHP { get; internal set; }

    public static bool HPNotFull { get; internal set; }

    public static uint CurrentMp { get; internal set; }
    #endregion
    internal static Queue<MacroItem> Macros { get; } = new Queue<MacroItem>();

    #region Action Record
    const int QUEUECAPACITY = 32;
    private static readonly Queue<ActionRec> _actions = new(QUEUECAPACITY);
    private static readonly Queue<DamageRec> _damages = new(QUEUECAPACITY);

    public static float DPSTaken
    {
        get
        {
            try
            {
                var recs = _damages.Where(r => DateTime.Now - r.ReceiveTime < TimeSpan.FromMilliseconds(5));

                if (!recs.Any()) return 0;

                var damages = recs.Sum(r => r.Ratio);

                var time = recs.Last().ReceiveTime - recs.First().ReceiveTime + TimeSpan.FromMilliseconds(2.5f);

                return damages / (float)time.TotalSeconds;
            }
            catch
            {
                return 0;
            }
        }
    }

    public static ActionRec[] RecordActions => _actions.Reverse().ToArray();
    private static DateTime _timeLastActionUsed = DateTime.Now;
    public static TimeSpan TimeSinceLastAction => DateTime.Now - _timeLastActionUsed;

    public static ActionID LastAction { get; private set; } = 0;

    public static ActionID LastGCD { get; private set; } = 0;

    public static ActionID LastAbility { get; private set; } = 0;
    public static float Ping => Math.Max(RTT, FetchTime);

    public static float RTT { get; internal set; } = 0.05f;
    public static float FetchTime { get; private set; } = 0.05f;


    public const float MinAnimationLock = 0.6f;
    internal static unsafe void AddActionRec(Action act)
    {
        if (!Player.Available) return;

        var id = (ActionID)act.RowId;

        //Record
        switch (act.GetActionCate())
        {
            case ActionCate.Spell:
            case ActionCate.Weaponskill:
                LastAction = LastGCD = id;
                if (ActionManager.GetAdjustedCastTime(ActionType.Action, (uint)id) == 0)
                {
                    FetchTime = WeaponElapsed;
                }
                break;
            case ActionCate.Ability:
                LastAction = LastAbility = id;

                try
                {
                    if (!act.IsRealGCD() && ActionManager.GetMaxCharges((uint)id, Player.Object?.Level ?? 1) < 2)
                    {
                        FetchTime = ActionManager.Instance()->GetRecastGroupDetail(act.CooldownGroup - 1)->Elapsed;
                    }
                }
                catch { }

                break;
            default:
                return;
        }

        if (_actions.Count >= QUEUECAPACITY)
        {
            _actions.Dequeue();
        }
        _timeLastActionUsed = DateTime.Now;
        _actions.Enqueue(new ActionRec(_timeLastActionUsed, act));
    }

    internal static void ResetAllRecords()
    {
        LastAction = 0;
        LastGCD = 0;
        LastAbility = 0;
        _timeLastActionUsed = DateTime.Now;
        _actions.Clear();

        MapEffects.Clear();
        ObjectEffects.Clear();
        VfxNewData.Clear();
    }

    internal static void AddDamageRec(float damageRatio)
    {
        if (_damages.Count >= QUEUECAPACITY)
        {
            _damages.Dequeue();
        }
        _damages.Enqueue(new DamageRec(DateTime.Now, damageRatio));
    }

    internal static DateTime KnockbackFinished { get; set; } = DateTime.MinValue;
    internal static DateTime KnockbackStart { get; set; } = DateTime.MinValue;
    #endregion

    internal static SortedList<string, string> AuthorHashes { get; set; } = [];
    internal static string[] ContributorsHash { get; set; } = [];
}
