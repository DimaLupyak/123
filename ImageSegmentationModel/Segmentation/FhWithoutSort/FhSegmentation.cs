using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageSegmentationModel.Segmentation.FhWithoutSort
{
    public class FhSegmentation : IFhSegmentation
    {
        #region private fields

        private Node[,] nodes;
        private Component[] components;
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
            components = new Component[width * height];
            for (int i = 0; i < components.Length; i++)
                components[i] = new Component();
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
                    nodes[i, j].Component = components[c];
                    // initialize component
                    components[c].Index = -1;
                    components[c].Count = 1;
                    components[c].MaxWeight = 0;
                    components[c].First = components[c].Last = nodes[i, j];
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
                    if(connectingMethod == ConnectingMethod.Connecred_16)
                    {
                        if ((i + 2) < width)
                            CreateEdge(edgesPerNode * c+4, nodes[i, j], nodes[i + 2, j], ImageHelper.Difference(pixels, i, j, i + 2, j, difType));

                        if ((j + 2) < height)
                            CreateEdge(edgesPerNode * c + 5, nodes[i, j], nodes[i, j + 2], ImageHelper.Difference(pixels, i, j, i, j + 2, difType));

                        if ((i + 2 < width) && (j - 2 >= 0))
                            CreateEdge(edgesPerNode * c + 6, nodes[i, j], nodes[i + 2, j - 2], ImageHelper.Difference(pixels, i, j, i + 2, j - 2, difType));

                        if (((i + 2) < width) && ((j + 2) < height))
                            CreateEdge(edgesPerNode * c + 7, nodes[i, j], nodes[i + 2, j + 2], ImageHelper.Difference(pixels, i, j, i + 2, j + 2, difType));
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
                    if ((actual.V1.Component != actual.V2.Component) &&
                        (idx <= Math.Min(actual.V1.Component.MaxWeight + k / actual.V1.Component.Count,
                                                actual.V2.Component.MaxWeight + k / actual.V2.Component.Count)))
                    {
                        Component c1, c2;
                        if (actual.V1.Component.Count >= actual.V2.Component.Count)
                        {
                            c1 = actual.V1.Component;
                            c2 = actual.V2.Component;
                        }
                        else
                        {
                            c1 = actual.V2.Component;
                            c2 = actual.V1.Component;
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
                    if ((actual.V1.Component != actual.V2.Component) &&
                        (actual.V1.Component.Count < minSize || actual.V2.Component.Count < minSize))
                    {
                        Component c1, c2;
                        if (actual.V1.Component.Count >= actual.V2.Component.Count)
                        {
                            c1 = actual.V1.Component;
                            c2 = actual.V2.Component;
                        }
                        else
                        {
                            c1 = actual.V2.Component;
                            c2 = actual.V1.Component;
                        }
                        AppendComponent(c1, c2, idx);
                    }
                    actual = actual.Next;
                }
            }
        }

        private void AppendComponent(Component c1, Component c2, double weight)
        {
            Node nodeB = c2.First;
            while (nodeB != null)
            {
                nodeB.Component = c1;
                nodeB = nodeB.Next;
            }
            c1.Last.Next = c2.First;
            c1.Last = c2.Last;
            c1.Count += c2.Count;
            c1.MaxWeight = weight;
        }

        private int[,] ReindexSegments(int width, int height)
        {
            int[,] segments = new int[width, height];
            int idx = 0;
            for (int j = 0; j < height; j++)
                for (int i = 0; i < width; i++)
                {
                    if (nodes[i, j].Component.Index < 0)
                    {
                        nodes[i, j].Component.Index = idx;
                        idx++;
                    }
                    segments[i, j] = nodes[i, j].Component.Index;
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
