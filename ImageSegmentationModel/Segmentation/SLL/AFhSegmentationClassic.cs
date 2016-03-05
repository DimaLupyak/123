using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSegmentationModel.Segmentation.SLL
{
    abstract class AFhSegmentationSLL : AFhSegmentation
    {
        protected Node[,] nodes;
        protected List<Edge> edges;
        protected Segment[] segments;

        protected override void BuildGraph(int width, int height, RGB[,] pixels, ConnectingMethod connectingMethod, ColorDifference difType)
        {
            segments = new Segment[width * height];
            for (int id = 0; id < width * height; id++)
                segments[id] = new Segment();

            nodes = new Node[width, height];
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    nodes[x, y] = new Node();

            edges = new List<Edge>();
            for (int j = 0; j < height; j++)
                for (int i = 0; i < width; i++)
                {
                    int c = j * width + i;
                    // initialize node
                    nodes[i, j].Segment = segments[c];
                    // initialize component
                    segments[c].Id = -1;
                    segments[c].Count = 1;
                    segments[c].SegmentWeight = 0;
                    segments[c].First = segments[c].Last = nodes[i, j];

                    if (i < (width - 1))
                        edges.Add(new Edge(nodes[i, j], nodes[i + 1, j], PixelComparator.Difference(pixels[i, j], pixels[i + 1, j], difType))); // Right                        

                    if (j < (height - 1))
                        edges.Add(new Edge(nodes[i, j], nodes[i, j + 1], PixelComparator.Difference(pixels[i, j], pixels[i, j + 1], difType))); // Down

                    if (connectingMethod == ConnectingMethod.Connected_8)
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
                if (a != b)
                {
                    if (MergePredicate(a, b, edge.Weight, param))
                        if (a.Count >= b.Count)
                            MergeSegment(a, b, edge.Weight);
                        else MergeSegment(b, a, edge.Weight);
                }
            }

        }

        protected override void MargeSmall(int minSize)
        {
            foreach (Edge edge in edges)
            {
                Segment a = edge.A.Segment;
                Segment b = edge.B.Segment;
                if (a != b)
                {
                    if (a.Count < minSize || b.Count < minSize)
                        if (a.Count >= b.Count)
                            MergeSegment(a, b, edge.Weight);
                        else MergeSegment(b, a, edge.Weight);
                }

            }
        }

        protected override int[,] ReindexSegments(int width, int height)
        {
            int[,] segments = new int[width, height];
            int idx = 0;
            for (int j = 0; j < height; j++)
                for (int i = 0; i < width; i++)
                {
                    if (nodes[i, j].Segment.Id < 0)
                    {
                        nodes[i, j].Segment.Id = idx;
                        idx++;
                    }
                    segments[i, j] = nodes[i, j].Segment.Id;
                }
            return segments;
        }

        protected void MergeSegment(Segment s1, Segment s2, double weight)
        {
            Node nodeB = s2.First;
            while (nodeB != null)
            {
                nodeB.Segment = s1;
                nodeB = nodeB.Next;
            }
            s1.Last.Next = s2.First;
            s1.Last = s2.Last;
            s1.Count += s2.Count;
            s1.SegmentWeight += weight + s2.SegmentWeight;
            s1.MaxWeight = weight;
        }

        protected abstract bool MergePredicate(Segment a, Segment b, double weight, int param);
    }
}
