using Lumina.Excel.GeneratedSheets;
using RotationSolver.Data;
using RotationSolver.Localization;
using RotationSolver.SigReplacers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotationSolver.Commands
{
    internal static partial class RotationSolverCommands
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

        internal static void ResetSpecial() => DoSpecialCommandType(SpecialCommandType.EndSpecial);

        private static void DoStateCommandType(StateCommandType stateType)
        {
            DoOneCommandType(stateType, EnumTranslations.ToSayout, role =>
            {
                if (StateType == StateCommandType.Smart
                && stateType == StateCommandType.Smart)
                {
                    Service.Configuration.TargetingIndex += 1;
                    Service.Configuration.TargetingIndex %= Service.Configuration.TargetingTypes.Count;
                }

                StateType = stateType;

                _stateString = stateType.ToStateString(role);
            });
        }

        private static void DoSpecialCommandType(SpecialCommandType specialType)
        {
            DoOneCommandType(specialType, EnumTranslations.ToSayout, role =>
            {
                _specialType = specialType;

                _specialString = specialType.ToSpecialString(role);
                _specialStateStartTime = DateTime.Now;
            });
        }

        private static void DoOneCommandType<T>(T type, Func<T, JobRole, string> sayout, Action<JobRole> doingSomething)
            where T : struct, Enum
        {
            //Get jobrole.
            var role = Service.DataManager.GetExcelSheet<ClassJob>().GetRow(
                Service.ClientState.LocalPlayer.ClassJob.Id).GetJobRole();

            //Saying out.
            if (Service.Configuration.AutoSayingOut) Watcher.Speak(sayout(type, role));

            doingSomething(role);
        }
    }
}
