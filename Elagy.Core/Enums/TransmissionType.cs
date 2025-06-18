using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.Enums
{
    public enum TransmissionType
    {
        /// <summary>
        /// Represents an unknown or unspecified transmission type.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Represents an automatic transmission.
        /// </summary>
        Automatic = 1,

        /// <summary>
        /// Represents a manual transmission.
        /// </summary>
        Manual = 2,

        /// <summary>
        /// Represents a transmission system that offers both automatic and manual modes (e.g., Tiptronic, automated manual transmission).
        /// </summary>
        Both = 3  
    }
}
