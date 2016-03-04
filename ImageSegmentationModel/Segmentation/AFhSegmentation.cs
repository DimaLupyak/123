using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            var watch = Stopwatch.StartNew();
            BuildGraph(width, height, pixels, connectingMethod, difType);
            watch.Stop();
            perfomanceInfo.BuildingPerfomance = watch.ElapsedMilliseconds;

            watch = Stopwatch.StartNew();
            SortEdges();
            watch.Stop();
            perfomanceInfo.SortingPerfomance = watch.ElapsedMilliseconds;

            watch = Stopwatch.StartNew();
            DoAlgorithm(k);
            watch.Stop();
            perfomanceInfo.AlgorithmPerfomance = watch.ElapsedMilliseconds;

            watch = Stopwatch.StartNew();
            if (minSize > 0)
                MargeSmall(minSize);
            watch.Stop();
            perfomanceInfo.SmallSegmentMargingPerfomance = watch.ElapsedMilliseconds;
            return ReindexSegments(width, height);
        }
    }
}
