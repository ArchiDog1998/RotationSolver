using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.SubKinds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIVComboPlus
{
    public enum Role : byte 
    {
        None = 0,
        防护 = 1,
        近战 = 2,
        远程 = 3,
        治疗 = 4,
    }
}
