
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageSegmentationModel.Segmentation.NoSort
{
    class Segment
    {
        public int Id, Count;
        public double SegmentWeight;
        public double MaxWeight;
        public Node First, Last;
    }
}
