using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.SubKinds;
using ECommons.DalamudServices;
using ECommons.ExcelServices;
using ECommons.GameHelpers;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Fate;
using Lumina.Excel.GeneratedSheets;
using RotationSolver.Basic.Configuration;
using RotationSolver.Basic.Configuration.Conditions;
using Action = Lumina.Excel.GeneratedSheets.Action;
using CharacterManager = FFXIVClientStructs.FFXIV.Client.Game.Character.CharacterManager;

namespace RotationSolver.Basic;

internal static class DataCenter
{
    private static uint _hostileTargetId = GameObject.InvalidGameObjectId;
    internal static BattleChara HostileTarget
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
                ConditionSets = new MajorConditionSet[] { new MajorConditionSet() };
            }

            var index = Service.Config.GetValue(PluginConfigInt.ActionSequencerIndex);
            if (index < 0 || index >= ConditionSets.Length)
            {
                index = 0;
                Service.Config.SetValue(PluginConfigInt.ActionSequencerIndex, index);
            }

            return ConditionSets[index];
        }
    }

    internal static MajorConditionSet[] ConditionSets { get; set; } = Array.Empty<MajorConditionSet>();

    /// <summary>
    /// Only recorded 15s hps.
    /// </summary>
    public const int HP_RECORD_TIME = 240;
    internal static Queue<(DateTime time, SortedList<uint, float> hpRatios)> RecordedHP { get; } = new(HP_RECORD_TIME + 1);

    public static ICustomRotation RightNowRotation { get; internal set; }

    internal static bool NoPoslock => Svc.Condition[ConditionFlag.OccupiedInEvent]
        || !Service.Config.GetValue(PluginConfigBool.PoslockCasting)
        //Key cancel.
        || Svc.KeyState[ConfigurationHelper.Keys[Service.Config.GetValue(PluginConfigInt.PoslockModifier) % ConfigurationHelper.Keys.Length]]
        //Gamepad cancel.
        || Svc.GamepadState.Raw(Dalamud.Game.ClientState.GamePad.GamepadButtons.L2) >= 0.5f;

    internal static DateTime EffectTime { private get; set; } = DateTime.Now;
    internal static DateTime EffectEndTime { private get; set; } = DateTime.Now;

    internal const int ATTACKED_TARGETS_COUNT = 48;
    internal static Queue<(ulong id, DateTime time)> AttackedTargets { get; } = new(ATTACKED_TARGETS_COUNT);

    internal static bool InEffectTime => DateTime.Now >= EffectTime && DateTime.Now <= EffectEndTime;
    internal static Dictionary<ulong, uint> HealHP { get; set; } = new Dictionary<ulong, uint>();
    internal static Dictionary<ulong, uint> ApplyStatus { get; set; } = new Dictionary<ulong, uint>();
    internal static uint MPGain { get; set; }
    internal static bool HasApplyStatus(uint id, StatusID[] ids)
    {
        if (InEffectTime && ApplyStatus.TryGetValue(id, out var statusId))
        {
            if (ids.Any(s => (ushort)s == statusId)) return true;
        }
        return false;
    }

    public static TerritoryType Territory { get; set; }

    public static string TerritoryName => Territory?.PlaceName?.Value?.Name?.RawString ?? "Territory";

    public static ContentFinderCondition ContentFinder => Territory?.ContentFinderCondition?.Value;

    public static string ContentFinderName => ContentFinder?.Name?.RawString ?? "Duty";

    public static bool IsInHighEndDuty => ContentFinder?.HighEndDuty ?? false;
    public static TerritoryContentType TerritoryContentType => (TerritoryContentType)(ContentFinder?.ContentType?.Value?.RowId ?? 0);

    public static AutoStatus AutoStatus { get; private set; } = AutoStatus.None;
    public static bool SetAutoStatus(AutoStatus status, bool keep)
    {
        if (keep)
        {
            AutoStatus |= status;
        }
        else
        {
            AutoStatus &= ~status;
        }
        return keep;
    }
    public static HashSet<uint> DisabledActionSequencer { get; set; } = new HashSet<uint>();

    private static List<NextAct> NextActs = new();
    public static IAction ActionSequencerAction { private get; set; }
    public static IAction CommandNextAction
    {
        get
        {
            if (ActionSequencerAction != null) return ActionSequencerAction;

            var next = NextActs.FirstOrDefault();

            while (next != null && NextActs.Count > 0 && (next.DeadTime < DateTime.Now || IActionHelper.IsLastAction(true, next.Act)))
            {
                NextActs.RemoveAt(0);
                next = NextActs.FirstOrDefault();
            }
            return next?.Act;
        }
    }
    public static Job Job { get; set; }

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
        NextActs = NextActs.OrderBy(i => i.DeadTime).ToList();
    }

    public static TargetHostileType RightNowTargetToHostileType
        => (TargetHostileType)Service.Config.GetValue(Job, Configuration.JobConfigInt.HostileType);

    public static unsafe ActionID LastComboAction => (ActionID)ActionManager.Instance()->Combo.Action;
    public static unsafe float ComboTime => ActionManager.Instance()->Combo.Timer;
    public static TargetingType TargetingType
    {
        get
        {
            if (Service.Config.GlobalConfig.TargetingTypes.Count == 0)
            {
                Service.Config.GlobalConfig.TargetingTypes.Add(TargetingType.Big);
                Service.Config.GlobalConfig.TargetingTypes.Add(TargetingType.Small);
                Service.Config.Save();
            }

            return Service.Config.GlobalConfig.TargetingTypes[Service.Config.GetValue(Configuration.PluginConfigInt.TargetingIndex) % Service.Config.GlobalConfig.TargetingTypes.Count];
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
                if (Service.Config.GetValue(Configuration.PluginConfigBool.ChangeTargetForFate) && (IntPtr)FateManager.Instance() != IntPtr.Zero
                    && (IntPtr)FateManager.Instance()->CurrentFate != IntPtr.Zero
                    && Player.Level <= FateManager.Instance()->CurrentFate->MaxLevel)
                {
                    return FateManager.Instance()->CurrentFate->FateId;
                }
            }
            catch (Exception ex)
            {
                Svc.Log.Error(ex.StackTrace);
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
    public static unsafe float ActionRemain => *(float*)((IntPtr)ActionManager.Instance() + 0x8);

    public static float AbilityRemain
    {
        get
        {
            var gcdRemain = WeaponRemain;
            if (gcdRemain - MinAnimationLock - Ping <= ActionRemain)
            {
                return gcdRemain + MinAnimationLock + Ping;
            }
            return ActionRemain;
        }
    }

    public static float NextAbilityToNextGCD => WeaponRemain - ActionRemain;

    public static float CastingTotal { get; internal set; }
    #endregion
    public static uint[] BluSlots { get; internal set; } = new uint[24];
    public static uint[] DutyActions { get; internal set; } = new uint[2];

    static DateTime _specialStateStartTime = DateTime.MinValue;
    private static double SpecialTimeElapsed => (DateTime.Now - _specialStateStartTime).TotalSeconds;
    public static double SpecialTimeLeft => WeaponTotal == 0 || WeaponElapsed == 0 ? Service.Config.GetValue(PluginConfigFloat.SpecialDuration) - SpecialTimeElapsed :
        Math.Ceiling((Service.Config.GetValue(PluginConfigFloat.SpecialDuration) + WeaponElapsed - SpecialTimeElapsed) / WeaponTotal) * WeaponTotal - WeaponElapsed;

    static SpecialCommandType _specialType = SpecialCommandType.EndSpecial;
    internal static SpecialCommandType SpecialType =>
         SpecialTimeLeft < 0 ? SpecialCommandType.EndSpecial : _specialType;

    public static bool IsHealArea => SpecialType == SpecialCommandType.HealArea || RightSet.HealAreaConditionSet.IsTrue(RightNowRotation);
    public static bool IsHealSingle => SpecialType == SpecialCommandType.HealSingle || RightSet.HealSingleConditionSet.IsTrue(RightNowRotation);
    public static bool IsDefenseArea => SpecialType == SpecialCommandType.DefenseArea || RightSet.DefenseAreaConditionSet.IsTrue(RightNowRotation);
    public static bool IsDefenseSingle => SpecialType == SpecialCommandType.DefenseSingle || RightSet.DefenseSingleConditionSet.IsTrue(RightNowRotation);
    public static bool IsEsunaStanceNorth => SpecialType == SpecialCommandType.EsunaStanceNorth || RightSet.EsunaStanceNorthConditionSet.IsTrue(RightNowRotation);
    public static bool IsRaiseShirk => SpecialType == SpecialCommandType.RaiseShirk || RightSet.RaiseShirkConditionSet.IsTrue(RightNowRotation);
    public static bool IsMoveForward => SpecialType == SpecialCommandType.MoveForward || RightSet.MoveForwardConditionSet.IsTrue(RightNowRotation);
    public static bool IsMoveBack => SpecialType == SpecialCommandType.MoveBack || RightSet.MoveBackConditionSet.IsTrue(RightNowRotation);
    public static bool IsAntiKnockback => SpecialType == SpecialCommandType.AntiKnockback || RightSet.AntiKnockbackConditionSet.IsTrue(RightNowRotation);
    public static bool IsBurst => SpecialType == SpecialCommandType.Burst || Service.Config.GetValue(PluginConfigBool.AutoBurst);
    public static bool IsSpeed => SpecialType == SpecialCommandType.Speed || RightSet.SpeedConditionSet.IsTrue(RightNowRotation);
    public static bool IsLimitBreak => SpecialType == SpecialCommandType.LimitBreak || RightSet.LimitBreakConditionSet.IsTrue(RightNowRotation);

    public static bool State { get; set; } = false;

    public static bool IsManual { get; set; } = false;

    public static void SetSpecialType(SpecialCommandType specialType)
    {
        _specialType = specialType;
        _specialStateStartTime = specialType == SpecialCommandType.EndSpecial ? DateTime.MinValue : DateTime.Now;
    }

    public static bool InCombat { get; set; }

    internal static float CombatTimeRaw { get; set; }

    public static IEnumerable<BattleChara> PartyMembers { get; internal set; } = Array.Empty<PlayerCharacter>();

    public static IEnumerable<BattleChara> PartyTanks { get; internal set; } = Array.Empty<PlayerCharacter>();

    public static IEnumerable<BattleChara> PartyHealers { get; internal set; } = Array.Empty<PlayerCharacter>();

    public static IEnumerable<BattleChara> AllianceMembers { get; internal set; } = Array.Empty<PlayerCharacter>();

    public static IEnumerable<BattleChara> AllianceTanks { get; internal set; } = Array.Empty<PlayerCharacter>();

    public static ObjectListDelay<BattleChara> DeathPeopleAll { get; } = new(
    () => (Service.Config.GetValue(PluginConfigFloat.DeathDelayMin),
    Service.Config.GetValue(PluginConfigFloat.DeathDelayMax)));

    public static ObjectListDelay<BattleChara> DeathPeopleParty { get; } = new(
    () => (Service.Config.GetValue(PluginConfigFloat.DeathDelayMin),
    Service.Config.GetValue(PluginConfigFloat.DeathDelayMax)));

    public static ObjectListDelay<BattleChara> WeakenPeople { get; } = new(
    () => (Service.Config.GetValue(PluginConfigFloat.WeakenDelayMin),
    Service.Config.GetValue(PluginConfigFloat.WeakenDelayMax)));

    public static IEnumerable<BattleChara> DyingPeople { get; internal set; } = Array.Empty<BattleChara>();

    public static ObjectListDelay<BattleChara> HostileTargets { get; } = new ObjectListDelay<BattleChara>(
    () => (Service.Config.GetValue(PluginConfigFloat.HostileDelayMin),
    Service.Config.GetValue(PluginConfigFloat.HostileDelayMax)));

    public static IEnumerable<BattleChara> AllHostileTargets { get; internal set; } = Array.Empty<BattleChara>();

    public static IEnumerable<BattleChara> TarOnMeTargets { get; internal set; } = Array.Empty<BattleChara>();

    public static ObjectListDelay<BattleChara> CanInterruptTargets { get; } = new ObjectListDelay<BattleChara>(
    () => (Service.Config.GetValue(PluginConfigFloat.InterruptDelayMin),
    Service.Config.GetValue(PluginConfigFloat.InterruptDelayMax)));

    public static IEnumerable<GameObject> AllTargets { get; set; }

    public static bool CanProvoke { get; set; } = false;

    public static uint[] TreasureCharas { get; internal set; } = Array.Empty<uint>();
    public static bool HasHostilesInRange => NumberOfHostilesInRange > 0;
    public static bool HasHostilesInMaxRange => NumberOfHostilesInMaxRange > 0;
    public static int NumberOfHostilesInRange { get; internal set; }
    public static int NumberOfHostilesInMaxRange { get; internal set; }
    public static int NumberOfAllHostilesInRange { get; internal set; }
    public static int NumberOfAllHostilesInMaxRange { get; internal set; }
    public static bool MobsTime { get; internal set; }
    public static float AverageTimeToKill { get; internal set; }

    public static bool IsHostileCastingAOE { get; internal set; }

    public static bool IsHostileCastingToTank { get; internal set; }

    public static bool HasPet { get; internal set; }

    public static unsafe bool HasCompanion => (IntPtr)Player.BattleChara != IntPtr.Zero
                                           && (IntPtr)CharacterManager.Instance()->LookupBuddyByOwnerObject(Player.BattleChara) != IntPtr.Zero;

    public static float RatioOfMembersIn2minsBurst
    {
        get
        {
            byte burst = 0, count = 0;

            foreach (var member in PartyMembers)
            {
                foreach (var burstInfo in StatusHelper.Burst2Mins)
                {
                    if (burstInfo.Jobs.Contains((Job)member.ClassJob.Id))
                    {
                        if (member.Level >= burstInfo.Level)
                        {
                            var tar = burstInfo.IsOnHostile
                                && Svc.Targets.Target is BattleChara b ? b
                                : Player.Object;
                            if (tar.HasStatus(false, burstInfo.Status)
                                && !tar.WillStatusEndGCD(0, 0, false, burstInfo.Status))
                            {
                                burst++;
                            }
                            count++;
                        }
                        break;
                    }
                }
            }
            if (count == 0) return -1;
            return (float)burst / count;
        }
    }

    #region HP
    public static Dictionary<uint, float> RefinedHP { get; internal set; } = new Dictionary<uint, float>();

    public static IEnumerable<float> PartyMembersHP { get; internal set; }
    public static float PartyMembersMinHP { get; internal set; }
    public static float PartyMembersAverHP { get; internal set; }
    public static float PartyMembersDifferHP { get; internal set; }

    public static bool HPNotFull { get; internal set; }

    public static bool CanHealAreaAbility { get; internal set; }

    public static bool CanHealAreaSpell { get; internal set; }

    public static bool CanHealSingleAbility { get; internal set; }

    public static bool CanHealSingleSpell { get; internal set; }

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
    public static float Ping => Math.Min(Math.Min(RTT, FetchTime), Service.Config.GetValue(Configuration.PluginConfigFloat.MaxPing));
    public static float RTT { get; internal set; } = 0.1f;
    public static float FetchTime { get; private set; } = 0.1f;


    public const float MinAnimationLock = 0.6f;
    internal static unsafe void AddActionRec(Action act)
    {
        if (!Player.Available) return;

        var id = (ActionID)act.RowId;

        //Record
        switch (act.GetActionCate())
        {
            case ActionCate.Spell:
            case ActionCate.WeaponSkill:
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
}
