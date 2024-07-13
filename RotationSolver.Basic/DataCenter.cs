using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.Enums;
using ECommons.DalamudServices;
using ECommons.ExcelServices;
using ECommons.GameFunctions;
using ECommons.GameHelpers;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Fate;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using Lumina.Excel.GeneratedSheets;
using RotationSolver.Basic.Configuration;
using RotationSolver.Basic.Configuration.Conditions;
using RotationSolver.Basic.Rotations.Duties;
using Action = Lumina.Excel.GeneratedSheets.Action;
using CharacterManager = FFXIVClientStructs.FFXIV.Client.Game.Character.CharacterManager;

namespace RotationSolver.Basic;

internal static class DataCenter
{
    private static ulong _hostileTargetId = 0;

    public static bool IsActivated() => State || IsManual || Service.Config.TeachingMode;

    internal static IBattleChara? HostileTarget
    {
        get => Svc.Objects.SearchById(_hostileTargetId) as IBattleChara;
        set => _hostileTargetId = value?.GameObjectId ?? 0;
    }

    internal static List<uint> PrioritizedNameIds { get; set; } = new();
    internal static List<uint> BlacklistedNameIds { get; set; } = new();

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

    internal static Queue<(DateTime time, SortedList<ulong, float> hpRatios)> RecordedHP { get; } =
        new(HP_RECORD_TIME + 1);

    public static ICustomRotation? RightNowRotation { get; internal set; }
    public static DutyRotation? RightNowDutyRotation { get; internal set; }

    public static Dictionary<string, DateTime> SystemWarnings { get; set; } = new();

    internal static bool NoPoslock => Svc.Condition[ConditionFlag.OccupiedInEvent]
                                      || !Service.Config.PoslockCasting
                                      //Key cancel.
                                      || Svc.KeyState[Service.Config.PoslockModifier.ToVirtual()]
                                      //Gamepad cancel.
                                      || Svc.GamepadState.Raw(Dalamud.Game.ClientState.GamePad.GamepadButtons.R1) >=
                                      0.5f;

    internal static DateTime EffectTime { private get; set; } = DateTime.Now;
    internal static DateTime EffectEndTime { private get; set; } = DateTime.Now;

    internal const int ATTACKED_TARGETS_COUNT = 48;
    internal static Queue<(ulong id, DateTime time)> AttackedTargets { get; } = new(ATTACKED_TARGETS_COUNT);

    internal static bool InEffectTime => DateTime.Now >= EffectTime && DateTime.Now <= EffectEndTime;
    internal static Dictionary<ulong, uint> HealHP { get; set; } = [];
    internal static Dictionary<ulong, uint> ApplyStatus { get; set; } = [];
    internal static uint MPGain { get; set; }

    internal static bool HasApplyStatus(ulong id, StatusID[] ids)
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

    public static TerritoryContentType TerritoryContentType =>
        (TerritoryContentType)(ContentFinder?.ContentType?.Value?.RowId ?? 0);

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

            while (next != null && NextActs.Count > 0 &&
                   (next.DeadTime < DateTime.Now || IActionHelper.IsLastAction(true, next.Act)))
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
                Service.Config.TargetingTypes.Add(TargetingType.LowHP);
                Service.Config.TargetingTypes.Add(TargetingType.HighHP);
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

    public static float DefaultGCDTotal => ActionManagerHelper.GetDefaultRecastTime();

    public static float DefaultGCDRemain =>
        ActionManagerHelper.GetDefaultRecastTime() - ActionManagerHelper.GetDefaultRecastTimeElapsed();

    public static float DefaultGCDElapsed => ActionManagerHelper.GetDefaultRecastTimeElapsed();

    public static float ActionAhead =>
        Service.Config.OverrideActionAheadTimer ? Service.Config.Action4Head : CalculatedActionAhead;

    public static float CalculatedActionAhead => DefaultGCDTotal * 0.25f;

    public static float GCDTime(uint gcdCount = 0, float offset = 0)
        => ActionManagerHelper.GetDefaultRecastTime() * gcdCount + offset;

    #endregion


    public static uint[] BluSlots { get; internal set; } = new uint[24];

    public static uint[] DutyActions { get; internal set; } = new uint[2];

    static DateTime _specialStateStartTime = DateTime.MinValue;
    private static double SpecialTimeElapsed => (DateTime.Now - _specialStateStartTime).TotalSeconds;
    public static double SpecialTimeLeft => Service.Config.SpecialDuration - SpecialTimeElapsed;

    static SpecialCommandType _specialType = SpecialCommandType.EndSpecial;

