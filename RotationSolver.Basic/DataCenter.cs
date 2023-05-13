using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Fate;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using Lumina.Excel.GeneratedSheets;
using Action = Lumina.Excel.GeneratedSheets.Action;
using CharacterManager = FFXIVClientStructs.FFXIV.Client.Game.Character.CharacterManager;

namespace RotationSolver.Basic;

public static class DataCenter
{
    public static DateTime EffectTime { private get; set; } = DateTime.Now;
    public static DateTime EffectEndTime { private get; set; } = DateTime.Now;

    public static bool InEffectTime => DateTime.Now >= EffectTime && DateTime.Now <= EffectEndTime;
    public static Dictionary<uint, ushort> HealHP { get; set; } = new Dictionary<uint, ushort>();
    public static Dictionary<uint, ushort> ApplyStatus { private get; set; } = new Dictionary<uint, ushort>();
    public static uint MPGain { get; set; }
    public static bool HasApplyStatus(uint id, StatusID[] ids)
    {
        if (InEffectTime && ApplyStatus.TryGetValue(id, out var statusId))
        {
            if (ids.Any(s => (ushort)s == statusId)) return true;
        }
        return false;
    }
    public static bool InHighEndDuty { get; set; } = false;
    public static TerritoryContentType TerritoryContentType { get; set; } = TerritoryContentType.None;

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
    public static HashSet<uint> DisabledAction { get; set; } = new HashSet<uint>();

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

    public static void AddCommandAction(IAction act, double time)
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
    {
        get
        {
            if (Service.Player == null) return 0;
            var id = Service.Player.ClassJob.Id;
            return GetTargetHostileType(Service.GetSheet<ClassJob>().GetRow(id));
        }
    }

    public static TargetHostileType GetTargetHostileType(ClassJob classJob)
    {
        if (Service.Config.TargetToHostileTypes.TryGetValue(classJob.RowId, out var type))
        {
            return (TargetHostileType)type;
        }

        return classJob.GetJobRole() == JobRole.Tank ? TargetHostileType.AllTargetsCanAttack : TargetHostileType.TargetsHaveTarget;
    }

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

