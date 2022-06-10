using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIVAutoAttack.Configuration
{
    public class ActionEvents
    {
        public string Name { get; set; }
        public int MacroIndex { get; set; }
        public bool IsShared { get; set; }

        public ActionEvents()
        {
            Name = "";
            MacroIndex = -1;
            IsShared = false;
        }
    }
}
