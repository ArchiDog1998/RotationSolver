using Lumina.Excel.GeneratedSheets;
using RotationSolver.Basic;
using RotationSolver.Basic.Data;
using RotationSolver.Basic.Helpers;
using RotationSolver.Localization;
using RotationSolver.SigReplacers;
using System;

namespace RotationSolver.Commands
{
    public static partial class RSCommands
    {
        private static DateTime _specialStateStartTime = DateTime.MinValue;
        private static double SpecialTimeLeft => Service.Config.SpecialDuration - (DateTime.Now - _specialStateStartTime).TotalSeconds;

        private static SpecialCommandType _specialType = SpecialCommandType.EndSpecial;
        public static SpecialCommandType SpecialType =>
             SpecialTimeLeft < 0 ? SpecialCommandType.EndSpecial : _specialType;

        public static StateCommandType StateType { get; private set; } = StateCommandType.Cancel;


        private static string _stateString = "Off", _specialString = string.Empty;
        internal static string EntryString =>
            _stateString + (SpecialTimeLeft < 0 ? string.Empty : $" - {_specialString}: {SpecialTimeLeft:F2}s");

        private static void UpdateToast()
        {
            if (!Service.Config.ShowInfoOnToast) return;

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
                Service.Config.TargetingIndex += 1;
                Service.Config.TargetingIndex %= Service.Config.TargetingTypes.Count;
            }

            StateType = stateType;

            UpdateStateNamePlate();

            _stateString = stateType.ToStateString(role);
            UpdateToast();
        });

        public static unsafe void UpdateStateNamePlate()
        {
            if (Service.Player == null) return;

            Service.Player.SetNamePlateIcon(
                StateType == StateCommandType.Cancel ? 0u : (uint)Service.Config.NamePlateIconId);
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
            var role = Service.GetSheet<ClassJob>().GetRow(
                Service.Player.ClassJob.Id).GetJobRole();

            doingSomething(role);

            //Saying out.
            if (Service.Config.SayOutStateChanged) Watcher.Speak(sayout(type, role));
        }
    }
}
