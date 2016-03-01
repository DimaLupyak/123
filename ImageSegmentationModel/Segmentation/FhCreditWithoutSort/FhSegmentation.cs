using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageSegmentationModel.Segmentation.FhCreditWithoutSort
{
    public class FhSegmentation : IFhSegmentation
    {
        #region private fields

        private Node[,] nodes;
        private Segment[] segments;
        private Edge[] edges;
        private Edge[] edgePockets;

        #endregion

        #region private methods

        private void CreateArrays(int width, int height, ConnectingMethod connectingMethod)
        {
            int edgesPerNode = 0;
            if (connectingMethod == ConnectingMethod.Connecred_4) edgesPerNode = 2;
            else if(connectingMethod == ConnectingMethod.Connecred_8) edgesPerNode = 4;
            else if (connectingMethod == ConnectingMethod.Connecred_16) edgesPerNode = 8;

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
        }

        private void FillArrays(int width, int height, RGB[,] pixels, int k, ConnectingMethod connectingMethod, ColorDifference difType)
        {
            int edgesPerNode = 0;
            if (connectingMethod == ConnectingMethod.Connecred_4) edgesPerNode = 2;
            else if (connectingMethod == ConnectingMethod.Connecred_8) edgesPerNode = 4;
            else if (connectingMethod == ConnectingMethod.Connecred_16) edgesPerNode = 8;

            for (int j = 0; j < height; j++)
                for (int i = 0; i < width; i++)
                {
                    int c = j * width + i;
                    // initialize node
                    nodes[i, j].Segment = segments[c];
                    // initialize component
                    segments[c].Id = -1;
                    segments[c].Count = 1;
                    segments[c].Credit = k;
                    segments[c].First = segments[c].Last = nodes[i, j];
                    // initialize edge
                    if ((i + 1) < width)
                        CreateEdge(edgesPerNode * c, nodes[i, j], nodes[i + 1, j], ImageHelper.Difference(pixels, i, j, i + 1, j, difType));

                    if ((j + 1) < height)
                        CreateEdge(edgesPerNode * c + 1, nodes[i, j], nodes[i, j + 1], ImageHelper.Difference(pixels, i, j, i, j + 1, difType));

                    if (connectingMethod == ConnectingMethod.Connecred_8) {
                        if ((i + 1 < width) && (j - 1>= 0))
                            CreateEdge(edgesPerNode * c + 2, nodes[i, j], nodes[i + 1, j - 1], ImageHelper.Difference(pixels, i, j, i + 1, j - 1, difType));

                        if (((i + 1) < width) && ((j + 1) < height))
                            CreateEdge(edgesPerNode * c + 3, nodes[i, j], nodes[i + 1, j + 1], ImageHelper.Difference(pixels, i, j, i + 1, j + 1, difType));
                    }
                }
        }

        private void CreateEdge(int idx, Node v1, Node v2, int diff)
        {
            if(diff>255) diff = 255;
            if ((diff >= 0) && (diff < 256))
            {
                edges[idx].V1 = v1;
                edges[idx].V2 = v2;
                edges[idx].Next = edgePockets[diff];
                edgePockets[diff] = edges[idx];
            }
        }

        private void DoAlgorithm(int k)
        {
            for (int idx = 0; idx < 256; idx++)
            {
                Edge actual = edgePockets[idx];
                while (actual != null)
                {
                    if ((actual.V1.Segment != actual.V2.Segment) &&
                        (idx <= Math.Min(actual.V1.Segment.Credit , actual.V2.Segment.Credit)))
                    {
                        Segment c1, c2;
                        if (actual.V1.Segment.Count >= actual.V2.Segment.Count)
                        {
                            c1 = actual.V1.Segment;
                            c2 = actual.V2.Segment;
                        }
                        else
                        {
                            c1 = actual.V2.Segment;
                            c2 = actual.V1.Segment;
                        }
                        AppendComponent(c1, c2, idx);
                    }
                    actual = actual.Next;
                }
            }
        }

        private void MargeSmall(int minSize)
        {
            for (int idx = 0; idx < 256; idx++)
            {
                Edge actual = edgePockets[idx];
                while (actual != null)
                {
                    if ((actual.V1.Segment != actual.V2.Segment) &&
                        (actual.V1.Segment.Count < minSize || actual.V2.Segment.Count < minSize))
                    {
                        Segment c1, c2;
                        if (actual.V1.Segment.Count >= actual.V2.Segment.Count)
                        {
                            c1 = actual.V1.Segment;
                            c2 = actual.V2.Segment;
                        }
                        else
                        {
                            c1 = actual.V2.Segment;
                            c2 = actual.V1.Segment;
                        }
                        AppendComponent(c1, c2, idx);
                    }
                    actual = actual.Next;
                }
            }
        }

        private void AppendComponent(Segment c1, Segment c2, double weight)
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
            c1.Credit -= weight;
        }

        private int[,] ReindexSegments(int width, int height)
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

        #endregion

        #region IFhSegmentation members

        public int[,] BuildSegments(int width, int height, RGB[,] pixels, int k, int minSize, ConnectingMethod connectingMethod, ColorDifference difType)
        {
            try
            {
                CreateArrays(width, height, connectingMethod);
                FillArrays(width, height, pixels, k, connectingMethod, difType);
                DoAlgorithm(k );
                MargeSmall(minSize);
                return ReindexSegments(width, height);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion
    }
}
