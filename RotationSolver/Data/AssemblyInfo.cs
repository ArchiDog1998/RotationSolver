using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotationSolver.Data
{
    internal record AssemblyInfo(
        string Name,
        string Author, 
        string FilePath, 
        string SupportLink, 
        string HelpLink, 
        string ChangeLog, 
        string DonateLink);
}
