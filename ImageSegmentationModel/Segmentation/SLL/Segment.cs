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
        public long SumR;
        public long SumG;
        public long SumB;

        public RGB RGB 
        { 
            get 
            { 
                return new RGB((byte)(SumR / Count), 
                               (byte)(SumG / Count), 
                               (byte)(SumB / Count)); 
            } 
        }
    }
}
