using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Combos.Basic;

namespace XIVAutoAttack.Combos.Script.Combos
{
    internal class MCHCombo_Script : MCHCombo_Base<Enum>, IScriptCombo
    {
        public override string Author => AuthorName + "-Script";

        public string AuthorName { get; set; }
        public string FilePath { get; set; }
        public ComboPart GeneralGCDPart { get; set; }

        private protected override bool AttackAbility(byte abilityRemain, out IAction act)
        {
            new JsonSerializerSettings()
            {
                MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            };
            act = null;
            return false;
        }

        private protected override bool GeneralGCD(out IAction act)
        {
            if(GeneralGCDPart.Part(out var action))
            {
                act = action;
                return true;
            }

            act = null;
            return false;
        }
    }
}
