using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIVAutoAttack.Combos.Script.Conditions
{
    internal interface ICondition : IDraw
    {
        bool IsTrue { get; }


    }
}
