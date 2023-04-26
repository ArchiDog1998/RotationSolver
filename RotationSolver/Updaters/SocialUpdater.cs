using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.UI;
using Lumina.Excel.GeneratedSheets;
using RotationSolver.Commands;
using RotationSolver.Localization;
using System.Security.Cryptography;
using System.Text;

namespace RotationSolver.Updaters;

internal class SocialUpdater
{
    public static bool InPvp { get; private set; }
    static List<string> _macroToAuthor = new List<string>()
    {
        "blush",
        "hug",
        "thumbsup",
        "yes",
        "clap",
        "cheer",
        "stroke",
    };

    static bool _canSaying = false;
    public static TerritoryType[] HighEndDuties { get; private set; } = new TerritoryType[0];

    static bool CanSocial
    {
        get
        {
            if (Service.Conditions[ConditionFlag.OccupiedInQuestEvent]
                || Service.Conditions[ConditionFlag.WaitingForDuty]
                || Service.Conditions[ConditionFlag.WaitingForDutyFinder]
                || Service.Conditions[ConditionFlag.OccupiedInCutSceneEvent]
                || Service.Conditions[ConditionFlag.BetweenAreas]
                || Service.Conditions[ConditionFlag.BetweenAreas51]) return false;

            if(Service.Player == null) return false;
            if (!Service.Player.IsTargetable()) return false;

            return Service.Conditions[ConditionFlag.BoundByDuty];
        }
    }

    internal static void Enable()
    {
        Service.DutyState.DutyStarted += DutyState_DutyStarted;
        Service.DutyState.DutyCompleted += DutyState_DutyCompleted;
        Service.ClientState.TerritoryChanged += ClientState_TerritoryChanged;

        HighEndDuties = Service.GetSheet<TerritoryType>()
            .Where(t => t?.ContentFinderCondition?.Value?.HighEndDuty ?? false)
            .ToArray();
    }

    static async void DutyState_DutyCompleted(object sender, ushort e)
    {
        if (DataCenter.PartyMembers.Count() < 2) return;

        await Task.Delay(new Random().Next(4000, 6000));

        Service.Config.DutyEnd.AddMacro();
    }

    static void ClientState_TerritoryChanged(object sender, ushort e)
    {
        RSCommands.UpdateStateNamePlate();
        var territory = Service.GetSheet<TerritoryType>().GetRow(e);
        if (territory?.ContentFinderCondition?.Value?.RowId != 0)
        {
            _canSaying = true;
        }
        InPvp = territory.IsPvpZone;
        DataCenter.TerritoryContentType = (TerritoryContentType)(territory?.ContentFinderCondition?.Value?.ContentType?.Value?.RowId ?? 0);
        DataCenter.InHighEndDuty = HighEndDuties.Any(t => t.RowId == territory.RowId);

        try
        {
            RotationUpdater.RightNowRotation?.OnTerritoryChanged();
        }
        catch(Exception ex)
        {
            PluginLog.Error(ex, "Failed on Territory changed.");
        }
    }

    static void DutyState_DutyStarted(object sender, ushort e)
    {
        var territory = Service.GetSheet<TerritoryType>().GetRow(e);
        if (HighEndDuties.Any(t => t.RowId == territory.RowId))
        {
            var str = territory.PlaceName?.Value?.Name.ToString() ?? "High-end Duty";
            Service.ToastGui.ShowError(string.Format(LocalizationManager.RightLang.HighEndWarning, str));
        }
    }

    internal static void Disable()
    {
        Service.DutyState.DutyStarted -= DutyState_DutyStarted;
        Service.DutyState.DutyCompleted -= DutyState_DutyCompleted;
        Service.ClientState.TerritoryChanged -= ClientState_TerritoryChanged;
    }

    static RandomDelay socialDelay = new RandomDelay(() => (3, 5));
    internal static async void UpdateSocial()
    {
        if (DataCenter.InCombat) return;
        if (_canSaying && socialDelay.Delay(CanSocial))
        {
            _canSaying = false;
#if DEBUG
            Service.ChatGui.Print("Macro now.");
#endif
            Service.Config.DutyStart.AddMacro();
            await Task.Delay(new Random().Next(1000, 1500));
            SayHelloToAuthor();
        }
    }

    private static async void SayHelloToAuthor()
    {
        var authors = DataCenter.AllianceMembers.OfType<PlayerCharacter>()
#if DEBUG
#else
            .Where(c => c.ObjectId != Service.ClientState.LocalPlayer.ObjectId)
#endif
            .Select(c =>
            {
                if (!RotationUpdater.AuthorHashes.TryGetValue(EncryptString(c), out var nameDesc)) nameDesc = string.Empty;
                return (c, nameDesc);
            })
            .Where(p => !string.IsNullOrEmpty(p.nameDesc));

        foreach (var author in authors)
        {
            while (!author.c.IsTargetable() && !DataCenter.InCombat)
            {
                await Task.Delay(100);
            }

#if DEBUG
#else
            Service.TargetManager.SetTarget(author.c);
            Service.SubmitToChat($"/{_macroToAuthor[new Random().Next(_macroToAuthor.Count)]} <t>");
#endif
            var message = new SeString(new IconPayload(BitmapFontIcon.Mentor),

                          new UIForegroundPayload(31),
                          new PlayerPayload(author.c.Name.TextValue, author.c.HomeWorld.Id),
                          UIForegroundPayload.UIForegroundOff,

                          new TextPayload($"({author.nameDesc}) is one of the authors of "),

                          new IconPayload(BitmapFontIcon.DPS),
                          RotationSolverPlugin.LinkPayload,
                          new UIForegroundPayload(31),
                          new TextPayload("Rotation Solver"),
                          UIForegroundPayload.UIForegroundOff,
                          RawPayload.LinkTerminator,

                          new TextPayload(". So say hello to him/her!"));

            Service.ChatGui.PrintChat(new Dalamud.Game.Text.XivChatEntry()
            {
                Message = message,
                Type = Dalamud.Game.Text.XivChatType.Notice,
            });
            UIModule.PlaySound(20, 0, 0, 0);

            await Task.Delay(new Random().Next(800, 1200));
            Service.TargetManager.SetTarget(null);
            await Task.Delay(new Random().Next(800, 1200));
        }
    }

    internal static string EncryptString(PlayerCharacter player)
    {
        byte[] inputByteArray = Encoding.UTF8.GetBytes(player.HomeWorld.GameData.InternalName.ToString()
            + " - " + player.Name.ToString() + "U6Wy.zCG");

        var tmpHash = MD5.Create().ComputeHash(inputByteArray);
        var retB = Convert.ToBase64String(tmpHash);
        return retB;
    }
}
