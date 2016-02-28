using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageSegmentationModel.Segmentation.Fh
{
    class Edge : IComparable
    {
        #region constructors

        public Edge(Node a, Node b, int weight)
        {
            A = a;
            B = b;
            Weight = weight;
        }

        #endregion

        #region public members

        public Node A { get; private set; }
        public Node B { get; private set; }

        public int Weight { get; private set; }

        #endregion

        #region IComparable memebrs

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            Edge edge = obj as Edge;
            if (edge != null)
                return this.Weight.CompareTo(edge.Weight);
            else
                throw new ArgumentException("Object is not a Edge");
        }

        #endregion
    }
}
