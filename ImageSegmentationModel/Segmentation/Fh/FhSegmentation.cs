using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

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

        private List<Edge> BuildEdges(int width, int height, RGB[,] pixels, Node[,] nodes, ConnectingMethod connectingMethod, ColorDifference difType)
        {
            List<Edge> edges = new List<Edge>();
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                {
                    if (x < (width - 1))
                    {
                        if (y > 0 && connectingMethod == ConnectingMethod.Connecred_8)
                            edges.Add(new Edge(nodes[x, y], nodes[x + 1, y - 1], ImageHelper.Difference(pixels, x, y, x + 1, y - 1, difType))); // Up-Right
                        edges.Add(new Edge(nodes[x, y], nodes[x + 1, y], ImageHelper.Difference(pixels, x, y, x + 1, y, difType))); // Right
                        if (y < (height - 1) && connectingMethod == ConnectingMethod.Connecred_8)
                            edges.Add(new Edge(nodes[x, y], nodes[x + 1, y + 1], ImageHelper.Difference(pixels, x, y, x + 1, y + 1, difType))); // Down-Right
                    }
                    if (y < (height - 1))
                        edges.Add(new Edge(nodes[x, y], nodes[x, y + 1], ImageHelper.Difference(pixels, x, y, x, y + 1, difType))); // Down
                }
            return edges;
        }

        private Graph GetGraph(int width, int height, RGB[,] pixels, int k, ConnectingMethod connectingMethod, ColorDifference difType)
        {
            List<Segment> segments = GetSegments(width * height, k);
            Node[,] nodes = GetNode(width, height, segments);
            List<Edge> edges = BuildEdges(width, height, pixels, nodes, connectingMethod, difType);

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

        public int[,] BuildSegments(int width, int height, RGB[,] pixels, int k, int minSize, ConnectingMethod connectingMethod, ColorDifference difType)
        {
            var watch = Stopwatch.StartNew();
            
            Graph graph = GetGraph(width, height, pixels, k, connectingMethod, difType);
            watch.Stop();
            MessageBox.Show("" + watch.ElapsedMilliseconds, "Create+fill", MessageBoxButtons.OK);
            watch = Stopwatch.StartNew();
            graph.Edges.Sort();
            watch.Stop();
            MessageBox.Show("" + watch.ElapsedMilliseconds, "Sort", MessageBoxButtons.OK);
            watch = Stopwatch.StartNew();
            foreach (Edge edge in graph.Edges)
            {
                try
                {
                    Segment a = edge.A.Segment;
                    Segment b = edge.B.Segment;
                    if (a.Id != b.Id)
                    {
                        if (edge.Weight <= MInt(a, b))
                            if (a.Nodes.Count >= b.Nodes.Count)
                                MergeSegment(a, b, edge.Weight);
                            else MergeSegment(b, a, edge.Weight);
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
            watch.Stop();
            MessageBox.Show("" + watch.ElapsedMilliseconds, "Marge", MessageBoxButtons.OK);
            watch = Stopwatch.StartNew();
            foreach (Edge edge in graph.Edges)
            {
                try
                {
                    Segment a = edge.A.Segment;
                    Segment b = edge.B.Segment;
                    if (a.Id != b.Id)
                    {
                        if (a.Nodes.Count < minSize || b.Nodes.Count < minSize)
                            if (a.Nodes.Count >= b.Nodes.Count)
                                MergeSegment(a, b, edge.Weight);
                            else MergeSegment(b, a, edge.Weight);
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
            watch.Stop();
            MessageBox.Show("" + watch.ElapsedMilliseconds, "minSize", MessageBoxButtons.OK);
            int[,] segments = new int[width, height];
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    segments[x, y] = graph.Nodes[x, y].Segment.Id;
            return segments;
        }

        #endregion
    }
}
