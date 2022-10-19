using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIVAutoAttack.Actions
{
    public abstract class BaseAction : IAction
    {
        public abstract uint ID { get; }
        public abstract bool Use();

        public sealed override bool Equals(object obj)
        {
            if (obj is not IAction act) return false;
            return this.ID == act.ID;
        }

        public sealed override int GetHashCode()
        {
            return (int)ID;
        }

        public static bool operator ==(BaseAction a, BaseAction b)
        {
            if(a == null || b == null) return false;
            return a.Equals(b);
        }

        public static bool operator !=(BaseAction a, BaseAction b)
        {
            return !(a == b);
        }
    }
}
