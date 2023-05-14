using RotationSolver.Localization;

namespace RotationSolver.Commands
{
    public static partial class RSCommands
    {
        private static string _stateString = "Off", _specialString = string.Empty;
        private static string AoeString => "AOE " + (Basic.Configuration.PluginConfiguration.GetValue(SettingsCommand.UseAOEAction) && (DataCenter.StateType != StateCommandType.Manual || Basic.Configuration.PluginConfiguration.GetValue(SettingsCommand.UseAOEWhenManual)) ? "on" : "off");

        private static string PreventString => "Prevent " + (Basic.Configuration.PluginConfiguration.GetValue(SettingsCommand.PreventActions) ? "on" : "off");

        private static string BurstString => "Burst " + (Basic.Configuration.PluginConfiguration.GetValue(SettingsCommand.AutoBurst) ? "on" : "off");


        internal static string EntryString =>
            $"{_stateString} ({AoeString}, {PreventString}, {BurstString})" +  (DataCenter.SpecialTimeLeft < 0 ? string.Empty : $" - {_specialString}: {DataCenter.SpecialTimeLeft:F2}s");

        private static void UpdateToast()
        {
            if (!Service.Config.ShowInfoOnToast) return;

            Service.ToastGui.ShowQuest(" " + EntryString, new Dalamud.Game.Gui.Toast.QuestToastOptions()
            {
                IconId = 101,
            });
        }

        private static unsafe void DoStateCommandType(StateCommandType stateType) => DoOneCommandType(EnumTranslations.ToSayout, role =>
        {
            if (DataCenter.StateType == StateCommandType.Auto
                && stateType == StateCommandType.Auto)
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

        private static void DoSpecialCommandType(SpecialCommandType specialType, bool sayout = true) => DoOneCommandType(sayout ? EnumTranslations.ToSayout : (s, r) => string.Empty, role =>
        {
            _specialString = specialType.ToSpecialString(role);
            DataCenter.SetSpecialType(specialType);
            if (sayout) UpdateToast();
            return specialType;
        });

        private static void DoOneCommandType<T>(Func<T, JobRole, string> sayout, Func<JobRole, T> doingSomething)
            where T : struct, Enum
        {
            //Get job role.
            var role = Service.Player.ClassJob.GameData.GetJobRole();

            T type = doingSomething(role);

            //Saying out.
            if (Service.Config.SayOutStateChanged) SpeechHelper.Speak(sayout(type, role));
        }
    }
}
