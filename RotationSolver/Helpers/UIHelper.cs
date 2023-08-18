using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using ECommons.DalamudServices;

namespace RotationSolver.Basic.Helpers;

public static class UIHelper
{
    public static void ShowWarning(this string message, int times = 3, DalamudLinkPayload link = null)
    {
        if (Service.Config.GetValue(Configuration.PluginConfigBool.HideWarning)) return;

        var seString = link == null
            ? new SeString(
              new IconPayload(BitmapFontIcon.DPS),
              RotationSolverPlugin.OpenLinkPayload,
              new UIForegroundPayload(31),
              new TextPayload("Rotation Solver"),
              UIForegroundPayload.UIForegroundOff,
              RawPayload.LinkTerminator,
              new TextPayload(": " + message))

            : new SeString(
              new IconPayload(BitmapFontIcon.DPS),
              RotationSolverPlugin.OpenLinkPayload,
              new UIForegroundPayload(31),
              new TextPayload("Rotation Solver"),
              UIForegroundPayload.UIForegroundOff,
              RawPayload.LinkTerminator,
              link,
              new TextPayload(": " + message),
              RawPayload.LinkTerminator);

        Svc.Chat.PrintChat(new Dalamud.Game.Text.XivChatEntry()
        {
            Message = seString,
            Type = Dalamud.Game.Text.XivChatType.ErrorMessage,
        });

        Task.Run(async () =>
        {
            for (int i = 0; i < times; i++)
            {
                await Task.Delay(3000);
                Svc.Toasts.ShowError(message);
            }
        });
    }
}
