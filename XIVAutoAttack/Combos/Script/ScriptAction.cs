using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Data;

namespace XIVAutoAttack.Combos.Script
{
    internal class ScriptAction
    {
        public BaseAction Action { get; set; }
        public bool MustUse { get; set; }
        public bool Empty { get; set; }

        public ActionCheck[] ActionChecks { get; set; }

        public bool ShouldUse(out BaseAction act)
        {
            act = Action;
            if (!CheckAction()) return false;
            return Action.ShouldUse(out _, MustUse, Empty);
        }

        private bool CheckAction()
        {
            foreach (var item in ActionChecks)
            {
                var and = item.IsAnd;
                var right = item.IsRight();

                if (and && !right) return false;
                if (!and && right) return true;
            }

            return ActionChecks[ActionChecks.Length - 1].IsRight();
        }
    }
}
