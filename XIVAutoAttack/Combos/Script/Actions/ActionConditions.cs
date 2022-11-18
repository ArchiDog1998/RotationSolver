using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Actions.BaseAction;
using XIVAutoAttack.Combos.Script.Conditions;
using XIVAutoAttack.Data;

namespace XIVAutoAttack.Combos.Script.Actions
{
    internal class ActionConditions
    {
        BaseAction _action;

        public ActionID ID { get; set; }
        public ConditionSet Set { get; set; }

        public bool MustUse { get; set; }
        public bool Empty { get; set; }

        public bool? ShouldUse(IScriptCombo owner, out IAction act)
        {
            if(ID != ActionID.None && _action == null)
            {
                _action = owner.AllActions.FirstOrDefault(a => (ActionID)a.ID == ID);
            }

            act = _action;

            if (_action != null)
            {
                return _action.ShouldUse(out act, MustUse, Empty) && Set.IsTrue;
            }
            else
            {
                return Set.IsTrue ? null : false;
            }
        }
    }
}
