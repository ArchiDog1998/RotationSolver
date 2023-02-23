using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.SubKinds;
using FFXIVClientStructs.FFXIV.Client.UI;
using Lumina.Excel.GeneratedSheets;
using RotationSolver.Commands;
using RotationSolver.Data;
using RotationSolver.Helpers;
using RotationSolver.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RotationSolver.Updaters;

internal class SocialUpdater
{
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

    internal static void Enable()
    {
        Service.DutyState.DutyStarted += DutyState_DutyStarted;
        Service.DutyState.DutyCompleted += DutyState_DutyCompleted;
    }

    static async void DutyState_DutyCompleted(object sender, ushort e)
    {
        RSCommands.CancelState();
        await Task.Delay(new Random().Next(4000, 6000));

        Service.Configuration.DutyEnd.AddMacro();
    }

    static async void DutyState_DutyStarted(object sender, ushort e)
    {
        var territory = Service.DataManager.GetExcelSheet<TerritoryType>().GetRow(e);
        if (territory?.ContentFinderCondition?.Value?.HighEndDuty ?? false)
        {
            var str = territory.PlaceName?.Value?.Name.ToString() ?? "High-end Duty";
            Service.ToastGui.ShowError(string.Format(LocalizationManager.RightLang.HighEndWarning, str));
        }

        await Task.Delay(new Random().Next(1000));

        var author = TargetUpdater.AllianceMembers.OfType<PlayerCharacter>()
            .FirstOrDefault(c => c.ObjectId != Service.ClientState.LocalPlayer.ObjectId
            && ConfigurationHelper.AuthorKeys.Contains(EncryptString(c)));

        if (author != null)
        {
            Service.TargetManager.SetTarget(author);
            RSCommands.SubmitToChat($"/{macroToAuthor[new Random().Next(macroToAuthor.Count)]} <t>");
            Service.ChatGui.PrintChat(new Dalamud.Game.Text.XivChatEntry()
            {
                Message = string.Format(LocalizationManager.RightLang.Commands_SayHelloToAuthor, author.Name),
                Type = Dalamud.Game.Text.XivChatType.Notice,
            });
            UIModule.PlaySound(20, 0, 0, 0);
            Service.TargetManager.SetTarget(null);
        }

        await Task.Delay(new Random().Next(1000));

        Service.Configuration.DutyStart.AddMacro();
    }

    internal static void Disable()
    {
        Service.DutyState.DutyStarted -= DutyState_DutyStarted;
        Service.DutyState.DutyCompleted -= DutyState_DutyCompleted;
    }

    internal static string EncryptString(PlayerCharacter player)
    {
        byte[] inputByteArray = Encoding.UTF8.GetBytes(player.HomeWorld.GameData.InternalName.ToString() + " - " + player.Name.ToString() + "U6Wy.zCG");

        var tmpHash = MD5.Create().ComputeHash(inputByteArray);
        var retB = Convert.ToBase64String(tmpHash.ToArray());
        return retB;
    }
}
