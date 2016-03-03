using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSegmentationModel.Segmentation
{
    abstract class AFhSegmentation : IFhSegmentation
    {
        protected abstract void BuildGraph(int width, int height, RGB[,] pixels, ConnectingMethod connectingMethod, ColorDifference difType);
        protected abstract void SortEdges();
        protected abstract void DoAlgorithm(int k);
        protected abstract void MargeSmall(int minSize);
        protected abstract int[,] ReindexSegments(int width, int height);
        public int[,] BuildSegments(int width, int height, RGB[,] pixels, int k, int minSize, ConnectingMethod connectingMethod, ColorDifference difType, ref PerfomanceInfo perfomanceInfo)
        {
            BuildGraph(width, height, pixels, connectingMethod, difType);
            SortEdges();
            DoAlgorithm(k);
            if (minSize > 0)
                MargeSmall(minSize);
            return ReindexSegments(width, height);
        }
    }
}
