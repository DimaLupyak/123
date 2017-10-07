using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSegmentationModel.Segmentation.DSD
{
    abstract class AFhSegmentationDSD : AFhSegmentation
    {
        protected Node[,] nodes;
        protected List<Edge> edges;

        protected override void BuildGraph(int width, int height, RGB[,] pixels, ConnectingMethod connectingMethod, ColorDifference difType, int[,] superPixels = null)
        {
            nodes = new Node[width, height];
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                {
                    nodes[x, y] = new Node(y * width + x);
                }

            edges = new List<Edge>();
            for (int j = 0; j < height; j++)
                for (int i = 0; i < width; i++)
                {
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
                    if (MergePredicate(edge.A.Find(), edge.B.Find(), edge.Weight, param))
                        edge.A.Union(edge.B, edge.Weight);                
            }

        }

        protected override void MargeSmall(int minSize)
        {
            foreach (Edge edge in edges)
            {
                if (edge.A.Find().Size < minSize || edge.B.Find().Size < minSize)
                {
                    edge.A.Union(edge.B, edge.Weight);
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
        
        protected abstract bool MergePredicate(Node a, Node b, double weight, int param);
    }
}
