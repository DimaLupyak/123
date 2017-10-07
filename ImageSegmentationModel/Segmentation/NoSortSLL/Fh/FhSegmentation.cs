using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ImageSegmentationModel.Segmentation.NoSortSLL.Fh
{
    class FhSegmentation : AFhSegmentationNoSortSLL
    {
        protected override bool MergePredicate(Segment a, Segment b, double weight, int k)
        {
            if (a == b)
                return false;
            return weight < MInt(a,b,k);
        }
        protected double T(Segment s, int k)
        {
            return (double)k / s.Count;
        }

        protected double MInt(Segment a, Segment b, int k)
        {
            return Math.Min(a.MaxWeight + T(a, k), b.MaxWeight + T(b, k));
        }
    }
}
