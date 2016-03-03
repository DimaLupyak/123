using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageSegmentationModel.Segmentation.Classic
{
    class Segment
    {
        #region constructors

        public Segment(int id)
        {
            Id = id;
            Nodes = new List<Node>();
        }

        #endregion

        #region public members

        public int Id { get; private set; }
        public List<Node> Nodes { get; private set; }

        public double MaxWeight { get; private set; }
        public double SegmentWeight { get; private set; }

        public void Clear()
        {
            Nodes.Clear();
            MaxWeight = 0;
            SegmentWeight = 0;
        }

        public void AddNode(Node node)
        {
            Clear();
            Nodes.Add(node);
        }

        public void AddNodes(List<Node> nodes, double edgeWeight, double bNodesCredit)
        {
            foreach (Node node in nodes)
                node.Segment = this;
            Nodes.AddRange(nodes);
            MaxWeight = edgeWeight;
            SegmentWeight += edgeWeight + bNodesCredit;
        }

        #endregion
    }
}
