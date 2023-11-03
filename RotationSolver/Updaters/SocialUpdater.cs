using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using ECommons.DalamudServices;
using ECommons.GameHelpers;
using FFXIVClientStructs.FFXIV.Client.UI;
using Lumina.Excel.GeneratedSheets;
using RotationSolver.Basic.Configuration;
using RotationSolver.Helpers;
using RotationSolver.Localization;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text;

namespace RotationSolver.Updaters;

internal class SocialUpdater
{
    private static readonly List<string> _macroToAuthor = new()
    {
        "blush",
        "hug",
        "thumbsup",
        "yes",
        "clap",
        "cheer",
        "stroke",
    };

    private static readonly HashSet<string> saidAuthors = new();

    static bool _canSaying = false;

    public static string GetDutyName(TerritoryType territory)
    {
        return territory.ContentFinderCondition?.Value?.Name?.RawString;
    }

    static bool CanSocial
    {
        get
        {
            if (Svc.Condition[ConditionFlag.OccupiedInQuestEvent]
                || Svc.Condition[ConditionFlag.WaitingForDuty]
                || Svc.Condition[ConditionFlag.WaitingForDutyFinder]
                || Svc.Condition[ConditionFlag.OccupiedInCutSceneEvent]
                || Svc.Condition[ConditionFlag.BetweenAreas]
                || Svc.Condition[ConditionFlag.BetweenAreas51]) return false;

            if (!Player.Interactable) return false;

            return Svc.Condition[ConditionFlag.BoundByDuty];
        }
    }

    internal static void Enable()
    {
        Svc.DutyState.DutyStarted += DutyState_DutyStarted;
        Svc.DutyState.DutyWiped += DutyState_DutyWiped;
        Svc.DutyState.DutyCompleted += DutyState_DutyCompleted;
        Svc.ClientState.TerritoryChanged += ClientState_TerritoryChanged;
        ClientState_TerritoryChanged(Svc.ClientState.TerritoryType);
    }

    static async void DutyState_DutyCompleted(object sender, ushort e)
    {
        if (DataCenter.PartyMembers.Count() < 2) return;

        await Task.Delay(new Random().Next(4000, 6000));

        Service.Config.GlobalConfig.DutyEnd.AddMacro();
    }

    static void ClientState_TerritoryChanged(ushort id)
    {
        DataCenter.ResetAllRecords();

        var territory = Service.GetSheet<TerritoryType>().GetRow(id);
        _canSaying = territory?.ContentFinderCondition?.Value?.RowId != 0;

        DataCenter.Territory = territory;

        try
        {
            DataCenter.RightNowRotation?.OnTerritoryChanged();
        }
        catch (Exception ex)
        {
            Svc.Log.Error(ex, "Failed on Territory changed.");
        }
    }

    static void DutyState_DutyStarted(object sender, ushort e)
    {
        if (!Player.Available) return;
        if (!Player.Object.IsJobCategory(JobRole.Tank) && !Player.Object.IsJobCategory(JobRole.Healer)) return;

        if (DataCenter.IsInHighEndDuty)
        {
            string.Format(LocalizationManager.RightLang.HighEndWarning,
                DataCenter.ContentFinderName).ShowWarning();
        }
    }

    static void DutyState_DutyWiped(object sender, ushort e)
    {
        if (!Player.Available) return;
        DataCenter.ResetAllRecords();
    }

    internal static void Disable()
    {
        Svc.DutyState.DutyStarted -= DutyState_DutyStarted;
        Svc.DutyState.DutyWiped -= DutyState_DutyWiped;
        Svc.DutyState.DutyCompleted -= DutyState_DutyCompleted;
        Svc.ClientState.TerritoryChanged -= ClientState_TerritoryChanged;
    }

    static RandomDelay socialDelay = new(() => (3, 5));
    internal static async void UpdateSocial()
    {
        if (DataCenter.InCombat) return;
        if (_canSaying && socialDelay.Delay(CanSocial))
        {
            _canSaying = false;
#if DEBUG
            Svc.Chat.Print("Macro now.");
#endif
            Service.Config.GlobalConfig.DutyStart.AddMacro();
            await Task.Delay(new Random().Next(1000, 1500));

            SayHelloToUsers();
        }
    }

