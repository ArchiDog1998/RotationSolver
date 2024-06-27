using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using ECommons.DalamudServices;
using ECommons.GameHelpers;
using FFXIVClientStructs.FFXIV.Client.UI;
using Lumina.Excel.GeneratedSheets;
using RotationSolver.Basic.Configuration;
using RotationSolver.Commands;
using System.Diagnostics.CodeAnalysis;
using XIVConfigUI;

namespace RotationSolver.Updaters;

internal class SocialUpdater
{
    private static readonly List<string> _macroToAuthor =
    [
        "blush",
        "hug",
        "thumbsup",
        "yes",
        "clap",
        "cheer",
        "stroke",
    ];

    private static readonly HashSet<string> saidAuthors = [];

    static bool _canSaying = false;

    public static string? GetDutyName(TerritoryType territory)
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

    static async void DutyState_DutyCompleted(object? sender, ushort e)
    {
        if (Service.Config.AutoOffWhenDutyCompleted)
        {
            RSCommands.CancelState();
        }

        if (DataCenter.PartyMembers.Length < 2) return;

        await Task.Delay(new Random().Next(4000, 6000));

        Service.Config.DutyEnd.AddMacro();
    }

    static void ClientState_TerritoryChanged(ushort id)
    {
        DataCenter.ResetAllRecords();

        var territory = Service.GetSheet<TerritoryType>().GetRow(id);
        _canSaying = territory?.ContentFinderCondition?.Value?.RowId != 0;
        _users.Clear();

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

    static void DutyState_DutyStarted(object? sender, ushort e)
    {
        if (!Player.Available) return;
        if (!Player.Object.IsJobCategory(JobRole.Tank) && !Player.Object.IsJobCategory(JobRole.Healer)) return;

        var message = Service.InvalidUseString;
        if (!string.IsNullOrEmpty(message))
        {
            message.ShowWarning();
            return;
        }

        if (DataCenter.IsInHighEndDuty)
        {
            string.Format(UiString.HighEndWarning.Local(),
                DataCenter.ContentFinderName).ShowWarning();
        }
    }

    static void DutyState_DutyWiped(object? sender, ushort e)
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

    static RandomDelay socialDelay = new(() => (5, 8));
    internal static async void UpdateSocial()
    {
        if (DataCenter.InCombat) return;
        if (_canSaying && socialDelay.Delay(CanSocial))
        {
            _canSaying = false;
            Service.Config.DutyStart.AddMacro();
            await Task.Delay(new Random().Next(1000, 1500));

            SayHelloToUsers();
        }
    }

    private static readonly ChatEntityComparer _comparer = new();
    internal static readonly List<PlayerCharacter> _users = [];
    private static async void SayHelloToUsers()
    {
        var players = DataCenter.AllianceMembers.OfType<PlayerCharacter>()
            .Where(c => c.EntityId != Player.Object.EntityId)
            .Select(player => (player, player.EncryptString()));

        IEnumerable<ChatEntity> entities = players
            .Select(c =>
            {
                if (!DataCenter.AuthorHashes.TryGetValue(c.Item2, out var nameDesc)) nameDesc = string.Empty;
                return (c.player, c.Item2, nameDesc);
            })
            .Where(p => !string.IsNullOrEmpty(p.nameDesc))
            .Select(p => new RotationAuthorChatEntity(p.player, p.Item2, p.nameDesc));

        entities = entities.Union(players
            .Where(p => DownloadHelper.ContributorsHash.Contains(p.Item2))
            .Select(p => new ContributorChatEntity(p.player, p.Item2)), _comparer);

        entities = entities.Union(players
            .Where(p => DownloadHelper.UsersHash.Contains(p.Item2))
            .Select(p => new UserChatEntity(p.player, p.Item2)), _comparer);

        _users.Clear();
        foreach (var entity in entities)
        {
            while (!entity.CanTarget && !DataCenter.InCombat)
            {
                await Task.Delay(100);
            }

            _users.Add(entity.player);

            if (Service.Config.RemindToAll)
            {
                Svc.Chat.Print(new Dalamud.Game.Text.XivChatEntry()
                {
                    Message = entity.GetMessage(),
                    Type = Dalamud.Game.Text.XivChatType.Notice,
                });
                UIModule.PlaySound(20, 0, 0, 0);
            }

            if (Service.Config.SayHelloToAll && !saidAuthors.Contains(entity.Hash)
                && !OtherConfiguration.RotationSolverRecord.SaidUsers.Contains(entity.Hash))
            {
                Svc.Targets.Target = entity.player;
                ECommons.Automation.Chat.Instance.SendMessage($"/{_macroToAuthor[new Random().Next(_macroToAuthor.Count)]} <t>");
                await Task.Delay(new Random().Next(800, 1200));
                Svc.Targets.Target = null;
            }

            entity.Dispose();
            await Task.Delay(new Random().Next(800, 1200));
        }
    }

    internal abstract class ChatEntity(PlayerCharacter character, string hash) : IDisposable
    {
        public readonly PlayerCharacter player = character;

        public string Hash => hash;

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
            WarningHelper.OpenLinkPayload,
            new UIForegroundPayload(31),
            new TextPayload("Rotation Solver"),
            UIForegroundPayload.UIForegroundOff,
            RawPayload.LinkTerminator);

        public abstract SeString GetMessage();

        public void Dispose()
        {
            OtherConfiguration.RotationSolverRecord.SayingHelloCount++;
            var hash = player.EncryptString();
            saidAuthors.Add(hash);
            if (Service.Config.JustSayHelloOnce)
            {
                OtherConfiguration.RotationSolverRecord.SaidUsers.Add(hash);
            }
        }
    }

    internal class ChatEntityComparer : IEqualityComparer<ChatEntity>
    {
        public bool Equals(ChatEntity? x, ChatEntity? y)
        {
            if(x == null || y == null) return false;
            return x.player.Equals(y.player);
        }

        public int GetHashCode([DisallowNull] ChatEntity obj)
            => obj.player.GetHashCode();
    }

    internal class RotationAuthorChatEntity(PlayerCharacter character, string hash, string nameDesc) : ChatEntity(character, hash)
    {
        private readonly string name = nameDesc;

        public override SeString GetMessage() =>
            Character
            .Append(new SeString(new TextPayload($"({name}) is one of the authors of ")))
            .Append(RotationSolver)
            .Append(new SeString(new TextPayload(". So say hello to them!")));
    }

    internal class ContributorChatEntity(PlayerCharacter character, string hash) 
        : ChatEntity(character, hash)
    {
        public override SeString GetMessage() =>
            Character
            .Append(new SeString(new TextPayload($" is one of the contributors of ")))
            .Append(RotationSolver)
            .Append(new SeString(new TextPayload(". So say hello to them!")));
    }

    internal class UserChatEntity(PlayerCharacter character, string hash) 
        : ChatEntity(character, hash)
    {
        public override BitmapFontIcon Icon => BitmapFontIcon.NewAdventurer;

        public override SeString GetMessage() =>
            Character
            .Append(new SeString(new TextPayload($" is one of the users of ")))
            .Append(RotationSolver)
            .Append(new SeString(new TextPayload(". So say hello to them!")));
    }
}
