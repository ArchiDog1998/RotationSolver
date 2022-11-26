using System.Resources;
using System.Reflection;
using System.Globalization;
using System.IO;
using Newtonsoft.Json;

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
    }
}
