using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.SubKinds;
using FFXIVClientStructs.FFXIV.Client.UI;
using Lumina.Excel.GeneratedSheets;
using RotationSolver.Basic;
using RotationSolver.Basic.Helpers;
using RotationSolver.Commands;
using RotationSolver.Localization;
using System.Security.Cryptography;
using System.Text;

namespace RotationSolver.Updaters;

internal class SocialUpdater
{
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
    public static bool InHighEndDuty { get; private set; } = false;
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
        InHighEndDuty = HighEndDuties.Any(t => t.RowId == territory.RowId);
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

    internal static async void UpdateSocial()
    {
        if (DataCenter.InCombat || DataCenter.PartyMembers.Count() < 2) return;
        if (_canSaying && CanSocial)
        {
            _canSaying = false;
            await Task.Delay(new Random().Next(3000, 5000));

#if DEBUG
            Service.ChatGui.PrintError("Macro now.");
#endif
            Service.Config.DutyStart.AddMacro();
            await Task.Delay(new Random().Next(1000, 1500));
            SayHelloToAuthor();
        }
    }

    private static async void SayHelloToAuthor()
    {
        var author = DataCenter.AllianceMembers.OfType<PlayerCharacter>()
            .FirstOrDefault(c =>
#if DEBUG
#else
            c.ObjectId != Service.ClientState.LocalPlayer.ObjectId &&
#endif
            RotationUpdater.AuthorHashes.Contains(EncryptString(c)));

        if (author != null)
        {
            while(!author.IsTargetable() && !DataCenter.InCombat)
            {
                await Task.Delay(100);
            }

#if DEBUG
            Service.ChatGui.PrintError("Author Time");
#else
            Service.TargetManager.SetTarget(author);
            Service.SubmitToChat($"/{_macroToAuthor[new Random().Next(_macroToAuthor.Count)]} <t>");
            Service.ChatGui.PrintChat(new Dalamud.Game.Text.XivChatEntry()
            {
                Message = string.Format(LocalizationManager.RightLang.Commands_SayHelloToAuthor, author.Name),
                Type = Dalamud.Game.Text.XivChatType.Notice,
            });
            UIModule.PlaySound(20, 0, 0, 0);
            Service.TargetManager.SetTarget(null);
#endif
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
