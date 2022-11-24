using System.Resources;
using System.Reflection;
using System.Globalization;
using System.IO;
using Newtonsoft.Json;
using CheapLoc;

namespace XIVAutoAttack.Localization
{
    internal class Strings
    {
        public struct ConfigWindow
        {
            public static string Header => Loc.Localize("ConfigWindow_Header", "Auto Attack Settings v");
        }
    }
}
