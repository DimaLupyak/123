using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSegmentationModel
{
    public enum SegmentationMethod
    {
        FhOriginal, FhWithoutSort, FhCreditWithoutSort, FhDSD
    }
    public enum ConnectingMethod
    {
        Connecred_4, Connecred_8, Connecred_16
    }

    public enum ColorDifference
    {
        RGB_std_deviation, CIE76, dRed, dBlue, dGreen, Grey
    }
}
