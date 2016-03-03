using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSegmentationModel.Segmentation.Classic
{
    abstract class AFhSegmentationClassic : AFhSegmentation
    {
        protected Node[,] nodes;
        protected List<Edge> edges;
        protected List<Segment> segments;

        protected override void BuildGraph(int width, int height, RGB[,] pixels, ConnectingMethod connectingMethod, ColorDifference difType)
        {
            segments = new List<Segment>(width * height);
            for (int id = 0; id < width * height; id++)
                segments.Add(new Segment(id));


            nodes = new Node[width, height];
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                {
                    nodes[x, y] = new Node(x, y, segments[y * width + x]);
                    segments[y * width + x].AddNode(nodes[x, y]);
                }

            edges = new List<Edge>();
            for (int j = 0; j < height; j++)
                for (int i = 0; i < width; i++)
                {
                    if (i < (width - 1))
                        edges.Add(new Edge(nodes[i, j], nodes[i + 1, j], PixelComparator.Difference(pixels[i, j], pixels[i + 1, j], difType))); // Right                        

                    if (j < (height - 1))
                        edges.Add(new Edge(nodes[i, j], nodes[i, j + 1], PixelComparator.Difference(pixels[i, j], pixels[i, j + 1], difType))); // Down

                    if (connectingMethod == ConnectingMethod.Connecred_8)
                    {
                        if (j > 0 && i < (width - 1))
                            edges.Add(new Edge(nodes[i, j], nodes[i + 1, j - 1], PixelComparator.Difference(pixels[i, j], pixels[i + 1, j - 1], difType))); // Up-Right

                        if (j < (height - 1) && i < (width - 1))
                            edges.Add(new Edge(nodes[i, j], nodes[i + 1, j + 1], PixelComparator.Difference(pixels[i, j], pixels[i + 1, j + 1], difType))); // Down-Right
                    }
                }
        }

        protected override void SortEdges()
        {
            edges.Sort();
        }

        protected override void DoAlgorithm(int param)
        {
            foreach (Edge edge in edges)
            {
                Segment a = edge.A.Segment;
                Segment b = edge.B.Segment;
                if (a.Id != b.Id)
                {
                    if (MergePredicate(a, b, edge.Weight, param))
                        if (a.Nodes.Count >= b.Nodes.Count)
                            MergeSegment(a, b, edge.Weight, b.SegmentWeight);
                        else MergeSegment(b, a, edge.Weight, a.SegmentWeight);
                }
            }

        }

        protected override void MargeSmall(int minSize)
        {
            foreach (Edge edge in edges)
            {
                Segment a = edge.A.Segment;
                Segment b = edge.B.Segment;
                if (a.Id != b.Id)
                {
                    if (a.Nodes.Count < minSize || b.Nodes.Count < minSize)
                        if (a.Nodes.Count >= b.Nodes.Count)
                            MergeSegment(a, b, edge.Weight, a.SegmentWeight);
                        else MergeSegment(b, a, edge.Weight, b.SegmentWeight);
                }

            }
        }

        protected override int[,] ReindexSegments(int width, int height)
        {
            int[,] segments = new int[width, height];
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    segments[x, y] = nodes[x, y].Segment.Id;
            return segments;
        }

        protected void MergeSegment(Segment a, Segment b, double weight, double bNodesCredit)
        {
            a.AddNodes(b.Nodes, weight, bNodesCredit);
            b.Clear();
        }

        protected abstract bool MergePredicate(Segment a, Segment b, double weight, int param);
    }
}
