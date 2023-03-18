using RotationSolver.Basic;
using RotationSolver.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotationSolver.UI;

internal partial class RotationConfigWindow
{
    private void DrawControlTab()
    {
        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Control_ShowControlWindow,
            ref Service.Config.ShowControlWindow, otherThing: RotationSolverPlugin.OpenControlWindow);

        if (!Service.Config.ShowControlWindow) return;

        DrawCheckBox(LocalizationManager.RightLang.ConfigWindow_Control_IsControlWindowLock,
            ref Service.Config.IsControlWindowLock);
    }
}
