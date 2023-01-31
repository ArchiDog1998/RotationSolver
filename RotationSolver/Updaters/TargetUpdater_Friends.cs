using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using FFXIVClientStructs.FFXIV.Client.UI;
using RotationSolver.Commands;
using RotationSolver.Data;
using RotationSolver.Helpers;
using RotationSolver.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace RotationSolver.Updaters;

internal static partial class TargetUpdater
{
    /// <summary>
    /// 小队成员们
    /// </summary>
    public static IEnumerable<BattleChara> PartyMembers { get; private set; } = new PlayerCharacter[0];
    /// <summary>
    /// 团队成员们
    /// </summary>
    internal static IEnumerable<BattleChara> AllianceMembers { get; private set; } = new PlayerCharacter[0];

    /// <summary>
    /// 小队坦克们
    /// </summary>
    internal static IEnumerable<BattleChara> PartyTanks { get; private set; } = new PlayerCharacter[0];
    /// <summary>
    /// 小队治疗们
    /// </summary>
    internal static IEnumerable<BattleChara> PartyHealers { get; private set; } = new PlayerCharacter[0];

    /// <summary>
    /// 团队坦克们
    /// </summary>
    internal static IEnumerable<BattleChara> AllianceTanks { get; private set; } = new PlayerCharacter[0];

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal static IEnumerable<BattleChara> DeathPeopleAll { get; private set; } = new PlayerCharacter[0];

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal static IEnumerable<BattleChara> DeathPeopleParty { get; private set; } = new PlayerCharacter[0];

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal static IEnumerable<BattleChara> WeakenPeople { get; private set; } = new PlayerCharacter[0];

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal static IEnumerable<BattleChara> DyingPeople { get; private set; } = new PlayerCharacter[0];
    /// <summary>
    /// 小队成员HP
    /// </summary>
    internal static IEnumerable<float> PartyMembersHP { get; private set; } = new float[0];
    /// <summary>
    /// 小队成员最小的HP
    /// </summary>
    internal static float PartyMembersMinHP { get; private set; } = 0;
    /// <summary>
    /// 小队成员平均HP
    /// </summary>
    internal static float PartyMembersAverHP { get; private set; } = 0;
    /// <summary>
    /// 小队成员HP差值
    /// </summary>
    internal static float PartyMembersDifferHP { get; private set; } = 0;


    [EditorBrowsable(EditorBrowsableState.Never)]
    internal static bool CanHealAreaAbility { get; private set; } = false;

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal static bool CanHealAreaSpell { get; private set; } = false;

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal static bool CanHealSingleAbility { get; private set; } = false;

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal static bool CanHealSingleSpell { get; private set; } = false;

    /// <summary>
    /// 有宠物
    /// </summary>
    internal static bool HavePet { get; private set; } = false;

    /// <summary>
    /// 有陆行鸟
    /// </summary>
    internal static bool HaveChocobo { get; private set; } = false;
    /// <summary>
    /// 血量没有满
    /// </summary>
    internal static bool HPNotFull { get; private set; } = false;

    private static IEnumerable<BattleChara> GetPartyMembers(IEnumerable<BattleChara> allTargets)
    {
        var party = Service.PartyList.Select(p => p.GameObject).OfType<BattleChara>().Where(b => b.DistanceToPlayer() <= 30);

        if (!party.Any()) party = new BattleChara[] { Service.ClientState.LocalPlayer };

        return party.Union(allTargets.Where(obj => obj.SubKind == 9));
    }

    private unsafe static void UpdateFriends(IEnumerable<BattleChara> allTargets)
    {
        #region Friend
        PartyMembers = GetPartyMembers(allTargets);

        var mayPet = allTargets.OfType<BattleNpc>().Where(npc => npc.OwnerId == Service.ClientState.LocalPlayer.ObjectId);
        HavePet = mayPet.Any(npc => npc.BattleNpcKind == BattleNpcSubKind.Pet);
        HaveChocobo = mayPet.Any(npc => npc.BattleNpcKind == BattleNpcSubKind.Chocobo); 

        AllianceMembers = allTargets.OfType<PlayerCharacter>();

        PartyTanks = PartyMembers.GetJobCategory(JobRole.Tank);
        PartyHealers = PartyMembers.GetJobCategory(JobRole.Healer);
        AllianceTanks = AllianceMembers.GetJobCategory(JobRole.Tank);

        DeathPeopleAll = AllianceMembers.GetDeath();
        DeathPeopleParty = PartyMembers.GetDeath();
        MaintainDeathPeople();

        WeakenPeople = PartyMembers.Where(p => p.StatusList.Any(status => 
            status.GameData.CanDispel && status.RemainingTime > 2));

        DyingPeople = WeakenPeople.Where(p => p.StatusList.Any(StatusHelper.IsDangerous));

        SayHelloToAuthor();
        #endregion

        #region Health
        PartyMembersHP = PartyMembers.Select(ObjectHelper.GetHealthRatio).Where(r => r > 0);
        PartyMembersAverHP = PartyMembersHP.Average();
        PartyMembersDifferHP = (float)Math.Sqrt(PartyMembersHP.Average(d => Math.Pow(d - PartyMembersAverHP, 2)));

        UpdateCanHeal(Service.ClientState.LocalPlayer);
        #endregion
    }