    internal static SpecialCommandType SpecialType
    {
        get => SpecialTimeLeft < 0 ? SpecialCommandType.EndSpecial : _specialType;
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

    public unsafe static IBattleChara[] PartyMembers => AllianceMembers.Where(ObjectHelper.IsParty)
        .Where(b => b.Character()->CharacterData.OnlineStatus != 15 && b.IsTargetable).ToArray();

    public unsafe static IBattleChara[] AllianceMembers => AllTargets.Where(ObjectHelper.IsAlliance)
        .Where(b => b.Character()->CharacterData.OnlineStatus != 15 && b.IsTargetable).ToArray();

    public static IBattleChara[] AllHostileTargets
    {
        get
        {
            return AllTargets.Where(b =>
            {
                if (!b.IsEnemy()) return false;

                //Dead.
                if (b.CurrentHp <= 1) return false;

                if (!b.IsTargetable) return false;
                
                if (b.StatusList.Any(StatusHelper.IsInvincible)) return false;
                return true;
            }).ToArray();
        }
    }

    public static IBattleChara? InterruptTarget =>
        AllHostileTargets.FirstOrDefault(ObjectHelper.CanInterrupt);

    public static IBattleChara? ProvokeTarget => AllHostileTargets.FirstOrDefault(ObjectHelper.CanProvoke);

    public static IBattleChara? DeathTarget
    {
        get
        {
            var deathAll = AllianceMembers.GetDeath();
            var deathParty = PartyMembers.GetDeath();
            
            if (deathParty.Any())
            {
                var deathT = deathParty.GetJobCategory(JobRole.Tank);

                if (deathT.Count() > 1)
                {
                    return deathT.FirstOrDefault();
                }

                var deathH = deathParty.GetJobCategory(JobRole.Healer);

                if (deathH.Any()) return deathH.FirstOrDefault();

                if (deathT.Any()) return deathT.FirstOrDefault();

                return deathParty.FirstOrDefault();
            }

            if (deathAll.Any() && Service.Config.RaiseAll)
            {
                var deathAllH = deathAll.GetJobCategory(JobRole.Healer);
                if (deathAllH.Any()) return deathAllH.FirstOrDefault();

                var deathAllT = deathAll.GetJobCategory(JobRole.Tank);
                if (deathAllT.Any()) return deathAllT.FirstOrDefault();

                return deathAll.FirstOrDefault();
            }

            return null;

        }
    }

    public static IBattleChara? DispelTarget
    {
        get
        {
            var weakenPeople = DataCenter.PartyMembers.Where(o => o is IBattleChara b && b.StatusList.Any(StatusHelper.CanDispel));
            var dyingPeople = weakenPeople.Where(o => o is IBattleChara b && b.StatusList.Any(StatusHelper.IsDangerous));

            return dyingPeople.OrderBy(ObjectHelper.DistanceToPlayer).FirstOrDefault()
                                      ?? weakenPeople.OrderBy(ObjectHelper.DistanceToPlayer).FirstOrDefault();
        }
    }

    public static IBattleChara[] AllTargets => Svc.Objects.OfType<IBattleChara>().GetObjectInRadius(30)
        .Where(o => !o.IsDummy() || !Service.Config.DisableTargetDummys).ToArray();

    public static ulong[] TreasureCharas
    {
        get
        {
            List<ulong> charas = new(5);
            //60687 - 60691 For treasure hunt.
            for (int i = 60687; i <= 60691; i++)
            {
                var b = AllTargets.FirstOrDefault(obj => obj.GetNamePlateIcon() == i);
                if (b == null || b.CurrentHp == 0) continue;
                charas.Add(b.GameObjectId);
            }

            return charas.ToArray();
        }
    }

    public static bool HasHostilesInRange => NumberOfHostilesInRange > 0;
    public static bool HasHostilesInMaxRange => NumberOfHostilesInMaxRange > 0;
    public static int NumberOfHostilesInRange => AllHostileTargets.Count(o => o.DistanceToPlayer() <= JobRange);
    public static int NumberOfHostilesInMaxRange => AllHostileTargets.Count(o => o.DistanceToPlayer() <= 25);
    public static int NumberOfAllHostilesInRange => AllHostileTargets.Count(o => o.DistanceToPlayer() <= JobRange);
    public static int NumberOfAllHostilesInMaxRange => AllHostileTargets.Count(o => o.DistanceToPlayer() <= 25);
    public static bool MobsTime => AllHostileTargets.Count(o => o.DistanceToPlayer() <= JobRange && o.CanSee())
                                   >= Service.Config.AutoDefenseNumber;

    public static bool AreHostilesCastingKnockback => AllHostileTargets.Any(IsHostileCastingKnockback);
    
    public static float JobRange
    {
        get
        {
            float radius = 25;
            if (!Player.Available) return radius;
           
            switch (DataCenter.Role)
            {
                case JobRole.Tank:
                case JobRole.Melee:
                    radius = 3;
                    break;
            }
            return radius;
        }
    }

    public static float AverageTimeToKill
    {
        get
        {
            var validTimes = AllHostileTargets.Select(b => b.GetTimeToKill()).Where(v => !float.IsNaN(v)).ToList();
            return validTimes.Any() ? validTimes.Average() : 0; 
        }
    }

    public static bool IsHostileCastingAOE => IsCastingAreaVfx() || AllHostileTargets.Any(IsHostileCastingArea);

    public static bool IsHostileCastingToTank => IsCastingTankVfx() || AllHostileTargets.Any(IsHostileCastingTank);

    private static DateTime _petLastSeen = DateTime.MinValue;

    public static bool HasPet
    {
        get
        {
            var mayPet = AllTargets.OfType<IBattleNpc>().Where(npc => npc.OwnerId == Player.Object.GameObjectId);
            var hasPet = mayPet.Any(npc => npc.BattleNpcKind == BattleNpcSubKind.Pet);
            if (hasPet)
            {
                _petLastSeen = DateTime.Now;
                return true;
            }
            else if (!hasPet && _petLastSeen.AddSeconds(1) < DateTime.Now)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    public static unsafe bool HasCompanion => (IntPtr)Player.BattleChara != IntPtr.Zero
                                              && (IntPtr)CharacterManager.Instance()->LookupBuddyByOwnerObject(
                                                  Player.BattleChara) != IntPtr.Zero;

    #region HP

    public static Dictionary<ulong, float> RefinedHP => PartyMembers
        .ToDictionary(p => p.GameObjectId, GetPartyMemberHPRatio);
    
    private static Dictionary<ulong, uint> _lastHp = [];
    private static float GetPartyMemberHPRatio(IBattleChara member)
    {
        if (member == null) return 0;

        if (!DataCenter.InEffectTime
            || !DataCenter.HealHP.TryGetValue(member.GameObjectId, out var hp))
        {
            return (float)member.CurrentHp / member.MaxHp;
        }

        var rightHp = member.CurrentHp;
        if (rightHp > 0)
        {
            if (!_lastHp.TryGetValue(member.GameObjectId, out var lastHp)) lastHp = rightHp;

            if (rightHp - lastHp == hp)
            {
                DataCenter.HealHP.Remove(member.GameObjectId);
                return (float)member.CurrentHp / member.MaxHp;
            }
            return Math.Min(1, (hp + rightHp) / (float)member.MaxHp);
        }
        return (float)member.CurrentHp / member.MaxHp;
    }

    public static IEnumerable<float> PartyMembersHP => RefinedHP.Values.Where(r => r > 0);
    public static float PartyMembersMinHP => PartyMembersHP.Any() ? PartyMembersHP.Min() : 0;
    public static float PartyMembersAverHP => PartyMembersHP.Any() ? PartyMembersHP.Average() : 0;
    public static float PartyMembersDifferHP => PartyMembersHP.Any() ? (float)Math.Sqrt(PartyMembersHP.Average(d => Math.Pow(d - PartyMembersAverHP, 2))) : 0;

    public static bool HPNotFull => PartyMembersMinHP < 1;

    public static uint CurrentMp => Math.Min(10000, Player.Object.CurrentMp + MPGain);

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
                break;
            case ActionCate.Ability:
                LastAction = LastAbility = id;
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
    
        private static bool IsCastingTankVfx()
    {
        return IsCastingVfx(s =>
        {
            if (!s.Path.StartsWith("vfx/lockon/eff/tank_lockon")) return false;
            if (!Player.Available) return false;
            if (Player.Object.IsJobCategory(JobRole.Tank) && s.ObjectId != Player.Object.GameObjectId) return false;

            return true;
        });
    }
    private static bool IsCastingAreaVfx()
    {
        return IsCastingVfx(s => s.Path.StartsWith("vfx/lockon/eff/coshare"));
    }

    private static bool IsCastingVfx(Func<VfxNewData, bool> isVfx)
    {
        if (isVfx == null) return false;
        try
        {
            foreach (var item in DataCenter.VfxNewData.Reverse())
            {
                if (item.TimeDuration.TotalSeconds is > 1 and < 5)
                {
                    if (isVfx(item)) return true;
                }
            }
        }
        catch
        {

        }

        return false;
    }

    private static bool IsHostileCastingTank(IBattleChara h)
    {
        return IsHostileCastingBase(h, (act) =>
        {
            return OtherConfiguration.HostileCastingTank.Contains(act.RowId)
                || h.CastTargetObjectId == h.TargetObjectId;
        });
    }

    private static bool IsHostileCastingArea(IBattleChara h)
    {
        return IsHostileCastingBase(h, (act) =>
        {
            return OtherConfiguration.HostileCastingArea.Contains(act.RowId);
        });
    }

    public static bool IsHostileCastingKnockback(IBattleChara h)
    {
        return IsHostileCastingBase(h, (act) =>
        {
            return OtherConfiguration.HostileCastingKnockback.Contains(act.RowId);
        });
    }

    private static bool IsHostileCastingBase(IBattleChara h, Func<Action, bool> check)
    {
        if (!h.IsCasting) return false;

        if (h.IsCastInterruptible) return false;
        var last = h.TotalCastTime - h.CurrentCastTime;
        var t = last - DataCenter.DefaultGCDTotal;

        if (!(h.TotalCastTime > 2.5 &&
            t > 0 && t < DataCenter.GCDTime(2))) return false;

        var action = Service.GetSheet<Action>().GetRow(h.CastActionId);
        if (action == null) return false;
        return check?.Invoke(action) ?? false;
    }

}
