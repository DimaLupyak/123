using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageSegmentationModel.Segmentation.NoSortDSD
{
    class Edge 
    {
        #region constructors
        public Edge():this(null,null) {}

        public Edge(Node a, Node b)
        {
            A = a;
            B = b;
        }

        #endregion
        #region public members

        public Node A { get; set; }
        public Node B { get; set; }

        public Edge Next { get; set; }

        #endregion
    }
}
