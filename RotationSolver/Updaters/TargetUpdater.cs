using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.SubKinds;
using ECommons.DalamudServices;
using ECommons.GameFunctions;
using ECommons.GameHelpers;
using RotationSolver.Basic.Configuration;
using RotationSolver.Basic.Record;
using Action = Lumina.Excel.GeneratedSheets.Action;

namespace RotationSolver.Updaters;

internal static partial class TargetUpdater
{
    static readonly ObjectListDelay<IBattleChara> 
        _raisePartyTargets = new(() => Service.Config.RaiseDelay),
        _raiseAllTargets = new(() => Service.Config.RaiseDelay);

    internal unsafe static void UpdateTarget()
    {
        var battles = Svc.Objects.OfType<IBattleChara>();
        DataCenter.AllTargets.Delay(GetLessTargets(battles.GetObjectInRadius(30)));
        UpdateHostileTargets(DataCenter.AllTargets);
        UpdateFriends(DataCenter.AllTargets
            .Where(b => b.Character()->CharacterData.OnlineStatus != 15 //Removed the one watching cutscene.
            && b.IsTargetable //Removed the one can't target.
            ));
        UpdateNamePlate(battles);

        if (Service.Config.MakeSoftTargetAsTarget && Svc.Targets.Target == null && Svc.Targets.SoftTarget != null)
        {
            Svc.Targets.Target = Svc.Targets.SoftTarget;
        }
    }

    internal static IEnumerable<IBattleChara> GetLessTargets(IEnumerable<IBattleChara> battles)
    {
        var removedPlayers = battles.OfType<IPlayerCharacter>().OrderBy(p => p.DistanceToPlayer()).Skip(30);
        return battles.Where(b => !removedPlayers.Contains(b));
    }

    private static DateTime _lastUpdateTimeToKill = DateTime.MinValue;
    private static readonly TimeSpan _timeToKillSpan = TimeSpan.FromSeconds(0.5);
    private static void UpdateTimeToKill(IEnumerable<IBattleChara> allTargets)
    {
        var now = DateTime.Now;
        if (now - _lastUpdateTimeToKill < _timeToKillSpan) return;
        _lastUpdateTimeToKill = now;

        if (DataCenter.RecordedHP.Count >= DataCenter.HP_RECORD_TIME)
        {
            DataCenter.RecordedHP.Dequeue();
        }

        DataCenter.RecordedHP.Enqueue((now, new SortedList<uint, float>(allTargets.Where(b => b != null && b.CurrentHp != 0).ToDictionary(b => b.EntityId, b => b.GetHealthRatio()))));
    }

    internal static void ClearTarget()
    {
        var empty = Array.Empty<IBattleChara>();
        DataCenter.AllHostileTargets
        = DataCenter.PartyMembers
        = DataCenter.AllianceMembers
        = empty;

        DataCenter.InterruptTarget = DataCenter.ProvokeTarget = null;
        DataCenter.AllTargets.Delay(empty);
    }

    #region Hostile
    private static float JobRange
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

    private static RandomDelay
        _interruptDelay = new(() => Service.Config.InterruptDelay),
        _knockbackDelay = new(() => Service.Config.AntiKnockbackDelay),
        _defenseMeDelay = new(() => Service.Config.DefenseSingleDelay),
        _defenseSingleDelay = new(() => Service.Config.DefenseSingleDelay),
        _defenseAreaDelay = new(() => Service.Config.DefenseAreaDelay),
        _provokeDelay = new(() => Service.Config.ProvokeDelay);

