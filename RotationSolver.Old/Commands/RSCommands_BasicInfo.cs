using Dalamud.Game.Command;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using RotationSolver.Data;
using RotationSolver.Localization;
using System;
using System.Linq;

namespace RotationSolver.Commands
{
    internal static partial class RSCommands
    {
        internal const string _command = "/rotation";

        internal static TargetingType TargetingType
        {
            get
            {
                if (Service.Configuration.TargetingTypes.Count == 0)
                {
                    Service.Configuration.TargetingTypes.Add(TargetingType.Big);
                    Service.Configuration.TargetingTypes.Add(TargetingType.Small);
                    Service.Configuration.Save();
                }

                return Service.Configuration.TargetingTypes[Service.Configuration.TargetingIndex %= Service.Configuration.TargetingTypes.Count];
            }
        }

        internal static void Enable()
            => Service.CommandManager.AddHandler(_command, new CommandInfo(OnCommand)
            {
                HelpMessage = LocalizationManager.RightLang.Commands_Rotation,
                ShowInHelp = true,
            });

        internal static void Disable() => Service.CommandManager.RemoveHandler(_command);

        private static void OnCommand(string command, string arguments)
        {
            DoOneCommand(arguments);
        }

        private static void DoOneCommand(string str)
        {
            if (TryGetOneEnum<StateCommandType>(str, out var stateType))
            {
                DoStateCommandType(stateType);
            }
            else if (TryGetOneEnum<SpecialCommandType>(str, out var specialType))
            {
                DoSpecialCommandType(specialType);
            }
            else if (TryGetOneEnum<OtherCommandType>(str, out var otherType))
            {
                DoOtherCommand(otherType, str.Substring(otherType.ToString().Length).Trim());
            }
            else
            {
                RotationSolverPlugin.OpenConfigWindow();
            }
        }

        private static bool TryGetOneEnum<T>(string str, out T type) where T : struct, Enum
        {
            type = default;
            try
            {
                type = Enum.GetValues<T>().First(c => str.StartsWith(c.ToString(), StringComparison.OrdinalIgnoreCase));
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
