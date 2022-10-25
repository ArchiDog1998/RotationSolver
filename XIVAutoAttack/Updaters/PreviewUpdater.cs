using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIVAutoAttack.Updaters
{
    internal static class PreviewUpdater
    {
        static DtrBarEntry dtrEntry;

        internal static void UpdateEntry()
        {
            if (Service.Configuration.UseDtr && IconReplacer.StateString != null)
            {
                if (dtrEntry == null)
                {
                    dtrEntry = Service.DtrBar.Get("Auto Attack");
                }
                dtrEntry.Shown = true;
                dtrEntry.Text = new SeString(
                    new IconPayload(BitmapFontIcon.DPS),
                    new TextPayload(IconReplacer.StateString)
                    );
            }
            else if (dtrEntry != null)
            {
                dtrEntry.Shown = false;
            }
        }

        internal static unsafe void UpdateCastBar()
        {
            ByteColor redColor = new ByteColor() { A = 255, R = 120, G = 0, B = 60 };
            ByteColor greenColor = new ByteColor() { A = 255, R = 60, G = 120, B = 30 };


            var castBar = Service.GameGui.GetAddonByName("_CastBar", 1);
            if (castBar == IntPtr.Zero) return;
            AtkResNode* progressBar = ((AtkUnitBase*)castBar)->UldManager.NodeList[5];

            bool canMove = !Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.OccupiedInEvent]
                && Service.Configuration.CheckForCasting && !Service.Conditions[Dalamud.Game.ClientState.Conditions.ConditionFlag.Casting];

            ByteColor c = canMove ? greenColor : redColor;
            XIVAutoAttackPlugin.movingController.IsMoving = canMove;

            progressBar->AddRed = c.R;
            progressBar->AddGreen = c.G;
            progressBar->AddBlue = c.B;
        }

        public static void Dispose()
        {
            dtrEntry?.Dispose();
        }
    }
}
