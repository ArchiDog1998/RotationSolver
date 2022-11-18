using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace XIVAutoAttack.Combos.Script.Actions
{
    internal class ComboSet : IDraw
    {
        public string Description { get; set; }
        public ActionsSet GeneralGCDSet { get; set; } = new ActionsSet()
        {
            Name = "通用GCD",
            Description = "最常规的GCD技能放这里。",
        };

        public void Draw(IScriptCombo combo)
        {
            var desc = Description;
            if (ImGui.InputTextMultiline("描述:", ref desc, 1024, new Vector2(250, 250)))
            {
                Description = desc;
            }

            GeneralGCDSet.DrawHeader();
        }
    }
}
