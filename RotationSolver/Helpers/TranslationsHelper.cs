using Dalamud.Utility;
using RotationSolver.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotationSolver.Helpers;
internal static class TranslationsHelper
{
    public static string GetName(this Enum type)
    {
        return (type.GetType().FullName ?? string.Empty + type.ToString()).Local(type.GetAttribute<DescriptionAttribute>()?.Description ?? type.ToString());
    }
}
