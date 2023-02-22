using Lumina.Excel.GeneratedSheets;
using RotationSolver.Data;
using RotationSolver.Helpers;
using RotationSolver.Localization;
using RotationSolver.SigReplacers;
using System;

namespace RotationSolver.Commands
{
    internal static partial class RSCommands
    {
        private static DateTime _specialStateStartTime = DateTime.MinValue;
        private static double SpecialTimeLeft => Service.Configuration.SpecialDuration - (DateTime.Now - _specialStateStartTime).TotalSeconds;

        private static SpecialCommandType _specialType = SpecialCommandType.EndSpecial;
        internal static SpecialCommandType SpecialType =>
             SpecialTimeLeft < 0 ? SpecialCommandType.EndSpecial : _specialType;

        internal static StateCommandType StateType { get; private set; } = StateCommandType.Cancel;


        private static string _stateString = "Off", _specialString = string.Empty;
        internal static string EntryString =>
            _stateString + (SpecialTimeLeft < 0 ? string.Empty : $" - {_specialString}: {SpecialTimeLeft:F2}s");

        private static void UpdateToast()
        {
            if (!Service.Configuration.ShowInfoOnToast) return;

            Service.ToastGui.ShowQuest(" " + EntryString, new Dalamud.Game.Gui.Toast.QuestToastOptions()
            {
                IconId = 101,
            });
        }

        private static unsafe void DoStateCommandType(StateCommandType stateType) => DoOneCommandType(stateType, EnumTranslations.ToSayout, role =>
        {
            if (StateType == StateCommandType.Smart
            && stateType == StateCommandType.Smart)
            {
                Service.Configuration.TargetingIndex += 1;
                Service.Configuration.TargetingIndex %= Service.Configuration.TargetingTypes.Count;
            }

            StateType = stateType;

            UpdateStateNamePlate();

            _stateString = stateType.ToStateString(role);
            UpdateToast();
        });

        public static unsafe void UpdateStateNamePlate()
        {
            if (Service.ClientState.LocalPlayer == null) return;

            Service.ClientState.LocalPlayer.GetAddress()->NamePlateIconId =
                StateType == StateCommandType.Cancel ? 0u : (uint)Service.Configuration.NamePlateIconId;
        }

        private static void DoSpecialCommandType(SpecialCommandType specialType, bool sayout = true) => DoOneCommandType(specialType, sayout ? EnumTranslations.ToSayout : (s, r) => string.Empty, role =>
        {
            _specialType = specialType;
            _specialString = specialType.ToSpecialString(role);

            _specialStateStartTime = specialType == SpecialCommandType.EndSpecial ? DateTime.MinValue : DateTime.Now;
            if (sayout) UpdateToast();
        });

        private static void DoOneCommandType<T>(T type, Func<T, JobRole, string> sayout, Action<JobRole> doingSomething)
            where T : struct, Enum
        {
            //Get jobrole.
            var role = Service.DataManager.GetExcelSheet<ClassJob>().GetRow(
                Service.ClientState.LocalPlayer.ClassJob.Id).GetJobRole();

            doingSomething(role);

            //Saying out.
            if (Service.Configuration.SayOutStateChanged) Watcher.Speak(sayout(type, role));
        }
    }
}
