using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.Enums
{
    public enum FuelType
    {
        /// <summary>
        /// Represents an unknown or unspecified fuel type.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Represents Gasoline/Petrol fuel.
        /// </summary>
        Petrol = 1, // Also commonly known as Gasoline

        /// <summary>
        /// Represents Diesel fuel.
        /// </summary>
        Diesel = 2,

        /// <summary>
        /// Represents Electric power.
        /// </summary>
        Electric = 3,

        /// <summary>
        /// Represents Liquefied Petroleum Gas (LPG).
        /// </summary>
        LPG = 4,

        /// <summary>
        /// Represents Compressed Natural Gas (CNG).
        /// </summary>
        CNG = 5,

        /// <summary>
        /// Represents a hybrid vehicle (e.g., Petrol-Electric, Diesel-Electric).
        /// </summary>
        Hybrid = 6,

        /// <summary>
        /// Represents Ethanol (e.g., E85).
        /// </summary>
        Ethanol = 7,

        /// <summary>
        /// Represents Hydrogen fuel cell.
        /// </summary>
        Hydrogen = 8
    }
}
