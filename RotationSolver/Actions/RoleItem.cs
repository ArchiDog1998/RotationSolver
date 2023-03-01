using RotationSolver.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotationSolver.Actions
{
    internal class RoleItem : BaseItem
    {
        private JobRole[] _roles;
        public RoleItem(uint row, JobRole[] roles, uint a4 = 65535) : base(row, a4)
        {
            _roles = roles;
        }

        internal bool InRole(JobRole role) => _roles.Contains(role);

    }
}
