using System;

namespace ImageSegmentationModel.Segmentation
{
    public interface IFhSegmentation
    {
        int[,] BuildSegments(int width, int height, byte[,] pixels, int k, int minSize, ConnectingMethod connectingMethod);
    }
}
