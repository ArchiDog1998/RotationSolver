using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Combos.Basic;
using XIVAutoAttack.Combos.Script.Actions;

namespace XIVAutoAttack.Combos.Script.Combos
{
    internal class MCHCombo_Script : MCHCombo_Base<Enum>, IScriptCombo
    {
        public override string Author => Set.GetAuthor();

        public string AuthorName { get; set; }
        public ComboSet Set { get ; set ; }

        private protected override bool AttackAbility(byte abilityRemain, out IAction act)
        {
            act = null;
            return false;
        }

        private protected override bool GeneralGCD(out IAction act) => Set.GeneralGCDSet.ShouldUse(this, out act);
    }
}
