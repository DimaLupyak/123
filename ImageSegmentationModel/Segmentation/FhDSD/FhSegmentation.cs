using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ImageSegmentationModel.Segmentation.FhDSD
{
    public class FhSegmentation : IFhSegmentation
    {
        #region private fields

        private Node[,] nodes;
        private List<Edge> edges;

        #endregion

        #region private methods

        private Node[,] GetNodes(int width, int height)
        {
            Node[,] nodes = new Node[width, height];
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                {
                    nodes[x, y] = new Node(y * width + x);
                }
            return nodes;
        }

        private List<Edge> BuildEdges(int width, int height, RGB[,] pixels, ConnectingMethod connectingMethod, ColorDifference difType)
        {
            List<Edge> edges = new List<Edge>();
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                {
                    if (x < (width - 1))
                    {
                        edges.Add(new Edge(nodes[x, y], nodes[x + 1, y], ImageHelper.Difference(pixels, x, y, x + 1, y, difType))); // Right

                        if (connectingMethod == ConnectingMethod.Connecred_8)
                        {
                            if (y > 0)
                                edges.Add(new Edge(nodes[x, y], nodes[x + 1, y - 1], ImageHelper.Difference(pixels, x, y, x + 1, y - 1, difType))); // Up-Right

                            if (y < (height - 1))
                                edges.Add(new Edge(nodes[x, y], nodes[x + 1, y + 1], ImageHelper.Difference(pixels, x, y, x + 1, y + 1, difType))); // Down-Right
                        }
                    }
                    if (y < (height - 1))
                        edges.Add(new Edge(nodes[x, y], nodes[x, y + 1], ImageHelper.Difference(pixels, x, y, x, y + 1, difType))); // Down
                }
            return edges;
        }


        #endregion

        #region IFhSegmentation members

        public int[,] BuildSegments(int width, int height, RGB[,] pixels, int credit, int minSize, ConnectingMethod connectingMethod, ColorDifference difType)
        {
            try
            {
                var watch = Stopwatch.StartNew();
                nodes = GetNodes(width, height);
                watch.Stop();
                MessageBox.Show("" + watch.ElapsedMilliseconds, "GetNodes", MessageBoxButtons.OK);

                watch = Stopwatch.StartNew();
                edges = BuildEdges(width, height, pixels, connectingMethod, difType);
                watch.Stop();
                MessageBox.Show("" + watch.ElapsedMilliseconds, "BuildEdges", MessageBoxButtons.OK);

                watch = Stopwatch.StartNew();
                edges.Sort();
                watch.Stop();
                MessageBox.Show("" + watch.ElapsedMilliseconds, "Sort", MessageBoxButtons.OK);

                watch = Stopwatch.StartNew();
                foreach (Edge edge in edges)
                {
                        int sumSegmentWeight = edge.A.Find().SegmentWeight + edge.B.Find().SegmentWeight;
                        if (edge.Weight + sumSegmentWeight <= credit)
                        {
                            edge.A.Union(edge.B, edge.Weight);
                        }
                }
                watch.Stop();
                MessageBox.Show("" + watch.ElapsedMilliseconds, "Marge", MessageBoxButtons.OK);
                watch = Stopwatch.StartNew();
                foreach (Edge edge in edges)
                {
                    if (edge.A.Find().Size < minSize || edge.B.Find().Size < minSize)
                    {
                        edge.A.Union(edge.B, edge.Weight);
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
