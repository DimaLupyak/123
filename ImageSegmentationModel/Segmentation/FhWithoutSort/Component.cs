﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageSegmentationModel.Segmentation.FhWithoutSort
{
    class Component
    {
        public int Index, Count;
        public double MaxWeight;
        public Node First, Last;
    }
}