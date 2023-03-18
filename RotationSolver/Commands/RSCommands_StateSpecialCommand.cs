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
        private static string _stateString = "Off", _specialString = string.Empty;
        internal static string EntryString =>
            _stateString +  (DataCenter.SpecialTimeLeft < 0 ? string.Empty : $" - {_specialString}: {DataCenter.SpecialTimeLeft:F2}s");

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
            if (DataCenter.StateType == StateCommandType.Smart
            && stateType == StateCommandType.Smart)
            {
                Service.Config.TargetingIndex += 1;
                Service.Config.TargetingIndex %= Service.Config.TargetingTypes.Count;
            }

            DataCenter.StateType = stateType;

            UpdateStateNamePlate();

            _stateString = stateType.ToStateString(role);
            UpdateToast();
        });

        public static unsafe void UpdateStateNamePlate()
        {
            if (Service.Player == null) return;

            Service.Player.SetNamePlateIcon(
                DataCenter.StateType == StateCommandType.Cancel ? 0u : (uint)Service.Config.NamePlateIconId);
        }

        private static void DoSpecialCommandType(SpecialCommandType specialType, bool sayout = true) => DoOneCommandType(specialType, sayout ? EnumTranslations.ToSayout : (s, r) => string.Empty, role =>
        {
            _specialString = specialType.ToSpecialString(role);
            DataCenter.SetSpecialType(specialType);
            if (sayout) UpdateToast();
        });

        private static void DoOneCommandType<T>(T type, Func<T, JobRole, string> sayout, Action<JobRole> doingSomething)
            where T : struct, Enum
        {
            //Get jobrole.
            var role = Service.Player.ClassJob.GameData.GetJobRole();

            doingSomething(role);

            //Saying out.
            if (Service.Config.SayOutStateChanged) SpeechHelper.Speak(sayout(type, role));
        }
    }
}
