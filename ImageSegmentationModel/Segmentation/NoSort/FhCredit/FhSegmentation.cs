using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ImageSegmentationModel.Segmentation.NoSort.FhCredit
{
    class FhSegmentation : AFhSegmentationNoSort
    {
        protected override bool MergePredicate(Segment a, Segment b, double weight, int credit)
        {
            if (a == b)
                return false; 
            return weight + a.SegmentWeight + b.SegmentWeight <= credit;
        }
    }
}

