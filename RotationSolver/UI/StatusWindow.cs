using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;

using Lumina.Excel.GeneratedSheets;

using RotationSolver.Commands;
using RotationSolver.Localization;
using RotationSolver.TextureItems;

namespace RotationSolver.UI
{
    internal class StatusWindow : InfoWindow
    {
        private static string AoeString => "AOE: " + (Basic.Configuration.PluginConfiguration.GetValue(SettingsCommand.UseAOEAction) 
            && (DataCenter.StateType != StateCommandType.Manual 
            || Basic.Configuration.PluginConfiguration.GetValue(SettingsCommand.UseAOEWhenManual)) ? "On" : "Off");
        private static string PreventString => "Prevent: " + (Basic.Configuration.PluginConfiguration.GetValue(SettingsCommand.PreventActions) ? "On" : "Off");
        private static string BurstString => "Burst: " + (Basic.Configuration.PluginConfiguration.GetValue(SettingsCommand.AutoBurst) ? "On" : "Off");
        internal static string SpecialString => (DataCenter.SpecialTimeLeft < 0 ? string.Empty : $" - {RSCommands._specialString}: {DataCenter.SpecialTimeLeft:F2}s");

        public StatusWindow() : base(nameof(StatusWindow))
        {

        }

        public override void Draw()
        {
            ImGui.Text("Status: " + RSCommands._stateString + SpecialString);
            ImGui.Separator();
            ImGui.Text(AoeString);
            ImGui.Text(PreventString);
            ImGui.Text(BurstString);
        }
    }
}
