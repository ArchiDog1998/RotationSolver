using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIVAutoAttack.Combos.Attributes
{
    public class ComboDevInfoAttribute : Attribute
    {
        public string URL { get; }
        public ComboDevInfoAttribute(string uRL)
        {
            URL = uRL;
        }
    }
}
