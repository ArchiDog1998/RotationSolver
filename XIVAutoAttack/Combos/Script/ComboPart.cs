using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XIVAutoAttack.Actions.BaseAction;

namespace XIVAutoAttack.Combos.Script
{
    internal class ComboPart
    {
        ScriptAction[] Actions { get; set; }

        public bool Part(out BaseAction act)
        {
            foreach (var item in Actions)
            {
                if(item.ShouldUse(out act))
                {
                    //It's a guard.
                    if (item.Action == null) return false;

                    //Just use it.
                    return true;
                }
            }

            act = null;
            return false;
        }
    }
}
