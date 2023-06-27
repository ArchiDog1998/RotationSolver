using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Logging;
using ECommons.Configuration;
using ECommons.DalamudServices;
using ECommons.GameFunctions;
using ECommons.GameHelpers;
using FFXIVClientStructs.FFXIV.Client.UI;
using Lumina.Excel.GeneratedSheets;
using RotationSolver.Commands;
using RotationSolver.Localization;
using RotationSolver.UI;
using System.Security.Cryptography;
using System.Text;

namespace RotationSolver.Updaters;

internal class SocialUpdater
{
    public static bool InPvp { get; private set; }
    public static bool InHouse { get; private set; }

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


    static bool _canSaying = false;
    public static TerritoryType[] HighEndDuties { get; private set; } = Array.Empty<TerritoryType>();

    public static bool IsHouseArea(TerritoryType territory)
        => territory.Bg.RawString.Contains("/hou/");

    public static string GetDutyName(TerritoryType territory)
    {
        return territory.ContentFinderCondition?.Value?.Name?.RawString ?? "High-end Duty";
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
        ClientState_TerritoryChanged(null, Svc.ClientState.TerritoryType);

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
        DataCenter.ResetAllLastActions();

        RSCommands.UpdateStateNamePlate();
        var territory = Service.GetSheet<TerritoryType>().GetRow(e);
        if (territory?.ContentFinderCondition?.Value?.RowId != 0)
        {
            _canSaying = true;
        }
        InPvp = territory.IsPvpZone;
        InHouse = IsHouseArea(territory);
        //if (PainterManager._painter != null) PainterManager._painter.Enable = !InHouse;
        DataCenter.TerritoryContentType = (TerritoryContentType)(territory?.ContentFinderCondition?.Value?.ContentType?.Value?.RowId ?? 0);
        DataCenter.InHighEndDuty = HighEndDuties.Any(t => t.RowId == territory.RowId);

#if DEBUG
        //PluginLog.Information($"Territory: {e}");
#endif

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
        if (!Player.Available) return;
        if (!Player.Object.IsJobCategory(JobRole.Tank) && !Player.Object.IsJobCategory(JobRole.Healer)) return;

        var territory = Service.GetSheet<TerritoryType>().GetRow(e);
        if (HighEndDuties.Any(t => t.RowId == territory.RowId))
        {
            var message = string.Format(LocalizationManager.RightLang.HighEndWarning, GetDutyName(territory));
            
            Svc.Chat.PrintChat(new Dalamud.Game.Text.XivChatEntry()
            {
                Message = new SeString(
                          new IconPayload(BitmapFontIcon.DPS),
                          RotationSolverPlugin.LinkPayload,
                          new UIForegroundPayload(31),
                          new TextPayload("Rotation Solver"),
                          UIForegroundPayload.UIForegroundOff,
                          RawPayload.LinkTerminator,

                          new TextPayload(": " + message)),
            Type = Dalamud.Game.Text.XivChatType.ErrorMessage,
            });

            Task.Run(async() =>
            {
                for (int i = 0; i < 3; i++)
                {
                    await Task.Delay(3000);
                    Svc.Toasts.ShowError(message);
                }
            });
        }
    }

    static void DutyState_DutyWiped(object sender, ushort e)
    {
        if (!Player.Available) return;
        DataCenter.ResetAllLastActions();
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
            .Where(c => c.ObjectId != Player.Object.ObjectId)
#endif
            .Select(c =>
            {
                if (!RotationUpdater.AuthorHashes.TryGetValue(EncryptString(c), out var nameDesc)) nameDesc = string.Empty;
                return (c, nameDesc);
            })
            .Where(p => !string.IsNullOrEmpty(p.nameDesc));

        foreach (var (c, nameDesc) in authors)
        {
            while (!c.IsTargetable() && !DataCenter.InCombat)
            {
                await Task.Delay(100);
            }

#if DEBUG
#else
            Svc.Targets.Target = c;
            Chat.Instance.SendMessage($"/{_macroToAuthor[new Random().Next(_macroToAuthor.Count)]} <t>");
#endif
            var message = new SeString(new IconPayload(BitmapFontIcon.Mentor),

                          new UIForegroundPayload(31),
                          new PlayerPayload(c.Name.TextValue, c.HomeWorld.Id),
                          UIForegroundPayload.UIForegroundOff,

                          new TextPayload($"({nameDesc}) is one of the authors of "),

                          new IconPayload(BitmapFontIcon.DPS),
                          RotationSolverPlugin.LinkPayload,
                          new UIForegroundPayload(31),
                          new TextPayload("Rotation Solver"),
                          UIForegroundPayload.UIForegroundOff,
                          RawPayload.LinkTerminator,

                          new TextPayload(". So say hello to him/her!"));

            Svc.Chat.PrintChat(new Dalamud.Game.Text.XivChatEntry()
            {
                Message = message,
                Type = Dalamud.Game.Text.XivChatType.Notice,
            });
            UIModule.PlaySound(20, 0, 0, 0);

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
            PluginLog.Warning(ex, "Failed to read the player's name and world.");
            return string.Empty;
        }
    }
}
