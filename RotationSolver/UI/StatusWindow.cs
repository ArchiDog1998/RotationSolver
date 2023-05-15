using RotationSolver.Basic.Configuration;
using RotationSolver.Commands;

namespace RotationSolver.UI
{
    internal class StatusWindow : InfoWindow
    {
        private static string AoeString => "AOE: " + (PluginConfiguration.GetValue(SettingsCommand.UseAOEAction) 
            && (DataCenter.StateType != StateCommandType.Manual 
            || PluginConfiguration.GetValue(SettingsCommand.UseAOEWhenManual)) ? "On" : "Off");
        private static string PreventString => "Prevent: " + (PluginConfiguration.GetValue(SettingsCommand.PreventActions) ? "On" : "Off");
        private static string BurstString => "Burst: " + (PluginConfiguration.GetValue(SettingsCommand.AutoBurst) ? "On" : "Off");

        public StatusWindow() : base(nameof(StatusWindow))
        {

        }

        public override void Draw()
        {
            ImGui.Text("Status: " + RSCommands.EntryString);
            ImGui.Separator();
            ImGui.Text(AoeString);
            ImGui.Text(PreventString);
            ImGui.Text(BurstString);
        }
    }
}
