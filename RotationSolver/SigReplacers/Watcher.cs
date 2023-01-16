using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using ImGuiNET;
using RotationSolver.Actions;
using RotationSolver.Data;
using RotationSolver.Helpers;
using RotationSolver.Localization;
using RotationSolver.Updaters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using Action = Lumina.Excel.GeneratedSheets.Action;

namespace RotationSolver.SigReplacers
{
    public static class Watcher
    {
        public record ActionRec(DateTime UsedTime, Action action);

        private delegate void ReceiveAbiltyDelegate(uint sourceId, IntPtr sourceCharacter, IntPtr pos, IntPtr effectHeader, IntPtr effectArray, IntPtr effectTrail);

        private static Hook<ReceiveAbiltyDelegate> _receivAbilityHook;

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal static ActionID LastAction { get; set; } = 0;

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal static ActionID LastGCD { get; set; } = 0;

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal static ActionID LastAbility { get; set; } = 0;

        internal static TimeSpan TimeSinceLastAction => DateTime.Now - _timeLastActionUsed;

        private static DateTime _timeLastActionUsed = DateTime.Now;

        const int QUEUECAPACITY = 32;
        private static Queue<ActionRec> _actions = new Queue<ActionRec>(QUEUECAPACITY);

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal static ActionRec[] RecordActions => _actions.Reverse().ToArray();

        internal static unsafe void Enable()
        {
            _receivAbilityHook = Hook<ReceiveAbiltyDelegate>.FromAddress(Service.Address.ReceiveAbilty, ReceiveAbilityEffect);

            _receivAbilityHook?.Enable();
        }


        private static void ReceiveAbilityEffect(uint sourceId, IntPtr sourceCharacter, IntPtr pos, IntPtr effectHeader, IntPtr effectArray, IntPtr effectTrail)
        {
            _receivAbilityHook.Original(sourceId, sourceCharacter, pos, effectHeader, effectArray, effectTrail);

            //不是自己放出来的
            if (Service.ClientState.LocalPlayer == null || sourceId != Service.ClientState.LocalPlayer.ObjectId) return;

            //不是一个Spell
            if (Marshal.ReadByte(effectHeader, 31) != 1) return;

            //获得行为
            var action = Service.DataManager.GetExcelSheet<Action>().GetRow((uint)Marshal.ReadInt32(effectHeader, 0x8));

            //获得目标
            var tar = Service.ObjectTable.SearchById((uint)Marshal.ReadInt32(effectHeader)) ?? Service.ClientState.LocalPlayer;

            //获得身为技能是否正确flag
            var flag = Marshal.ReadByte(effectArray + 3);
            RecordAction(tar, action, flag);
        }

        private static unsafe void RecordAction(GameObject tar, Action action, byte flag)
        {
            var id = (ActionID)action.RowId;

            //Record
            switch (action.GetActinoType())
            {
                case ActionCate.Spell: //魔法
                case ActionCate.Weaponskill: //战技
                    LastGCD = id;
                    break;
                case ActionCate.Ability: //能力
                    LastAbility = id;
                    break;
                default:
                    return;
            }
            _timeLastActionUsed = DateTime.Now;
            LastAction = id;

            if (_actions.Count >= QUEUECAPACITY)
            {
                _actions.Dequeue();
            }
            _actions.Enqueue(new ActionRec(_timeLastActionUsed, action));

            //Macro
            foreach (var item in Service.Configuration.Events)
            {
                if (item.Name != action.Name) continue;
                if (item.MacroIndex < 0 || item.MacroIndex > 99) break;

                MacroUpdater.Macros.Enqueue(new MacroItem(tar, item.IsShared ? RaptureMacroModule.Instance->Shared[item.MacroIndex] :
                    RaptureMacroModule.Instance->Individual[item.MacroIndex]));
            }

#if DEBUG
            if (flag != 0) Service.ChatGui.Print($"{action.Name}, {flag}");
#endif

            //事后骂人！
            if (Service.Configuration.ShowLocationWrong
                && StatusHelper.ActionLocations.TryGetValue(id, out var loc)
                && loc.Tags.Length > 0 && !loc.Tags.Contains(flag))
            {
                //Todo: Damage Icon should be something intersting.
                Service.FlyTextGui.AddFlyText(Dalamud.Game.Gui.FlyText.FlyTextKind.NamedIcon, 0, 0, 0, string.Format(LocalizationManager.RightLang.Action_WrongLocation, loc.Loc.ToName()), "",
                    ImGui.GetColorU32(new Vector4(0.4f, 0, 0, 1)), action.Icon, 0);
                if (!string.IsNullOrEmpty(Service.Configuration.LocationWrongText))
                {
                    Speak(Service.Configuration.LocationWrongText);
                }
            }
        }

        internal static void Speak(string text, bool wait = false)
        {
            ExecuteCommand(
                $@"Add-Type -AssemblyName System.speech; 
                $speak = New-Object System.Speech.Synthesis.SpeechSynthesizer; 
                $speak.Volume = ""{Service.Configuration.VoiceVolume}"";
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

                    if (wait)
                        process.WaitForExit();
                }
            }
        }


        public static void Dispose()
        {
            _receivAbilityHook?.Dispose();
        }
    }
}
