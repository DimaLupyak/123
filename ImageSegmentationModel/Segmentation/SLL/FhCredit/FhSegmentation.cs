using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ImageSegmentationModel.Segmentation.SLL.FhCredit
{
    class FhSegmentation : AFhSegmentationSLL
    {
        protected override bool MergePredicate(Segment a, Segment b, double weight, int credit)
        {            
            return weight + a.SegmentWeight + b.SegmentWeight <= credit;
        }
    }
}

