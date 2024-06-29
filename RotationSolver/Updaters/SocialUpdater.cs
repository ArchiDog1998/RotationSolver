using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using ECommons.DalamudServices;
using ECommons.GameHelpers;
using FFXIVClientStructs.FFXIV.Client.UI;
using Lumina.Excel.GeneratedSheets;
using RotationSolver.Basic.Configuration;
using RotationSolver.Basic.Rotations.Duties;
using RotationSolver.Commands;
using RotationSolver.Data;
using RotationSolver.Helpers;
using RotationSolver.Localization;
using System.Diagnostics.CodeAnalysis;

namespace RotationSolver.Updaters;

internal class SocialUpdater
{
#if DEBUG
#else
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
#endif

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
        await Task.Delay(new Random().Next(4000, 6000));

        Service.Config.DutyEnd.AddMacro();

        if (Service.Config.AutoOffWhenDutyCompleted)
        {
            RSCommands.CancelState();
        }
    }

    static void ClientState_TerritoryChanged(ushort id)
    {
        DataCenter.ResetAllRecords();

        var territory = Service.GetSheet<TerritoryType>().GetRow(id);
        _canSaying = territory?.ContentFinderCondition?.Value?.RowId != 0;

        DataCenter.Territory = territory;

#if DEBUG
        Svc.Log.Debug($"Move to {DataCenter.TerritoryName} ({id})");
#endif

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
        }
    }

    private static readonly ChatEntityComparer _comparer = new();

    internal abstract class ChatEntity(IPlayerCharacter character) : IDisposable
    {
        public readonly IPlayerCharacter player = character;

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

        public abstract SeString GetMessage();

        public void Dispose()
        {
            OtherConfiguration.RotationSolverRecord.SayingHelloCount++;
            var hash = player.EncryptString();
            saidAuthors.Add(hash);
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

    internal class RotationAuthorChatEntity(IPlayerCharacter character, string nameDesc) : ChatEntity(character)
    {
        private readonly string name = nameDesc;

        public override SeString GetMessage() =>
            Character
            .Append(new SeString(new TextPayload($"({name}) is one of the authors of ")))
            .Append(RotationSolver)
            .Append(new SeString(new TextPayload(". So say hello to them!")));
    }


    internal class ContributorChatEntity(IPlayerCharacter character) 
        : ChatEntity(character)
    {
        public override SeString GetMessage() =>
            Character
            .Append(new SeString(new TextPayload($" is one of the contributors of ")))
            .Append(RotationSolver)
            .Append(new SeString(new TextPayload(". So say hello to them!")));
    }

    internal class UserChatEntity(IPlayerCharacter character) 
        : ChatEntity(character)
    {
        public override BitmapFontIcon Icon => BitmapFontIcon.NewAdventurer;

        public override SeString GetMessage() =>
            Character
            .Append(new SeString(new TextPayload($" is one of the users of ")))
            .Append(RotationSolver)
            .Append(new SeString(new TextPayload(". So say hello to them!")));
    }
}
