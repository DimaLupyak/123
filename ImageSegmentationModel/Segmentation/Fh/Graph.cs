using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageSegmentationModel.Segmentation.Fh
{
    class Graph
    {
        #region constructors

        public Graph(Node[,] nodes, List<Edge> edges, List<Segment> segments)
        {
            Nodes = nodes;
            Edges = edges;
            Segments = segments;
        }

        #endregion

        #region public members

        public Node[,] Nodes { get; private set; }
        public List<Edge> Edges { get; private set; }
        public List<Segment> Segments { get; private set; }

        #endregion
    }
}