    private unsafe static void UpdateHostileTargets(IEnumerable<IBattleChara> allTargets)
    {
        allTargets = allTargets.Where(b =>
        {
            if (!b.IsEnemy()) return false;

            //Dead.
            if (b.CurrentHp <= 1) return false;

            if (!b.IsTargetable) return false;

            return true;
        });

        UpdateTimeToKill(allTargets);

        DataCenter.AllHostileTargets = allTargets.Where(b =>
        {
            if (b.StatusList.Any(StatusHelper.IsInvincible)) return false;

            if (b is IPlayerCharacter p)
            {
                var hash = p.EncryptString();

                //Don't attack authors!!
                if (DataCenter.AuthorHashes.ContainsKey(hash)) return false;

                //Don't attack contributors!!
                if (DownloadHelper.ContributorsHash.Contains(hash)) return false;
            }
            return true;
        }).ToArray();

        var timesToKill = DataCenter.AllHostileTargets.Select(b => b.GetTimeToKill()).Where(v => !float.IsNaN(v));
        DataCenter.AverageTimeToKill = timesToKill.Any() ? timesToKill.Average() : 0;

        DataCenter.InterruptTarget = _interruptDelay.Delay(DataCenter.AllHostileTargets.FirstOrDefault(ObjectHelper.CanInterrupt));

        DataCenter.NumberOfHostilesInRange = DataCenter.AllHostileTargets.Count(o => o.DistanceToPlayer() <= JobRange);

        DataCenter.NumberOfHostilesInMaxRange = DataCenter.AllHostileTargets.Count(o => o.DistanceToPlayer() <= 25);

        DataCenter.NumberOfAllHostilesInRange = DataCenter.AllHostileTargets.Count(o => o.DistanceToPlayer() <= JobRange);

        DataCenter.NumberOfAllHostilesInMaxRange = DataCenter.AllHostileTargets.Count(o => o.DistanceToPlayer() <= 25);

        DataCenter.MobsTime = DataCenter.AllHostileTargets.Count(o => o.DistanceToPlayer() <= JobRange && o.CanSee())
            >= Service.Config.AutoDefenseNumber;

        DataCenter.IsHostileCastingToMe = _defenseMeDelay.Delay(IsCastingTankVfx(true) || DataCenter.AllHostileTargets.Any(t => IsHostileCastingTank(t, true)));
        DataCenter.IsHostileCastingToOthers = _defenseSingleDelay.Delay(IsCastingTankVfx(false) || DataCenter.AllHostileTargets.Any(t => IsHostileCastingTank(t, false)));

        DataCenter.IsHostileCastingAOE = _defenseAreaDelay.Delay(IsCastingAreaVfx() || DataCenter.AllHostileTargets.Any(IsHostileCastingArea));
        DataCenter.IsHostileCastingKnockback = _knockbackDelay.Delay(DataCenter.AllHostileTargets.Any(IsHostileCastingKnockback));

        DataCenter.ProvokeTarget = _provokeDelay.Delay(DataCenter.AllHostileTargets.FirstOrDefault(ObjectHelper.CanProvoke));
    }

