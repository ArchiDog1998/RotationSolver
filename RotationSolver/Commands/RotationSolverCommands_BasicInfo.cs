using Dalamud.Game.Command;
using Lumina.Excel.GeneratedSheets;
using RotationSolver.Actions.BaseAction;
using RotationSolver.Combos.CustomCombo;
using RotationSolver.Data;
using RotationSolver.Helpers;
using RotationSolver.Localization;
using RotationSolver.SigReplacers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotationSolver.Commands
{
    internal static partial class RotationSolverCommands
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
            string[] array = arguments.Split();

            if (!array.Any()) return;

            DoOneCommand(array[0]);
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
