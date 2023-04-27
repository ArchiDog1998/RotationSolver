using RotationSolver.Localization;

namespace RotationSolver.Commands
{
    public static partial class RSCommands
    {
        private static string _stateString = "Off", _specialString = string.Empty;
        private static string _aoeString => "AOE " + (Service.Config.GetValue(SettingsCommand.UseAOEAction) && (DataCenter.StateType != StateCommandType.Manual || Service.Config.GetValue(SettingsCommand.UseAOEWhenManual)) ? "on" : "off");

        private static string _preventString => "Prevent " + (Service.Config.GetValue(SettingsCommand.PreventActions) ? "on" : "off");

        internal static string EntryString =>
            $"{_stateString} ({_aoeString}, {_preventString})" +  (DataCenter.SpecialTimeLeft < 0 ? string.Empty : $" - {_specialString}: {DataCenter.SpecialTimeLeft:F2}s");

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

            if (Service.Config.ToggleManual 
                && DataCenter.StateType == StateCommandType.Manual
                && stateType == StateCommandType.Manual)
            {
                stateType = StateCommandType.Cancel;
            }

            DataCenter.StateType = stateType;

            UpdateStateNamePlate();

            _stateString = stateType.ToStateString(role);
            UpdateToast();
            return stateType;
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
            return specialType;
        });

        private static void DoOneCommandType<T>(T type, Func<T, JobRole, string> sayout, Func<JobRole, T> doingSomething)
            where T : struct, Enum
        {
            //Get jobrole.
            var role = Service.Player.ClassJob.GameData.GetJobRole();

            type =  doingSomething(role);

            //Saying out.
            if (Service.Config.SayOutStateChanged) SpeechHelper.Speak(sayout(type, role));
        }
    }
}