    private static bool IsCastingTankVfx(bool isMe)
    {
        return IsCastingVfx(s =>
        {
            if (!s.Path.StartsWith("vfx/lockon/eff/tank_lockon")) return false;
            if (!Player.Available) return false;

            if (isMe)
            {
                if (s.Object?.GameObjectId != Player.Object.GameObjectId) return false;
            }
            else
            {
                if (s.Object?.GameObjectId == Player.Object.GameObjectId) return false;
            }

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
            foreach (var item in Recorder.GetData<VfxNewData>(1, 5))
            {
                if (isVfx(item)) return true;
            }
        }
        catch
        {
        }

        return false;
    }

    private static bool IsHostileCastingTank(IBattleChara h, bool isMe)
    {
        return IsHostileCastingBase(h, (act) =>
        {
            if (!Player.Available) return false;

            if (isMe)
            {
                if (h.CastTargetObjectId != Player.Object.GameObjectId) return false;
            }
            else
            {
                if (h.CastTargetObjectId == Player.Object.GameObjectId) return false;
            }

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

    private static bool IsHostileCastingKnockback(IBattleChara h)
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
        var t = last - DataCenter.WeaponRemain;

        if (!(h.TotalCastTime > 2.5 && t is > 0 and < 5)) return false;

        var action = Service.GetSheet<Action>().GetRow(h.CastActionId);
        if (action == null) return false;
        return check?.Invoke(action) ?? false;
    }
    #endregion

    #region Friends
    private static Dictionary<uint, uint> _lastHp = [];
    private static uint _lastMp = 0;
    private unsafe static void UpdateFriends(IEnumerable<IBattleChara> allTargets)
    {
        DataCenter.AllianceMembers = allTargets.Where(ObjectHelper.IsAlliance).ToArray();
        DataCenter.PartyMembers = DataCenter.AllianceMembers.Where(ObjectHelper.IsParty).ToArray();

        var mayPet = allTargets.OfType<IBattleNpc>().Where(npc => npc.OwnerId == Player.Object.EntityId);
        DataCenter.HasPet = mayPet.Any(npc => npc.BattleNpcKind == BattleNpcSubKind.Pet);

        _raiseAllTargets.Delay(DataCenter.AllianceMembers.GetDeath());
        _raisePartyTargets.Delay(DataCenter.PartyMembers.GetDeath());
        DataCenter.DeathTarget = GetDeathTarget(_raiseAllTargets, _raisePartyTargets);

        var weakenPeople = DataCenter.PartyMembers.Where(o => o is IBattleChara b && b.StatusList.Any(StatusHelper.CanDispel));
        var dyingPeople = weakenPeople.Where(o => o is IBattleChara b && b.StatusList.Any(StatusHelper.IsDangerous));

        DataCenter.DispelTarget = dyingPeople.OrderBy(ObjectHelper.DistanceToPlayer).FirstOrDefault()
            ?? weakenPeople.OrderBy(ObjectHelper.DistanceToPlayer).FirstOrDefault();

        DataCenter.RefinedHP = DataCenter.PartyMembers
            .ToDictionary(p => p.EntityId, GetPartyMemberHPRatio);
        DataCenter.PartyMembersHP = DataCenter.RefinedHP.Values.Where(r => r > 0);

        if (DataCenter.PartyMembersHP.Any())
        {
            DataCenter.PartyMembersAverHP = DataCenter.PartyMembersHP.Average();
            DataCenter.PartyMembersDifferHP = (float)Math.Sqrt(DataCenter.PartyMembersHP.Average(d => Math.Pow(d - DataCenter.PartyMembersAverHP, 2)));
        }
        else
        {
            DataCenter.PartyMembersAverHP = DataCenter.PartyMembersDifferHP = 0;
        }

        DataCenter.PartyMembersMinHP = DataCenter.PartyMembersHP.Any() ? DataCenter.PartyMembersHP.Min() : 0;
        DataCenter.HPNotFull = DataCenter.PartyMembersMinHP < 1;

        _lastHp = DataCenter.PartyMembers.ToDictionary(p => p.EntityId, p => p.CurrentHp);

        if (DataCenter.InEffectTime)
        {
            var rightMp = Player.Object.CurrentMp;
            if (rightMp - _lastMp == DataCenter.MPGain)
            {
                DataCenter.MPGain = 0;
            }
            DataCenter.CurrentMp = Math.Min(10000, Player.Object.CurrentMp + DataCenter.MPGain);
        }
        else
        {
            DataCenter.CurrentMp = Player.Object.CurrentMp;
        }
        _lastMp = Player.Object.CurrentMp;
    }

    private static IBattleChara? GetDeathTarget(IEnumerable<IBattleChara> deathAll, IEnumerable<IBattleChara> deathParty)
    {
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

    private static float GetPartyMemberHPRatio(IBattleChara member)
    {
        if (member == null) return 0;

        if (!DataCenter.InEffectTime
            || !DataCenter.HealHP.TryGetValue(member.EntityId, out var hp))
        {
            return (float)member.CurrentHp / member.MaxHp;
        }

        var rightHp = member.CurrentHp;
        if (rightHp > 0)
        {
            if (!_lastHp.TryGetValue(member.EntityId, out var lastHp)) lastHp = rightHp;

            if (rightHp - lastHp == hp)
            {
                DataCenter.HealHP.Remove(member.EntityId);
                return (float)member.CurrentHp / member.MaxHp;
            }
            return Math.Min(1, (hp + rightHp) / (float)member.MaxHp);
        }
        return (float)member.CurrentHp / member.MaxHp;
    }
    #endregion

    private static void UpdateNamePlate(IEnumerable<IBattleChara> allTargets)
    {
        List<uint> charas = new(5);
        //60687 - 60691 For treasure hunt.
        for (int i = 60687; i <= 60691; i++)
        {
            var b = allTargets.FirstOrDefault(obj => obj.GetNamePlateIcon() == i);
            if (b == null || b.CurrentHp == 0) continue;
            charas.Add(b.EntityId);
        }
        DataCenter.TreasureCharas = [.. charas];
    }
}
