using Dalamud.Plugin;
using ECommons.DalamudServices;
using ECommons.Reflection;
using RotationSolver.Localization;

namespace RotationSolver;

internal static class SpeechHelper
{
    static IDalamudPlugin _textToTalk = null;
    static MethodInfo _say = null;
    static object _manager = null;
    static MethodInfo _stop = null;
    static bool _showed = false;
    internal static void Speak(string text)
    {
        if(_textToTalk == null)
        {
            if (!DalamudReflector.TryGetDalamudPlugin("TextToTalk", out _textToTalk))
            {
                if (!_showed)
                {
                    _showed = true;
                    Svc.Chat.PrintError(LocalizationManager.RightLang.TextToTalkWarning);
                }
            }
        }
        _say ??= _textToTalk?.GetType().GetRuntimeMethods().FirstOrDefault(m => m.Name == "Say");
        _manager ??= _textToTalk?.GetType().GetRuntimeFields().FirstOrDefault(m => m.Name == "backendManager").GetValue(_textToTalk);
        _stop ??= _manager?.GetType().GetRuntimeMethods().FirstOrDefault(m => m.Name == "CancelAllSpeech");

        _stop?.Invoke(_manager, Array.Empty<object>());
        _say?.Invoke(_textToTalk, new object[] { null, text, 1 });
    }
}