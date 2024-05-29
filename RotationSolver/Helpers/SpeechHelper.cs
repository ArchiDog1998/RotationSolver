using System.Diagnostics;
using System.Text;

namespace RotationSolver.Helpers;

internal static class SpeechHelper
{
    internal static void Speak(string text)
    {
        if (!Service.Config.SayOutStateChanged) return;

        ExecuteCommand(
        $@"Add-Type -AssemblyName System.speech; 
               $speak = New-Object System.Speech.Synthesis.SpeechSynthesizer; 
               $speak.Volume = ""{Service.Config.VoiceVolume}"";
               $speak.Speak(""{text}"");");

        static void ExecuteCommand(string command)
        {
            string path = Path.GetTempPath() + Guid.NewGuid() + ".ps1";

            // make sure to be using System.Text
            using StreamWriter sw = new(path, false, Encoding.UTF8);
            sw.Write(command);

            ProcessStartInfo start = new()
            {
                FileName = @"C:\Windows\System32\windowspowershell\v1.0\powershell.exe",
                LoadUserProfile = false,
                UseShellExecute = false,
                CreateNoWindow = true,
                Arguments = $"-executionpolicy bypass -File {path}",
                WindowStyle = ProcessWindowStyle.Hidden
            };

            Process.Start(start);
        }
    }
}