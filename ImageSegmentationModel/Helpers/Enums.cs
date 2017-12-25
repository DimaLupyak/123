using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSegmentationModel
{

    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum SortModification
    {
        [Description("Без сортування")]
        NoSorting,
        [Description("З сортуванням")]
        WithSorting
    }
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum MargeHeuristic
    {
        [Description("Параметр k")]
        K,
        [Description("Параметр credit")]
        Credit
    }
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum ConnectingMethod
    {
        [Description("4-connected")]
        Connected_4,
        [Description("8-connected")]
        Connected_8
    }
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum ColorDifference
    {
        [Description("RGB середнє квадратичне")]
        RGB_std_deviation,
        [Description("CIE76")]
        CIE76,
        [Description("CIE94")]
        CIE94,
        [Description("CIE2000")]
        CIE2000,
        [Description("Різниця складових Red")]
        dRed,
        [Description("Різниця складових Blue")]
        dBlue,
        [Description("Різниця складових Green")]
        dGreen,
        [Description("Різниця відтінків сірого")]
        Grey
    }
}
