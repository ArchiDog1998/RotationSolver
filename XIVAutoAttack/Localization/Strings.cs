using System.Resources;
using System.Reflection;
using System.Globalization;
using System.IO;
using Newtonsoft.Json;
using Lumina.Excel.GeneratedSheets;

namespace XIVAutoAttack.Localization
{
    internal class Strings
    {
        public string ConfigWindow_Header { get; set; } = "Auto Attack Settings v";
        public string ConfigWindow_DebugItem { get; set; } = "Debug";
        public string ConfigWindow_AboutItem { get; set; } = "About";
        public string ConfigWindow_AttackItem { get; set; } = "Attack";
        public string ConfigWindow_ParamItem { get; set; } = "Param";
        public string ConfigWindow_EventsItem { get; set; } = "Events";
        public string ConfigWindow_ActionsItem { get; set; } = "Actions";
        public string ConfigWindow_HelpItem { get; set; } = "Help";

        public string ConfigWindow_ActionItem_Description { get; set; }
            = "In this panel, you can modify the use conditions for every action.";

        public string ConfigWindow_HelpItem_Description { get; set; }
            = "In this panel, you can see all commands for combat that can use in for XIV Auto Attack";

        public string Configwindow_HelpItem_AttackSmart { get; set; }
            = "Start Attacking in auto-target mode when not in combat. Switch the targeting condition in combat.";

        public string Configwindow_HelpItem_AttackManual { get; set; }
            = "Start Attacking in manual mode.";

        public string Configwindow_HelpItem_AttackCancel { get; set; }
            = "Stop Attacking, please always do it!";

        public string Configwindow_HelpItem_HealArea { get; set; }
            = "Open one window period to use one or serveral range healing actions.";
        public string Configwindow_HelpItem_HealSingle { get; set; }
            = "Open one window period to use one or serveral single healing actions.";
        public string Configwidow_HelpItem_DefenseArea { get; set; }
            = "Open one window period to use one or serveral range defense actions.";
        public string Configwidow_HelpItem_DefenseSingle { get; set; }
            = "Open one window period to use one or serveral single defense actions.";
        public string Configwidow_HelpItem_EsunaShield { get; set; }
         = "Open one window period to use Esuna,tank stance skills or True North.";
        public string Configwidow_HelpItem_RaiseShirk { get; set; }
         = "Open one window period to use Raise or Shirk";
        public string Configwidow_HelpItem_AntiRepulsion { get; set; }
        = "Open one window period to use knockback-penalty skill";
        public string Configwidow_HelpItem_BreakProvoke { get; set; }
        = "Open one window period to use break-combo or provoke skill";
        public string Configwidow_HelpItem_Move { get; set; }
        = "Open one window period to use move skill";
    }
}
