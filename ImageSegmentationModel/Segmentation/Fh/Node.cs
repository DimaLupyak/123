using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageSegmentationModel.Segmentation.Fh
{
    class Node
    {
        #region constructors

        public Node(int x, int y, Segment segment)
        {
            X = x;
            Y = y;
            Segment = segment;
        }

        #endregion

        #region public members

        public int X { get; private set; }
        public int Y { get; private set; }

        public Segment Segment { get; set; }

        #endregion
    }
}
