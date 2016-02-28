using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageSegmentationModel.Segmentation.Fh
{
    class Segment
    {
        #region constructors

        public Segment(int id, int k)
        {
            Id = id;
            _k = k;
            Nodes = new List<Node>();
        }

        #endregion

        #region private members

        private readonly int _k;

        #endregion

        #region public members

        public int Id { get; private set; }
        public List<Node> Nodes { get; private set; }

        private double _int;
        private double _threshold;
        public double Int
        {
            get { return _int + _threshold; }
        }

        public void Clear()
        {
            Nodes.Clear();
            _int = 0;
            _threshold = _k;
        }

        public void AddNode(Node node)
        {
            Clear();
            Nodes.Add(node);
        }

        public void AddNodes(List<Node> nodes, int edgeWeight)
        {
            foreach (Node node in nodes)
                node.Segment = this;
            Nodes.AddRange(nodes);
             _int = edgeWeight;
            _threshold = _k / Nodes.Count;
        }

        #endregion
    }
}
