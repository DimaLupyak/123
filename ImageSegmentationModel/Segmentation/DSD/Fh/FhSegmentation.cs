using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ImageSegmentationModel.Segmentation.DSD.Fh
{
    class FhSegmentation : AFhSegmentationDSD
    {
        protected override bool MergePredicate(Node a, Node b, double weight, int k)
        {            
            return weight <= MInt(a,b,k);
        }
        protected double T(Node s, int k)
        {
            return (double)k / s.Size;
        }

        protected double MInt(Node a, Node b, int k)
        {
            return Math.Min(a.MaxWeight + T(a, k), b.MaxWeight + T(b, k));
        }
    }
}
