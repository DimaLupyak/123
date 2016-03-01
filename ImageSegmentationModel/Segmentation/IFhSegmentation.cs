using System;

namespace ImageSegmentationModel.Segmentation
{
    public interface IFhSegmentation
    {
        int[,] BuildSegments(int width, int height, RGB[,] pixels, int k, int minSize, ConnectingMethod connectingMethod, ColorDifference difType);
    }
}
