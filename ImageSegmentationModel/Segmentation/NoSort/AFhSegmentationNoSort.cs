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
        protected List<Segment> segments;
        protected Edge[] edges;
        protected Edge[] edgePockets;

        protected override void BuildGraph(int width, int height, RGB[,] pixels, ConnectingMethod connectingMethod, ColorDifference difType, int[,] superPixels = null)
        {
            int edgesPerNode = 0;
            if (connectingMethod == ConnectingMethod.Connected_4) edgesPerNode = 2;
            else if (connectingMethod == ConnectingMethod.Connected_8) edgesPerNode = 4;


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

            edges = new Edge[edgesPerNode * width * height];
            for (int i = 0; i < edges.Length; i++)
                edges[i] = new Edge();
            edgePockets = new Edge[256];

            for (int j = 0; j < height; j++)
                for (int i = 0; i < width; i++)
                {
                    int c = j * width + i;
                    // initialize edge
                    if ((i + 1) < width)
                        CreateEdge(edgesPerNode * c, nodes[i, j], nodes[i + 1, j], (int)PixelComparator.Difference(pixels[i, j], pixels[i + 1, j], difType));

                    if ((j + 1) < height)
                        CreateEdge(edgesPerNode * c + 1, nodes[i, j], nodes[i, j + 1], (int)PixelComparator.Difference(pixels[i, j], pixels[i, j + 1], difType));

                    if (connectingMethod == ConnectingMethod.Connected_8)
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
                        if (actual.A.Segment.Nodes.Count >= actual.B.Segment.Nodes.Count)
                        {
                            c1 = actual.A.Segment;
                            c2 = actual.B.Segment;
                        }
                        else
                        {
                            c1 = actual.B.Segment;
                            c2 = actual.A.Segment;
                        }
                        MergeSegment(c1, c2, edgeWeight, c1.SegmentWeight);
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
                        if (actual.A.Segment.Nodes.Count < minSize || actual.B.Segment.Nodes.Count < minSize)
                        {
                            Segment s1, s2;
                            if (actual.A.Segment.Nodes.Count >= actual.B.Segment.Nodes.Count)
                            {
                                s1 = actual.A.Segment;
                                s2 = actual.B.Segment;
                            }
                            else
                            {
                                s1 = actual.B.Segment;
                                s2 = actual.A.Segment;
                            }
                            MergeSegment(s1, s2, edgeWeight, s1.SegmentWeight);
                        }
                    actual = actual.Next;
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

        protected void MergeSegment(Segment a, Segment b, double weight, double bNodesCredit)
        {
            a.AddNodes(b.Nodes, weight, bNodesCredit);
            b.Clear();
        }

        protected abstract bool MergePredicate(Segment a, Segment b, double weight, int param);
    }
}
