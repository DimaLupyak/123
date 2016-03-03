using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSegmentationModel
{
    public enum SegmentationMethod
    {
        OriginalFh, OriginalCreditFh, DSDFh, DSDCreditFh, NoSortFh, NoSortCreditFh, NoSortDSDFh, NoSortCreditDSDFh
    }
    public enum ConnectingMethod
    {
        Connecred_4, Connecred_8
    }

    public enum ColorDifference
    {
        RGB_std_deviation, CIE76, CIE94, CIE2000, dRed, dBlue, dGreen, Grey
    }
}
