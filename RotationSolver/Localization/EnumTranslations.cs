using Dalamud.Game.ClientState.Keys;
using RotationSolver.Commands;
using RotationSolver.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace RotationSolver.Localization
{
    internal static class EnumTranslations
    {
        internal static string ToName(this VirtualKey k) => k switch
            {
                VirtualKey.SHIFT => "SHIFT",
                VirtualKey.CONTROL => "CTRL",
                VirtualKey.MENU => "ALT",
                _ => k.ToString(),
            };

        public static string ToName(this JobRole role) => role switch
        {
            JobRole.None => LocalizationManager.RightLang.JobRole_None,
            JobRole.Tank => LocalizationManager.RightLang.JobRole_Tank,
            JobRole.Melee => LocalizationManager.RightLang.JobRole_Melee,
            JobRole.Ranged => LocalizationManager.RightLang.JobRole_Ranged,
            JobRole.Healer => LocalizationManager.RightLang.JobRole_Healer,
            JobRole.RangedPhysical => LocalizationManager.RightLang.JobRole_RangedPhysical,
            JobRole.RangedMagicial => LocalizationManager.RightLang.JobRole_RangedMagicial,
            JobRole.DiscipleoftheLand => LocalizationManager.RightLang.JobRole_DiscipleoftheLand,
            JobRole.DiscipleoftheHand => LocalizationManager.RightLang.JobRole_DiscipleoftheHand,
            _ => string.Empty,
        };

        public static string ToName(this TargetingType role) => role switch
        {
            TargetingType.Big => "Big",
            TargetingType.Small => "Small",
            TargetingType.HighHP => "High HP",
            TargetingType.LowHP => "Low HP",
            TargetingType.HighMaxHP => "High Max HP",
            TargetingType.LowMaxHP => "Low Max HP",
            _ => string.Empty,
        };

        internal static string ToSayout(this SpecialCommandType type, JobRole role) => type switch
        {
            SpecialCommandType.HealArea => "Start Heal Area",
            SpecialCommandType.HealSingle => "Start Heal Single",
            SpecialCommandType.DefenseArea => "Start Defense Area",
            SpecialCommandType.DefenseSingle => "Start Defense Single",
            SpecialCommandType.EsunaShield => "Start " + (role == JobRole.Tank ? "Shield" : "Esuna"),
            SpecialCommandType.RaiseShirk => "Start " + (role == JobRole.Tank ? "Shirk" : "Raise"),
            SpecialCommandType.MoveForward => "Start Move Forward",
            SpecialCommandType.MoveBack => "Start Move Back",
            SpecialCommandType.AntiRepulsion => "Start AntiRepulsion",
            SpecialCommandType.Break => "Start Break",
            SpecialCommandType.EndSpecial => "End Special",
            _ => string.Empty,
        };

        internal static string ToSayout(this StateCommandType type, JobRole role) => type switch
        {
            StateCommandType.Smart => "Start Smart " + RSCommands.TargetingType.ToName(),
            StateCommandType.Manual => "Start Manual",
            StateCommandType.Cancel => "Cancel",
            _ => string.Empty,
        };

        internal static string ToSpecialString(this SpecialCommandType type, JobRole role) => type switch
        {
            SpecialCommandType.HealArea => "Heal Area",
            SpecialCommandType.HealSingle => "Heal Single",
            SpecialCommandType.DefenseArea => "Defense Area",
            SpecialCommandType.DefenseSingle => "Defense Single",
            SpecialCommandType.EsunaShield => role == JobRole.Tank ? "Shield" : "Esuna",
            SpecialCommandType.RaiseShirk => role == JobRole.Tank ? "Shirk" : "Raise",
            SpecialCommandType.MoveForward => "Move Forward",
            SpecialCommandType.MoveBack => "Move Back",
            SpecialCommandType.AntiRepulsion => "AntiRepulsion",
            SpecialCommandType.Break => "Break",
            SpecialCommandType.EndSpecial => "End Special",
            _ => string.Empty,
        };

        internal static string ToStateString(this StateCommandType type, JobRole role) => type switch
        {
            StateCommandType.Smart => "Smart " + RSCommands.TargetingType.ToName(),
            StateCommandType.Manual => "Manual",
            StateCommandType.Cancel => "Off",
            _ => string.Empty,
        };

        internal static string ToHelp(this SpecialCommandType type) => type switch
        {
            SpecialCommandType.HealArea => LocalizationManager.RightLang.Configwindow_HelpItem_HealArea,
            SpecialCommandType.HealSingle => LocalizationManager.RightLang.Configwindow_HelpItem_HealSingle,
            SpecialCommandType.DefenseArea => LocalizationManager.RightLang.Configwindow_HelpItem_DefenseArea,
            SpecialCommandType.DefenseSingle => LocalizationManager.RightLang.Configwindow_HelpItem_DefenseSingle,
            SpecialCommandType.EsunaShield => LocalizationManager.RightLang.Configwindow_HelpItem_EsunaShield,
            SpecialCommandType.RaiseShirk => LocalizationManager.RightLang.Configwindow_HelpItem_RaiseShirk,
            SpecialCommandType.MoveForward => LocalizationManager.RightLang.Configwindow_HelpItem_MoveForward,
            SpecialCommandType.MoveBack => LocalizationManager.RightLang.Configwindow_HelpItem_MoveBack,
            SpecialCommandType.AntiRepulsion => LocalizationManager.RightLang.Configwindow_HelpItem_AntiRepulsion,
            SpecialCommandType.Break => LocalizationManager.RightLang.Configwindow_HelpItem_Break,
            SpecialCommandType.EndSpecial => LocalizationManager.RightLang.Configwindow_HelpItem_EndSpecial,
            _ => string.Empty,
        };

        internal static string ToHelp(this StateCommandType type) => type switch
        {
            StateCommandType.Smart => LocalizationManager.RightLang.Configwindow_HelpItem_AttackSmart,
            StateCommandType.Manual => LocalizationManager.RightLang.Configwindow_HelpItem_AttackManual,
            StateCommandType.Cancel => LocalizationManager.RightLang.Configwindow_HelpItem_AttackCancel,
            _ => string.Empty,
        };
    }
}
