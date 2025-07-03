using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.Enums
{
    public enum fuel_policy
    {
        FullToFull, // Full tank at pickup, full tank at return
        FullToEmpty, // Full tank at pickup, empty tank at return
        EmptyToFull, // Empty tank at pickup, full tank at return
        EmptyToEmpty // Empty tank at pickup, empty tank at return
    }
}
