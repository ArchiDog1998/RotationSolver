using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Data;

namespace XIVAutoAttack.Combos.Script.Actions
{
    internal class ActionsSet
    {
        public ActionConditions[] ActionsCondition { get; set; }

        public bool ShouldUse(IScriptCombo owner, out IAction act)
        {
            act = null;

            foreach (var condition in ActionsCondition)
            {
                var result = condition.ShouldUse(owner, out act);

                if (result.HasValue && result.Value) return true;
                else if(!result.HasValue) return false;
            }

            return false;
        }
    }
}
