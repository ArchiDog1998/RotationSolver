using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using RotationSolver.Basic;
using RotationSolver.Data;
using RotationSolver.Helpers;
using System.Numerics;

namespace RotationSolver.Updaters;

internal static partial class TargetUpdater
{
    private static IEnumerable<BattleChara> GetPartyMembers(IEnumerable<BattleChara> allTargets)
    {
        var party = Service.PartyList.Select(p => p.GameObject).OfType<BattleChara>().Where(b => b.DistanceToPlayer() <= 30);

        if (!party.Any()) party = new BattleChara[] { Service.Player };

        return party.Union(allTargets.Where(obj => obj.SubKind == 9));
    }

    private unsafe static void UpdateFriends(IEnumerable<BattleChara> allTargets)
    {
        DataCenter.PartyMembers = GetPartyMembers(allTargets);
        DataCenter.AllianceMembers = allTargets.OfType<PlayerCharacter>();

        var mayPet = allTargets.OfType<BattleNpc>().Where(npc => npc.OwnerId == Service.Player.ObjectId);
        DataCenter.HasPet = mayPet.Any(npc => npc.BattleNpcKind == BattleNpcSubKind.Pet);

        DataCenter.PartyTanks = DataCenter.PartyMembers.GetJobCategory(JobRole.Tank);
        DataCenter.PartyHealers = DataCenter.PartyMembers.GetJobCategory(JobRole.Healer);
        DataCenter.AllianceTanks = DataCenter.AllianceMembers.GetJobCategory(JobRole.Tank);

        var deathAll = DataCenter.AllianceMembers.GetDeath();
        var deathParty = DataCenter.PartyMembers.GetDeath();
        MaintainDeathPeople(ref deathAll, ref deathParty);
        DataCenter.DeathPeopleAll.Delay(deathAll);
        DataCenter.DeathPeopleParty.Delay(deathParty);

        DataCenter.WeakenPeople.Delay(DataCenter.PartyMembers.Where(p => p.StatusList.Any(StatusHelper.CanDispel)));
        DataCenter.DyingPeople.Delay(DataCenter.WeakenPeople.Where(p => p.StatusList.Any(StatusHelper.IsDangerous)));

        DataCenter.PartyMembersHP = DataCenter.PartyMembers.Select(ObjectHelper.GetHealthRatio).Where(r => r > 0);
        if (DataCenter.PartyMembersHP.Any())
        {
            DataCenter.PartyMembersAverHP = DataCenter.PartyMembersHP.Average();
            DataCenter.PartyMembersDifferHP = (float)Math.Sqrt(DataCenter.PartyMembersHP.Average(d => Math.Pow(d - DataCenter.PartyMembersAverHP, 2)));
        }
        else
        {
            DataCenter.PartyMembersAverHP = DataCenter.PartyMembersDifferHP = 0;
        }

        UpdateCanHeal(Service.Player);
    }

    static RandomDelay _healDelay1 = new RandomDelay(GetHealRange);
    static RandomDelay _healDelay2 = new RandomDelay(GetHealRange);
    static RandomDelay _healDelay3 = new RandomDelay(GetHealRange);
    static RandomDelay _healDelay4 = new RandomDelay(GetHealRange);

    static (float min, float max) GetHealRange() => (Service.Config.HealDelayMin, Service.Config.HealDelayMax);

    static void UpdateCanHeal(PlayerCharacter player)
    {
        var job = (ClassJobID)player.ClassJob.Id;

        var hotSubSingle = job.GetHealingOfTimeSubtractSingle();
        var singleAbility = ShouldHealSingle(StatusHelper.SingleHots, job.GetHealSingleAbility(), hotSubSingle);
        var singleSpell = ShouldHealSingle(StatusHelper.SingleHots, job.GetHealSingleSpell(), hotSubSingle);
        DataCenter.CanHealSingleAbility = singleAbility > 0;
        DataCenter.CanHealSingleSpell = singleSpell > 0;
        DataCenter.CanHealAreaAbility = singleAbility > 2;
        DataCenter.CanHealAreaSpell = singleSpell > 2;

        if (DataCenter.PartyMembers.Count() > 2)
        {
            //TODO:少了所有罩子类技能
            var ratio = GetHealingOfTimeRatio(player, StatusHelper.AreaHots) * job.GetHealingOfTimeSubtractArea();

            if(!DataCenter.CanHealAreaAbility)
                DataCenter.CanHealAreaAbility = DataCenter.PartyMembersDifferHP < Service.Config.HealthDifference && DataCenter.PartyMembersAverHP < ConfigurationHelper.GetHealAreaAbility(job) - ratio;

            if (!DataCenter.CanHealAreaSpell)
                DataCenter.CanHealAreaSpell = DataCenter.PartyMembersDifferHP < Service.Config.HealthDifference && DataCenter.PartyMembersAverHP < ConfigurationHelper.GetHealAreaSpell(job) - ratio;
        }

        //Delay
        DataCenter.CanHealSingleAbility = _healDelay1.Delay(DataCenter.CanHealSingleAbility);
        DataCenter.CanHealSingleSpell = _healDelay2.Delay(DataCenter.CanHealSingleSpell);
        DataCenter.CanHealAreaAbility = _healDelay3.Delay(DataCenter.CanHealAreaAbility);
        DataCenter.CanHealAreaSpell = _healDelay4.Delay(DataCenter.CanHealAreaSpell);

        DataCenter.PartyMembersMinHP = DataCenter.PartyMembersHP.Any() ? DataCenter.PartyMembersHP.Min() : 0;
        DataCenter.HPNotFull = DataCenter.PartyMembersMinHP < 1;
    }

    static int ShouldHealSingle(StatusID[] hotStatus, float healSingle, float hotSubSingle) => DataCenter.PartyMembers.Count(p =>
    {
        var ratio = GetHealingOfTimeRatio(p, hotStatus);

        var h = p.GetHealthRatio();
        if (h == 0 || !p.NeedHealing()) return false;

        return h < healSingle - hotSubSingle * ratio;
    });

    static float GetHealingOfTimeRatio(BattleChara target, params StatusID[] statusIds)
    {
        const float buffWholeTime = 15;

        var buffTime = target.StatusTime(false, statusIds);

        return Math.Min(1, buffTime / buffWholeTime);
    }

    static SortedDictionary<uint, Vector3> _locations = new SortedDictionary<uint, Vector3>();
    private static void MaintainDeathPeople(ref IEnumerable<BattleChara> deathAll, ref IEnumerable<BattleChara> deathParty)
    {
        SortedDictionary<uint, Vector3> locs = new SortedDictionary<uint, Vector3>();
        foreach (var item in deathAll)
        {
            locs[item.ObjectId] = item.Position;
        }
        foreach (var item in deathParty)
        {
            locs[item.ObjectId] = item.Position;
        }

        deathAll = FilterForDeath(deathAll);
        deathParty = FilterForDeath(deathParty);

        _locations = locs;
    }

    private static IEnumerable<BattleChara> FilterForDeath(IEnumerable<BattleChara> battleCharas) 
        => battleCharas.Where(b =>
        {
            if (!_locations.TryGetValue(b.ObjectId, out var loc)) return false;

            return loc == b.Position;
        });
}
