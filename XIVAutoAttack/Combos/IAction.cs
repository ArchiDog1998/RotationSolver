using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIVAutoAttack.Combos
{
    public interface IAction
    {
        bool Use();
        uint ID { get; }
    }
}
