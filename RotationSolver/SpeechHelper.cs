using RotationSolver.Basic;
using System.Diagnostics;
using System.Speech.Synthesis;
using System.Text;

namespace RotationSolver;

internal static class SpeechHelper
{
    static SpeechSynthesizer _speech;
    internal static void Speak(string text)
    {
        try
        {
            _speech ??= new SpeechSynthesizer();
            _speech.Volume = Service.Config.VoiceVolume;
            _speech.SpeakAsyncCancelAll();
            _speech.SpeakAsync(text);

            //When I got win 11, I'll do that.
            //_speech.GetInstalledVoices().FirstOrDefault().VoiceInfo;
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