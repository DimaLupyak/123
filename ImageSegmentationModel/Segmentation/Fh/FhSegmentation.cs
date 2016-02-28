using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageSegmentationModel.Segmentation.Fh
{
    class FhSegmentation : IFhSegmentation
    {
        #region generate graph private members

        private List<Segment> GetSegments(int count, int k)
        {
            List<Segment> segments = new List<Segment>(count);
            for (int id = 0; id < count; id++)
                segments.Add(new Segment(id, k));
            return segments;
        }

        private Node[,] GetNode(int width, int height, List<Segment> segments)
        {
            Node[,] nodes = new Node[width, height];
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                {
                    nodes[x, y] = new Node(x, y, segments[y * width + x]);
                    segments[y * width + x].AddNode(nodes[x, y]);
                }
            return nodes;
        }

        private List<Edge> BuildEdges(int width, int height, byte[,] pixels, Node[,] nodes, ConnectingMethod connectingMethod)
        {
            List<Edge> edges = new List<Edge>();
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                {
                    byte pixel1 = pixels[x, y];
                    if (x < (width - 1))
                    {
                        if (y > 0 && connectingMethod == ConnectingMethod.Connecred_8)
                            edges.Add(new Edge(nodes[x, y], nodes[x + 1, y - 1], Math.Abs(pixel1 - pixels[x + 1, y - 1]))); // Up-Right
                        edges.Add(new Edge(nodes[x, y], nodes[x + 1, y], Math.Abs(pixel1 - pixels[x + 1, y]))); // Right
                        if (y < (height - 1) && connectingMethod == ConnectingMethod.Connecred_8)
                            edges.Add(new Edge(nodes[x, y], nodes[x + 1, y + 1], Math.Abs(pixel1 - pixels[x + 1, y + 1]))); // Down-Right
                    }
                    if (y < (height - 1))
                        edges.Add(new Edge(nodes[x, y], nodes[x, y + 1], Math.Abs(pixel1 - pixels[x, y + 1]))); // Down
                }
            return edges;
        }

        private Graph GetGraph(int width, int height, byte[,] pixels, int k, ConnectingMethod connectingMethod)
        {
            List<Segment> segments = GetSegments(width * height, k);
            Node[,] nodes = GetNode(width, height, segments);
            List<Edge> edges = BuildEdges(width, height, pixels, nodes, connectingMethod);

            return new Graph(nodes, edges, segments);
        }

        #endregion

        #region algorithm private members

        private double MInt(Segment a, Segment b)
        {
            return Math.Min(a.Int, b.Int);
        }

        private void MergeSegment(Segment a, Segment b, int weight)
        {
            a.AddNodes(b.Nodes, weight);
            b.Clear();
        }

        #endregion

        #region IFhSegmentation members

        public int[,] BuildSegments(int width, int height, byte[,] pixels, int k, int minSize, ConnectingMethod connectingMethod)
        {
            Graph graph = GetGraph(width, height, pixels, k, connectingMethod);

            graph.Edges.Sort();
            foreach (Edge edge in graph.Edges)
            {
                try
                {
                    Segment a = edge.A.Segment;
                    Segment b = edge.B.Segment;
                    if (a.Id != b.Id)
                    {
                        if (edge.Weight < MInt(a, b))
                            MergeSegment(a, b, edge.Weight);
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
            foreach (Edge edge in graph.Edges)
            {
                try
                {
                    Segment a = edge.A.Segment;
                    Segment b = edge.B.Segment;
                    if (a.Id != b.Id)
                    {
                        if (a.Nodes.Count < minSize || b.Nodes.Count < minSize)
                            MergeSegment(a, b, edge.Weight);
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
            int[,] segments = new int[width, height];
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    segments[x, y] = graph.Nodes[x, y].Segment.Id;
            return segments;
        }

        #endregion
    }
}
