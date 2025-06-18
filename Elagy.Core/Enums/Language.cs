using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.Enums
{
    using System.ComponentModel; // Required for [Description] attribute

    public enum Language
    {
        /// <summary>
        /// Represents an unknown or unspecified language.
        /// </summary>
        [Description("Unknown")] 
        Unknown = 0,

        // Top 10-15 most spoken languages globally (by total speakers)
        [Description("English")]
        En = 1,

        [Description("Mandarin Chinese")]
        Zh = 2, // ISO 639-1 for Chinese. Often used for Mandarin, but can encompass other Chinese languages depending on context.

        [Description("Hindi")]
        Hi = 3,

        [Description("Spanish")]
        Es = 4,

        [Description("French")]
        Fr = 5,

        [Description("Arabic")]
        Ar = 6, // General Arabic. Specific dialects (e.g., Egyptian Arabic) might need more granular representation if required.

        [Description("Bengali")]
        Bn = 7,

        [Description("Portuguese")]
        Pt = 8,

        [Description("Russian")]
        Ru = 9,

        [Description("Indonesian")]
        Id = 10, // Also Bahasa Indonesia

        [Description("Urdu")]
        Ur = 11,

        [Description("German")]
        De = 12,

        [Description("Japanese")]
        Ja = 13,

        [Description("Vietnamese")]
        Vi = 14,

        [Description("Turkish")]
        Tr = 15,

        // Additional common languages you might want to include
        [Description("Italian")]
        It = 16,

        [Description("Korean")]
        Ko = 17,

        [Description("Polish")]
        Pl = 18,

        [Description("Dutch")]
        Nl = 19,

        [Description("Swedish")]
        Sv = 20,

        [Description("Greek")]
        El = 21,

        [Description("Thai")]
        Th = 22,

        [Description("Hebrew")]
        He = 23,

        [Description("Persian")]
        Fa = 24, // Farsi

        [Description("Romanian")]
        Ro = 25,

        [Description("Ukrainian")]
        Uk = 26,

        [Description("Czech")]
        Cs = 27,

        [Description("Hungarian")]
        Hu = 28,

        [Description("Finnish")]
        Fi = 29,

        [Description("Norwegian")]
        No = 30, // Often covers nb (Bokmål) and nn (Nynorsk)

        [Description("Danish")]
        Da = 31,

        [Description("Swahili")]
        Sw = 32,

        [Description("Malay")]
        Ms = 33,

        [Description("Zulu")]
        Zu = 34,

        [Description("Amharic")]
        Am = 35,

        [Description("Hausa")]
        Ha = 36,

        [Description("Yoruba")]
        Yo = 37,

        [Description("Tagalog")]
        Tl = 38,

        [Description("Nepali")]
        Ne = 39,

        [Description("Kannada")]
        Kn = 40,

        [Description("Telugu")]
        Te = 41,

        [Description("Tamil")]
        Ta = 42,

        [Description("Gujarati")]
        Gu = 43,

        [Description("Marathi")]
        Mr = 44,

        [Description("Punjabi")]
        Pa = 45, // General Punjabi

        [Description("Sinhala")]
        Si = 46,

        [Description("Somali")]
        So = 47,

        [Description("Kurdish")]
        Ku = 48,

        [Description("Albanian")]
        Sq = 49
    }
}
