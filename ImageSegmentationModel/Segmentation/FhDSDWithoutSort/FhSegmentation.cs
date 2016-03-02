using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ImageSegmentationModel.Segmentation.FhDSDWithoutSort
{
    public class FhSegmentation : IFhSegmentation
    {
        #region private fields

        private Node[,] nodes;
        private Edge[] edges;
        private Edge[] edgePockets;

        #endregion

        #region private methods

        private void BuildNodes(int width, int height)
        {
            nodes = new Node[width, height];
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                {
                    nodes[x, y] = new Node(y * width + x);
                }
        }

        private void BuildEdges(int width, int height, RGB[,] pixels, ConnectingMethod connectingMethod, ColorDifference difType)
        {
            int edgesPerNode = 0;
            if (connectingMethod == ConnectingMethod.Connecred_4) edgesPerNode = 2;
            else if (connectingMethod == ConnectingMethod.Connecred_8) edgesPerNode = 4;
            else if (connectingMethod == ConnectingMethod.Connecred_16) edgesPerNode = 8;

            edges = new Edge[edgesPerNode * width * height];
            for (int i = 0; i < edges.Length; i++)
                edges[i] = new Edge();
            edgePockets = new Edge[256];

            for (int j = 0; j < height; j++)
                for (int i = 0; i < width; i++)
                {
                    int c = j * width + i;

                    if ((i + 1) < width)
                        CreateEdge(edgesPerNode * c, nodes[i, j], nodes[i + 1, j], ImageHelper.Difference(pixels, i, j, i + 1, j, difType));

                    if ((j + 1) < height)
                        CreateEdge(edgesPerNode * c + 1, nodes[i, j], nodes[i, j + 1], ImageHelper.Difference(pixels, i, j, i, j + 1, difType));

                    if (connectingMethod == ConnectingMethod.Connecred_8)
                    {
                        if ((i + 1 < width) && (j - 1 >= 0))
                            CreateEdge(edgesPerNode * c + 2, nodes[i, j], nodes[i + 1, j - 1], ImageHelper.Difference(pixels, i, j, i + 1, j - 1, difType));

                        if (((i + 1) < width) && ((j + 1) < height))
                            CreateEdge(edgesPerNode * c + 3, nodes[i, j], nodes[i + 1, j + 1], ImageHelper.Difference(pixels, i, j, i + 1, j + 1, difType));
                    }
                }
        }


        private void CreateEdge(int idx, Node v1, Node v2, int diff)
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

        #endregion

        #region IFhSegmentation members

        public int[,] BuildSegments(int width, int height, RGB[,] pixels, int credit, int minSize, ConnectingMethod connectingMethod, ColorDifference difType)
        {
            try
            {
                var watch = Stopwatch.StartNew();
                BuildNodes(width, height);
                watch.Stop();
                MessageBox.Show("" + watch.ElapsedMilliseconds, "GetNodes", MessageBoxButtons.OK);

                watch = Stopwatch.StartNew();
                BuildEdges(width, height, pixels, connectingMethod, difType);
                watch.Stop();
                MessageBox.Show("" + watch.ElapsedMilliseconds, "BuildEdges", MessageBoxButtons.OK);

                watch = Stopwatch.StartNew();
                watch.Stop();
                MessageBox.Show("" + watch.ElapsedMilliseconds, "Sort", MessageBoxButtons.OK);

                watch = Stopwatch.StartNew();

                for (int edgeWeight = 0; edgeWeight < 256; edgeWeight++)
                {
                    Edge edge = edgePockets[edgeWeight];
                    while (edge != null)
                    {
                        int sumSegmentWeight = edge.A.Find().SegmentWeight + edge.B.Find().SegmentWeight;
                        if (edgeWeight + sumSegmentWeight <= credit)
                        {
                            edge.A.Union(edge.B, edgeWeight);
                        }                        
                        edge = edge.Next;
                    }
                }
                
                watch.Stop();
                MessageBox.Show("" + watch.ElapsedMilliseconds, "Marge", MessageBoxButtons.OK);
                watch = Stopwatch.StartNew();
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
                watch.Stop();
                MessageBox.Show("" + watch.ElapsedMilliseconds, "ninSize", MessageBoxButtons.OK);
                int[,] segments = new int[width, height];
                for (int y = 0; y < height; y++)
                    for (int x = 0; x < width; x++)
                        segments[x, y] = nodes[x, y].SegmentId;
                return segments;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion
    }
}
