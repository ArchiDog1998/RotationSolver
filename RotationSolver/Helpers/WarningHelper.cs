using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using ECommons.DalamudServices;

namespace RotationSolver.Basic.Helpers;

public static class WarningHelper
{
    private static Queue<string> _showWarnings = new Queue<string>();
    private static bool _run = false;

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
              new TextPayload(": " + message),

              RotationSolverPlugin.HideWarningLinkPayload,
              new TextPayload("(Hide Warning)"),
              RawPayload.LinkTerminator)

            : new SeString(
              new IconPayload(BitmapFontIcon.DPS),
              RotationSolverPlugin.OpenLinkPayload,
              new UIForegroundPayload(31),
              new TextPayload("Rotation Solver"),
              UIForegroundPayload.UIForegroundOff,
              RawPayload.LinkTerminator,
              new TextPayload(": "),
              link,
              new TextPayload(message),
              RawPayload.LinkTerminator,

              RotationSolverPlugin.HideWarningLinkPayload,
              new TextPayload("(Hide Warning)"),
              RawPayload.LinkTerminator);

        Svc.Chat.Print(new Dalamud.Game.Text.XivChatEntry()
        {
            Message = seString,
            Type = Dalamud.Game.Text.XivChatType.ErrorMessage,
        });

        for (int i = 0; i < times; i++)
        {
            _showWarnings.Enqueue(message);
        }

        if (!_run)
        {
            _run = true;
            Task.Run(RunShowError);
        }
    }

    private static async Task RunShowError()
    {
        while (_showWarnings.TryDequeue(out var message))
        {
            Svc.Toasts.ShowError(message);
            await Task.Delay(3000);
        }
        _run = false;
    }
}