            return Service.Config.TargetingTypes[Service.Config.TargetingIndex %= Service.Config.TargetingTypes.Count];
        }
    }

    public unsafe static bool IsMoving => AgentMap.Instance()->IsPlayerMoving > 0;

    public static unsafe ushort FateId
    {
        get
        {
            try
            {
                if (Service.Config.ChangeTargetForFate && (IntPtr)FateManager.Instance() != IntPtr.Zero
                    && (IntPtr)FateManager.Instance()->CurrentFate != IntPtr.Zero
                    && Service.Player.Level <= FateManager.Instance()->CurrentFate->MaxLevel)
                {
                    return FateManager.Instance()->CurrentFate->FateId;
                }
            }
            catch (Exception ex)
            {
                PluginLog.Error(ex.StackTrace);
            }
            return 0;
        }
    }

    #region GCD
    public static float WeaponRemain { get; set; }

    public static float WeaponTotal { get; set; }

    public static float WeaponElapsed { get; set; }

    /// <summary>
    /// Time to the next action
    /// </summary>
    public static unsafe float ActionRemain => *(float*)((IntPtr)ActionManager.Instance() + 0x8);

    public static float AbilityRemain
    {
        get
        {
            var gcdRemain = WeaponRemain;
            if ((gcdRemain - MinAnimationLock - Ping).IsLessThan(ActionRemain))
            {
                return gcdRemain + MinAnimationLock + Ping;
            }
            return ActionRemain;
        }
    }

    public static float NextAbilityToNextGCD => WeaponRemain - ActionRemain;

    public static float CastingTotal { get; set; }
    #endregion
    public static uint[] BluSlots { get; set; } = new uint[24];

    static DateTime _specialStateStartTime = DateTime.MinValue;
    private static double SpecialTimeElapsed => (DateTime.Now - _specialStateStartTime).TotalSeconds;
    public static double SpecialTimeLeft => WeaponTotal == 0 || WeaponElapsed == 0 ? Service.Config.SpecialDuration - SpecialTimeElapsed :
        Math.Ceiling((Service.Config.SpecialDuration + WeaponElapsed - SpecialTimeElapsed) / WeaponTotal) * WeaponTotal - WeaponElapsed;

    static SpecialCommandType _specialType = SpecialCommandType.EndSpecial;
    public static SpecialCommandType SpecialType =>
         SpecialTimeLeft < 0 ? SpecialCommandType.EndSpecial : _specialType;
    public static StateCommandType StateType { get; set; } = StateCommandType.Cancel;

    public static void SetSpecialType(SpecialCommandType specialType)
    {
        _specialType = specialType;
        _specialStateStartTime = specialType == SpecialCommandType.EndSpecial ? DateTime.MinValue : DateTime.Now;
    }

    public static bool InCombat { get; set; }

    public static float CombatTime { get; set; }

    public static IEnumerable<BattleChara> PartyMembers { get; set; } = Array.Empty<PlayerCharacter>();

    public static IEnumerable<BattleChara> PartyTanks { get; set; } = Array.Empty<PlayerCharacter>();

    public static IEnumerable<BattleChara> PartyHealers { get; set; } = Array.Empty<PlayerCharacter>();

    public static IEnumerable<BattleChara> AllianceMembers { get; set; } = Array.Empty<PlayerCharacter>();

    public static IEnumerable<BattleChara> AllianceTanks { get; set; } = Array.Empty<PlayerCharacter>();

    public static ObjectListDelay<BattleChara> DeathPeopleAll { get; } = new(
    () => (Service.Config.DeathDelayMin, Service.Config.DeathDelayMax));

    public static ObjectListDelay<BattleChara> DeathPeopleParty { get; } = new(
        () => (Service.Config.DeathDelayMin, Service.Config.DeathDelayMax));

    public static ObjectListDelay<BattleChara> WeakenPeople { get; } = new(
        () => (Service.Config.WeakenDelayMin, Service.Config.WeakenDelayMax));

    public static ObjectListDelay<BattleChara> DyingPeople { get; } = new(
        () => (Service.Config.WeakenDelayMin, Service.Config.WeakenDelayMax));

    public static ObjectListDelay<BattleChara> HostileTargets { get; } = new ObjectListDelay<BattleChara>(
    () => (Service.Config.HostileDelayMin, Service.Config.HostileDelayMax));

    public static IEnumerable<BattleChara> AllHostileTargets { get; set; } = Array.Empty<BattleChara>();

    public static IEnumerable<BattleChara> TarOnMeTargets { get; set; } = Array.Empty<BattleChara>();

    public static ObjectListDelay<BattleChara> CanInterruptTargets { get; } = new ObjectListDelay<BattleChara>(
        () => (Service.Config.InterruptDelayMin, Service.Config.InterruptDelayMax));

    public static IEnumerable<GameObject> AllTargets { get; set; }

    public static uint[] TreasureCharas { get; set; } = Array.Empty<uint>();
    public static bool HasHostilesInRange => NumberOfHostilesInRange > 0;
    public static bool HasHostilesInMaxRange => NumberOfHostilesInMaxRange > 0;
    public static int NumberOfHostilesInRange { get; set; }
    public static int NumberOfHostilesInMaxRange { get; set; }

    public static bool IsHostileCastingAOE { get; set; }

    public static bool IsHostileCastingToTank { get; set; }

    public static bool HasPet { get; set; }


    public static unsafe bool HasCompanion => (IntPtr)Service.RawPlayer != IntPtr.Zero 
                                           && (IntPtr)CharacterManager.Instance()->LookupBuddyByOwnerObject(Service.RawPlayer) != IntPtr.Zero;

    public static float RatioOfMembersIn2minsBurst
    {
        get
        {
            byte burst = 0, count = 0;

            foreach (var member in PartyMembers)
            {
                foreach (var burstInfo in StatusHelper.Burst2Mins)
                {
                    if (burstInfo.Jobs.Contains((ClassJobID)member.ClassJob.Id))
                    {
                        if (member.Level >= burstInfo.Level)
                        {
                            var tar = burstInfo.IsOnHostile 
                                && Service.TargetManager.Target is BattleChara b ? b 
                                : Service.Player;
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
    public static Dictionary<uint, float> RefinedHP { get; set; } = new Dictionary<uint, float>();

    public static IEnumerable<float> PartyMembersHP { get; set; }
    public static float PartyMembersMinHP { get; set; }
    public static float PartyMembersAverHP { get; set; }
    public static float PartyMembersDifferHP { get; set; }

    public static bool HPNotFull { get; set; }

    public static bool CanHealAreaAbility { get; set; }

    public static bool CanHealAreaSpell { get; set; }

    public static bool CanHealSingleAbility { get; set; }

    public static bool CanHealSingleSpell { get; set; }

    public static uint CurrentMp { get; set; }
    #endregion
    public static Queue<MacroItem> Macros { get; } = new Queue<MacroItem>();

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
    public static float Ping => Math.Min(RTT, FetchTime);
    public static float RTT { get; set; } = 0.1f;
    public static float FetchTime { get; set; } = 0.1f;


    public const float MinAnimationLock = 0.6f;
    public static unsafe void AddActionRec(Action act)
    {
        var id = (ActionID)act.RowId;

        //Record
        switch (act.GetActionType())
        {
            case ActionCate.Spell:
            case ActionCate.WeaponSkill:
                LastAction = LastGCD = id;
                if (ActionManager.GetAdjustedCastTime(ActionType.Spell, (uint)id) == 0)
                {
                    FetchTime = WeaponElapsed;
                }
                break;
            case ActionCate.Ability:
                LastAction = LastAbility = id;

                if (!act.IsRealGCD() && ActionManager.GetMaxCharges((uint)id, Service.Player.Level) < 2)
                {
                    FetchTime = ActionManager.Instance()->GetRecastGroupDetail(act.CooldownGroup - 1)->Elapsed;
                }
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

    public static void AddDamageRec(float damageRatio)
    {
        if (_damages.Count >= QUEUECAPACITY)
        {
            _damages.Dequeue();
        }
        _damages.Enqueue(new DamageRec(DateTime.Now, damageRatio));
    }
    #endregion
}
