using Dalamud.Logging;
using RotationSolver.Basic;
using System.Diagnostics;
using System.Speech.Synthesis;
using System.Text;

namespace RotationSolver;

internal static class SpeechHelper
{
    static SpeechSynthesizer _speech;
    public static string[] VoiceNames { get; private set; }
    static bool _errored;
    internal static void Speak(string text)
    {
        try
        {
            try
            {
                VoiceNames ??= _speech.GetInstalledVoices().Select(v => v.VoiceInfo.Name).ToArray();
                _speech ??= new SpeechSynthesizer();
                _speech.Volume = Service.Config.VoiceVolume;
                _speech.SelectVoice(Service.Config.VoiceName);
            }
            catch(Exception ex)
            {
                if (!_errored)
                {
                    _errored = true;
                    PluginLog.Error(ex, "Speech Exception");
                }
            }
            _speech.SpeakAsyncCancelAll();
            _speech.SpeakAsync(text);
        }
        catch
        {
            ExecuteCommand(
                $@"Add-Type -AssemblyName System.speech; 
                    $speak = New-Object System.Speech.Synthesis.SpeechSynthesizer; 
                    $speak.Volume = ""{Service.Config.VoiceVolume}"";
                    $speak.Speak(""{text}"");");

            void ExecuteCommand(string command)
            {
                string path = Path.GetTempPath() + Guid.NewGuid() + ".ps1";

                // make sure to be using System.Text
                using (StreamWriter sw = new StreamWriter(path, false, Encoding.UTF8))
                {
                    sw.Write(command);

                    ProcessStartInfo start = new ProcessStartInfo()
                    {
                        FileName = @"C:\Windows\System32\windowspowershell\v1.0\powershell.exe",
                        LoadUserProfile = false,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        Arguments = $"-executionpolicy bypass -File {path}",
                        WindowStyle = ProcessWindowStyle.Hidden
                    };

                    Process process = Process.Start(start);
                }
            }
        }
    }
}