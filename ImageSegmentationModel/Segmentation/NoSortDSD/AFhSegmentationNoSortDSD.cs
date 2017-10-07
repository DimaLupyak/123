using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSegmentationModel.Segmentation.NoSortDSD
{
    abstract class AFhSegmentationNoSortDSD : AFhSegmentation
    {
        #region private fields
        protected Node[,] nodes;
        protected Edge[] edges;
        protected Edge[] edgePockets;
        #endregion

        protected override void BuildGraph(int width, int height, RGB[,] pixels, ConnectingMethod connectingMethod, ColorDifference difType, int[,] superPixels = null)
        {
            int edgesPerNode = 0;
            if (connectingMethod == ConnectingMethod.Connected_4) edgesPerNode = 2;
            else if (connectingMethod == ConnectingMethod.Connected_8) edgesPerNode = 4;

            nodes = new Node[width, height];
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                {
                    nodes[x, y] = new Node(y * width + x);
                }

            edges = new Edge[edgesPerNode * width * height];
            for (int i = 0; i < edges.Length; i++)
                edges[i] = new Edge();
            edgePockets = new Edge[256];

            for (int j = 0; j < height; j++)
                for (int i = 0; i < width; i++)
                {
                    int c = j * width + i;

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

        protected override void SortEdges() { }

        protected override void DoAlgorithm(int param)
        {
            for (int edgeWeight = 0; edgeWeight < 256; edgeWeight++)
            {
                Edge edge = edgePockets[edgeWeight];
                while (edge != null)
                {
                    if (MergePredicate(edge.A.Find(), edge.B.Find(), edgeWeight, param))
                    {
                        edge.A.Union(edge.B, edgeWeight);
                    }
                    edge = edge.Next;
                }
            }
        }

        protected override void MargeSmall(int minSize)
        {
            for (int edgeWeight = 0; edgeWeight < 256; edgeWeight++)
            {
                Edge edge = edgePockets[edgeWeight];
                while (edge != null)
                {
                    if (edge.A.Find().Size < minSize || edge.B.Find().Size < minSize)
                    {
                        edge.A.Union(edge.B, edgeWeight);
                    }
                    edge = edge.Next;
                }
            }
        }

        protected override int[,] ReindexSegments(int width, int height)
        {
            int[,] segments = new int[width, height];
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    segments[x, y] = nodes[x, y].SegmentId;
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
        protected abstract bool MergePredicate(Node a, Node b, double weight, int param);
    }
}
