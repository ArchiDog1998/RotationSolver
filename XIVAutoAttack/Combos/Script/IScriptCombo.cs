using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XIVAutoAttack.Combos.CustomCombo;

namespace XIVAutoAttack.Combos.Script
{
    internal interface IScriptCombo : ICustomCombo
    {
        string AuthorName { get; set; }

        string FilePath { get; set; }

        public ComboPart GeneralGCDPart { get; set; }
    }
}
