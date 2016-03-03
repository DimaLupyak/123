using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSegmentationModel.Segmentation.NoSort
{
    abstract class AFhSegmentationNoSort : AFhSegmentation
    {
        protected Node[,] nodes;
        protected Segment[] segments;
        protected Edge[] edges;
        protected Edge[] edgePockets;

        protected override void BuildGraph(int width, int height, RGB[,] pixels, ConnectingMethod connectingMethod, ColorDifference difType)
        {
            int edgesPerNode = 0;
            if (connectingMethod == ConnectingMethod.Connecred_4) edgesPerNode = 2;
            else if (connectingMethod == ConnectingMethod.Connecred_8) edgesPerNode = 4;

            nodes = new Node[width, height];
            for (int j = 0; j < height; j++)
                for (int i = 0; i < width; i++)
                    nodes[i, j] = new Node();
            segments = new Segment[width * height];
            for (int i = 0; i < segments.Length; i++)
                segments[i] = new Segment();
            edges = new Edge[edgesPerNode * width * height];
            for (int i = 0; i < edges.Length; i++)
                edges[i] = new Edge();
            edgePockets = new Edge[256];

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
                    // initialize edge
                    if ((i + 1) < width)
                        CreateEdge(edgesPerNode * c, nodes[i, j], nodes[i + 1, j], (int)PixelComparator.Difference(pixels[i, j], pixels[i + 1, j], difType));

                    if ((j + 1) < height)
                        CreateEdge(edgesPerNode * c + 1, nodes[i, j], nodes[i, j + 1], (int)PixelComparator.Difference(pixels[i, j], pixels[i, j + 1], difType));

                    if (connectingMethod == ConnectingMethod.Connecred_8)
                    {
                        if ((i + 1 < width) && (j - 1 >= 0))
                            CreateEdge(edgesPerNode * c + 2, nodes[i, j], nodes[i + 1, j - 1], (int)PixelComparator.Difference(pixels[i, j], pixels[i + 1, j - 1], difType));

                        if (((i + 1) < width) && ((j + 1) < height))
                            CreateEdge(edgesPerNode * c + 3, nodes[i, j], nodes[i + 1, j + 1], (int)PixelComparator.Difference(pixels[i, j], pixels[i + 1, j + 1], difType));
                    }
                }
        }

        protected override void SortEdges()
        {
        }

        protected override void DoAlgorithm(int param)
        {
            for (int edgeWeight = 0; edgeWeight < 256; edgeWeight++)
            {
                Edge actual = edgePockets[edgeWeight];
                while (actual != null)
                {
                    if (MergePredicate(actual.A.Segment, actual.B.Segment, edgeWeight, param))
                    {
                        Segment c1, c2;
                        if (actual.A.Segment.Count >= actual.B.Segment.Count)
                        {
                            c1 = actual.A.Segment;
                            c2 = actual.B.Segment;
                        }
                        else
                        {
                            c1 = actual.B.Segment;
                            c2 = actual.A.Segment;
                        }
                        AppendComponent(c1, c2, edgeWeight);
                    }
                    actual = actual.Next;
                }
            }
        }

        protected override void MargeSmall(int minSize)
        {
            for (int edgeWeight = 0; edgeWeight < 256; edgeWeight++)
            {
                Edge actual = edgePockets[edgeWeight];
                while (actual != null)
                {
                    if (actual.A.Segment != actual.B.Segment)
                        if (actual.A.Segment.Count < minSize || actual.B.Segment.Count < minSize)
                        {
                            Segment c1, c2;
                            if (actual.A.Segment.Count >= actual.B.Segment.Count)
                            {
                                c1 = actual.A.Segment;
                                c2 = actual.B.Segment;
                            }
                            else
                            {
                                c1 = actual.B.Segment;
                                c2 = actual.A.Segment;
                            }
                            AppendComponent(c1, c2, edgeWeight);
                        }
                    actual = actual.Next;
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

        protected void CreateEdge(int idx, Node v1, Node v2, int diff)
        {
            if (diff > 255) diff = 255;
            if ((diff >= 0) && (diff < 256))
            {
                edges[idx].A = v1;
                edges[idx].B = v2;
                edges[idx].Next = edgePockets[diff];
                edgePockets[diff] = edges[idx];
            }
        }

        protected void AppendComponent(Segment c1, Segment c2, double weight)
        {
            Node nodeB = c2.First;
            while (nodeB != null)
            {
                nodeB.Segment = c1;
                nodeB = nodeB.Next;
            }
            c1.Last.Next = c2.First;
            c1.Last = c2.Last;
            c1.Count += c2.Count;
            c1.SegmentWeight += weight + c2.SegmentWeight;
            c1.MaxWeight = weight;
        }

        protected abstract bool MergePredicate(Segment a, Segment b, double weight, int param);
    }
}
