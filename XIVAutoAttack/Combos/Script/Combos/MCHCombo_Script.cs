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
        public override string Author => this.GetAuthor();

        public string AuthorName { get; set; }
        public ActionsSet GeneralGCDSet { get; set; }

        private protected override bool AttackAbility(byte abilityRemain, out IAction act)
        {
            new JsonSerializerSettings()
            {
                MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            };
            act = null;
            return false;
        }

        private protected override bool GeneralGCD(out IAction act) => GeneralGCDSet.ShouldUse(this, out act);
    }
}
