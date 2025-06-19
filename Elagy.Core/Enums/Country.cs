using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.Enums
{
    public enum Country
    {
        /// <summary>
        /// Represents an unknown or unspecified Arab country.
        /// </summary>
        [Description("Unknown")]
        Unknown = 0,

        /// <summary>
        /// Algeria (Al-Jazāʾir)
        /// </summary>
        [Description("Algeria")]
        DZ = 1,

        /// <summary>
        /// Bahrain (Al-Baḥrayn)
        /// </summary>
        [Description("Bahrain")]
        BH = 2,

        /// <summary>
        /// Comoros (Juzur al-Qamar)
        /// </summary>
        [Description("Comoros")]
        KM = 3,

        /// <summary>
        /// Djibouti (Jībūtī)
        /// </summary>
        [Description("Djibouti")]
        DJ = 4,

        /// <summary>
        /// Egypt (Miṣr)
        /// </summary>
        [Description("Egypt")]
        EG = 5,

        /// <summary>
        /// Iraq (Al-ʿIrāq)
        /// </summary>
        [Description("Iraq")]
        IQ = 6,

        /// <summary>
        /// Jordan (Al-ʾUrdun)
        /// </summary>
        [Description("Jordan")]
        JO = 7,

        /// <summary>
        /// Kuwait (Al-Kuwayt)
        /// </summary>
        [Description("Kuwait")]
        KW = 8,

        /// <summary>
        /// Lebanon (Lubnān)
        /// </summary>
        [Description("Lebanon")]
        LB = 9,

        /// <summary>
        /// Libya (Lībiyā)
        /// </summary>
        [Description("Libya")]
        LY = 10,

        /// <summary>
        /// Mauritania (Mūrītānyā)
        /// </summary>
        [Description("Mauritania")]
        MR = 11,

        /// <summary>
        /// Morocco (Al-Maġrib)
        /// </summary>
        [Description("Morocco")]
        MA = 12,

        /// <summary>
        /// Oman (ʿUmān)
        /// </summary>
        [Description("Oman")]
        OM = 13,

        /// <summary>
        /// Palestine (Filasṭīn) - State of Palestine
        /// </summary>
        [Description("Palestine")]
        PS = 14,

        /// <summary>
        /// Qatar (Qaṭar)
        /// </summary>
        [Description("Qatar")]
        QA = 15,

        /// <summary>
        /// Saudi Arabia (Al-ʿArabiyyah as-Suʿūdiyyah)
        /// </summary>
        [Description("Saudi Arabia")]
        SA = 16,

        /// <summary>
        /// Somalia (Aṣ-Ṣūmāl)
        /// </summary>
        [Description("Somalia")]
        SO = 17,

        /// <summary>
        /// Sudan (As-Sūdān)
        /// </summary>
        [Description("Sudan")]
        SD = 18,

        /// <summary>
        /// Syria (Sūriyā) - Syrian Arab Republic
        /// </summary>
        [Description("Syria")]
        SY = 19,

        /// <summary>
        /// Tunisia (Tūnis)
        /// </summary>
        [Description("Tunisia")]
        TN = 20,

        /// <summary>
        /// United Arab Emirates (Al-ʾImārāt al-ʿArabiyyah al-Muttaḥidah)
        /// </summary>
        [Description("United Arab Emirates")]
        AE = 21,

        /// <summary>
        /// Yemen (Al-Yaman)
        /// </summary>
        [Description("Yemen")]
        YE = 22
    }
}