    private static readonly ChatEntityComparer _comparer = new();
    private static async void SayHelloToUsers()
    {
        var players = DataCenter.AllianceMembers.OfType<PlayerCharacter>()
#if DEBUG
#else
            .Where(c => c.ObjectId != Player.Object.ObjectId)
#endif
            .Select(player => (player, EncryptString(player)))
            .Where(pair => !saidAuthors.Contains(pair.Item2)
                && !OtherConfiguration.RotationSolverRecord.SaidUsers.Contains(pair.Item2));

        IEnumerable<ChatEntity> entities = players
            .Select(c =>
            {
                if (!RotationUpdater.AuthorHashes.TryGetValue(c.Item2, out var nameDesc)) nameDesc = string.Empty;
                return (c.player, nameDesc);
            })
            .Where(p => !string.IsNullOrEmpty(p.nameDesc))
            .Select(p => new RotationAuthorChatEntity(p.player, p.nameDesc));

        entities = entities.Union(players
            .Where(p => DownloadHelper.ContributorsHash.Contains(p.Item2))
            .Select(p => new ContributorChatEntity(p.player)), _comparer);

        if (Service.Config.GetValue(PluginConfigBool.SayHelloToUsers))
        {
            entities = entities.Union(players
                .Where(p => DownloadHelper.UsersHash.Contains(p.Item2))
                .Select(p => new UserChatEntity(p.player)), _comparer);
        }

        foreach (var entity in entities)
        {
            while (!entity.CanTarget && !DataCenter.InCombat)
            {
                await Task.Delay(100);
            }

#if DEBUG
#else
            Svc.Targets.Target = entity.player;
            ECommons.Automation.Chat.Instance.SendMessage($"/{_macroToAuthor[new Random().Next(_macroToAuthor.Count)]} <t>");
#endif
            Svc.Chat.Print(new Dalamud.Game.Text.XivChatEntry()
            {
                Message = entity.GetMessage(),
                Type = Dalamud.Game.Text.XivChatType.Notice,
            });
            UIModule.PlaySound(20, 0, 0, 0);
            entity.Dispose();

            await Task.Delay(new Random().Next(800, 1200));
            Svc.Targets.Target = null;
            await Task.Delay(new Random().Next(800, 1200));
        }
    }

    internal static string EncryptString(PlayerCharacter player)
    {
        try
        {
            byte[] inputByteArray = Encoding.UTF8.GetBytes(player.HomeWorld.GameData.InternalName.ToString()
    + " - " + player.Name.ToString() + "U6Wy.zCG");

            var tmpHash = MD5.HashData(inputByteArray);
            var retB = Convert.ToBase64String(tmpHash);
            return retB;
        }
        catch (Exception ex)
        {
            Svc.Log.Warning(ex, "Failed to read the player's name and world.");
            return string.Empty;
        }
    }

    internal abstract class ChatEntity : IDisposable
    {
        public readonly PlayerCharacter player;

        public bool CanTarget
        {
            get
            {
                try
                {
                    return player.IsTargetable;
                }
                catch
                {
                    return false;
                }
            }
        }

        public virtual BitmapFontIcon Icon => BitmapFontIcon.Mentor;

        protected SeString Character => new(new IconPayload(Icon),
            new UIForegroundPayload(31),
            new PlayerPayload(player.Name.TextValue, player.HomeWorld.Id),
            UIForegroundPayload.UIForegroundOff);

        protected static SeString RotationSolver => new(new IconPayload(BitmapFontIcon.DPS),
            RotationSolverPlugin.OpenLinkPayload,
            new UIForegroundPayload(31),
            new TextPayload("Rotation Solver"),
            UIForegroundPayload.UIForegroundOff,
            RawPayload.LinkTerminator);

        public ChatEntity(PlayerCharacter character)
        {
            player = character;
        }
        public abstract SeString GetMessage();

        public void Dispose()
        {
            var hash = EncryptString(player);
            saidAuthors.Add(hash);
            if (Service.Config.GetValue(PluginConfigBool.JustSayHelloOnce))
            {
                OtherConfiguration.RotationSolverRecord.SaidUsers.Add(hash);
            }
        }
    }

    internal class ChatEntityComparer : IEqualityComparer<ChatEntity>
    {
        public bool Equals(ChatEntity x, ChatEntity y)
            => x.player.Equals(y.player);

        public int GetHashCode([DisallowNull] ChatEntity obj)
            => obj.player.GetHashCode();
    }

    internal class RotationAuthorChatEntity : ChatEntity
    {
        private readonly string name;
        public RotationAuthorChatEntity(PlayerCharacter character, string nameDesc) : base(character)
        {
            name = nameDesc;
        }

        public override SeString GetMessage() =>
            Character
            .Append(new SeString(new TextPayload($"({name}) is one of the authors of ")))
            .Append(RotationSolver)
            .Append(new SeString(new TextPayload(". So say hello to him/her!")));
    }


    internal class ContributorChatEntity : ChatEntity
    {
        public ContributorChatEntity(PlayerCharacter character) : base(character)
        {
        }

        public override SeString GetMessage() =>
            Character
            .Append(new SeString(new TextPayload($" is one of the contributors of ")))
            .Append(RotationSolver)
            .Append(new SeString(new TextPayload(". So say hello to him/her!")));
    }

    internal class UserChatEntity : ChatEntity
    {
        public override BitmapFontIcon Icon => BitmapFontIcon.NewAdventurer;

        public UserChatEntity(PlayerCharacter character) : base(character)
        {
        }

        public override SeString GetMessage() =>
            Character
            .Append(new SeString(new TextPayload($" is one of the users of ")))
            .Append(RotationSolver)
            .Append(new SeString(new TextPayload(". So say hello to him/her!")));
    }
}