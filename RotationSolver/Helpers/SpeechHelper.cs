using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Plugin;
using ECommons.DalamudServices;
using ECommons.Reflection;

namespace RotationSolver.Helpers;

internal static class SpeechHelper
{
    static IDalamudPlugin? _textToTalk = null;
    static MethodInfo? _say = null;
    static object? _manager = null;
    static MethodInfo? _stop = null;

    internal static void Speak(string text)
    {
        if (_textToTalk == null)
        {
            if (!DalamudReflector.TryGetDalamudPlugin("TextToTalk", out _textToTalk))
            {
                return;
            }
        }

        _say ??= _textToTalk?.GetType().GetRuntimeMethods().FirstOrDefault(m => m.Name == "Say");
        _manager ??= _textToTalk?.GetType().GetRuntimeFields().FirstOrDefault(m => m.Name == "backendManager")?.GetValue(_textToTalk);
        _stop ??= _manager?.GetType().GetRuntimeMethods().FirstOrDefault(m => m.Name == "CancelAllSpeech");

        try
        {
            _stop?.Invoke(_manager, []);
            try
            {
                _say?.Invoke(_textToTalk, [null, text, 1]);
            }
            catch
            {
                _say?.Invoke(_textToTalk, [null, new SeString(new TextPayload("Rotation Solver")), text, 1]);
            }
        }
        catch (Exception ex)
        {
            Svc.Log.Warning(ex, "Something wrong with TTT.");
        }
    }
}