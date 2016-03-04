using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSegmentationModel
{    
    public enum DataStructure
    {
        SimpleGhaph, DisjointSetDataGhaph
    }
    public enum SortModification
    {
        NoSorting, WithSorting
    }

    public enum MargeHeuristic
    {
        K, Credit
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