    static void UpdateCanHeal(PlayerCharacter player)
    {
        var job = (ClassJobID)player.ClassJob.Id;

        var hotSubSingle = job.GetHealingOfTimeSubtractSingle();
        var singleAbility = ShouldHealSingle(StatusHelper.SingleHots, job.GetHealSingleAbility(), hotSubSingle);
        var singleSpell = ShouldHealSingle(StatusHelper.SingleHots, job.GetHealSingleSpell(), hotSubSingle);
        CanHealSingleAbility = singleAbility > 0;
        CanHealSingleSpell = singleSpell > 0;
        CanHealAreaAbility = singleAbility > 2;
        CanHealAreaSpell = singleSpell > 2;

        if (PartyMembers.Count() > 2)
        {
            //TODO:少了所有罩子类技能
            var ratio = GetHealingOfTimeRatio(player, StatusHelper.AreaHots) * job.GetHealingOfTimeSubtractArea();

            if(!CanHealAreaAbility)
                CanHealAreaAbility = PartyMembersDifferHP < Service.Configuration.HealthDifference && PartyMembersAverHP < ConfigurationHelper.GetHealAreaAbility(job) - ratio;

            if (!CanHealAreaSpell)
                CanHealAreaSpell = PartyMembersDifferHP < Service.Configuration.HealthDifference && PartyMembersAverHP < ConfigurationHelper.GetHealAreafSpell(job) - ratio;
        }

        PartyMembersMinHP = PartyMembersHP.Any() ? PartyMembersHP.Min() : 0;
        HPNotFull = PartyMembersMinHP < 1;
    }

    static int ShouldHealSingle(StatusID[] hotStatus, float healSingle, float hotSubSingle) => PartyMembers.Count(p =>
    {
        var ratio = GetHealingOfTimeRatio(p, hotStatus);

        var h = p.GetHealthRatio();
        if (h == 0 || !StatusHelper.NeedHealing(p)) return false;

        return h < healSingle - hotSubSingle * ratio;
    });

    static float GetHealingOfTimeRatio(BattleChara target, params StatusID[] statusIds)
    {
        const float buffWholeTime = 15;

        var buffTime = target.StatusTime(false, statusIds);

        return Math.Min(1, buffTime / buffWholeTime);
    }

    static SortedDictionary<uint, Vector3> _locations = new SortedDictionary<uint, Vector3>();
    private static void MaintainDeathPeople()
    {
        SortedDictionary<uint, Vector3> locs = new SortedDictionary<uint, Vector3>();
        foreach (var item in DeathPeopleAll)
        {
            locs[item.ObjectId] = item.Position;
        }
        foreach (var item in DeathPeopleParty)
        {
            locs[item.ObjectId] = item.Position;
        }

        DeathPeopleAll = FilterForDeath(DeathPeopleAll);
        DeathPeopleParty = FilterForDeath(DeathPeopleParty);

        _locations = locs;
    }

    private static IEnumerable<BattleChara> FilterForDeath(IEnumerable<BattleChara> battleCharas)
    {
        return battleCharas.Where(b =>
        {
            if (!_locations.TryGetValue(b.ObjectId, out var loc)) return false;

            return loc == b.Position;
        });
    }


    /// <summary>
    /// 作者本人
    /// </summary>
    static DateTime foundTime = DateTime.Now;
    static TimeSpan relayTime = TimeSpan.Zero;

    static List<string> macroToAuthor = new List<string>()
    {
        "blush",
        "hug",
        "thumbsup",
        "yes",
        "clap",
        "cheer",
        "stroke",
    };
    private static void SayHelloToAuthor()
    {
        //只有任务中才能执行此操作
        if (!Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.BoundByDuty]
            || Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.OccupiedInQuestEvent]
            || Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.WaitingForDuty]
            || Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.WaitingForDutyFinder]
            || Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.OccupiedInCutSceneEvent]
            || Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.BetweenAreas]
            || Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.BetweenAreas51]) return;

        //战斗中不执行
        if (ActionUpdater.InCombat) return;

        //已经干过此事，不执行
        if (foundTime == DateTime.MinValue) return;

        //找作者
        var author = AllianceMembers.FirstOrDefault(c => c is PlayerCharacter player && ConfigurationHelper.AuthorKeys.Contains(EncryptString(player))
                        && c.ObjectId != Service.ClientState.LocalPlayer.ObjectId) as PlayerCharacter;

        //没找到作者
        if (author == null) return;

        //Random Time
        if (relayTime == TimeSpan.Zero)
        {
            foundTime = DateTime.Now;
            relayTime = new TimeSpan(new Random().Next(30000, 80000));
        }

        if (DateTime.Now - foundTime > relayTime)
        {
            Service.TargetManager.SetTarget(author);
            RSCommands.SubmitToChat($"/{macroToAuthor[new Random().Next(macroToAuthor.Count)]} <t>");
            Service.ChatGui.PrintChat(new Dalamud.Game.Text.XivChatEntry()
            {
                Message = string.Format(LocalizationManager.RightLang.Commands_SayHelloToAuthor, author.Name),
                Type = Dalamud.Game.Text.XivChatType.Notice,
            });
            UIModule.PlaySound(20, 0, 0, 0);
            foundTime = DateTime.MinValue;
            Service.TargetManager.SetTarget(null);
        }
    }

    internal static string EncryptString(PlayerCharacter player)
    {
        byte[] inputByteArray = Encoding.UTF8.GetBytes(player.HomeWorld.GameData.InternalName.ToString() + " - " + player.Name.ToString() + "U6Wy.zCG");

        var tmpHash = MD5.Create().ComputeHash(inputByteArray);
        var retB = Convert.ToBase64String(tmpHash.ToArray());
        return retB;
    }
}
