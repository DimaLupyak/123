using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageSegmentationModel.Segmentation.SLL
{
    class Segment
    {
        public int Count, Id;
        public double SegmentWeight;
        public double MaxWeight;
        public Node First, Last;

    }
}
