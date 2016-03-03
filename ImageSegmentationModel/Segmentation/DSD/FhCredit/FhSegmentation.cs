using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ImageSegmentationModel.Segmentation.DSD.FhCredit
{
    class FhSegmentation : AFhSegmentationDSD
    {
        protected override bool MergePredicate(Node a, Node b, double weight, int credit)
        {
            return weight + a.SegmentWeight + b.SegmentWeight <= credit;
        }
    }
}

