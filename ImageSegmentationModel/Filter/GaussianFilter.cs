using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSegmentationModel.Filter
{
    public class GaussianFilter
    {
        #region convolution methods

        private enum Parametr
        {
            R, G, B
        }

        private byte Convolution(int i, int j, int width, int height, RGB[,] pixels, Kernel kernel, Parametr paramtr)
        {
            double sum = 0;
            for (int k = -kernel.R; k <= kernel.R; k++)
                for (int l = -kernel.R; l <= kernel.R; l++)
                {
                    int parametrValue = 0;
                    switch (paramtr)
                    {
                        case Parametr.R:
                            parametrValue = pixels[GetPixelIndex(i + k, width), GetPixelIndex(j + l, height)].Red;
                            break;
                        case Parametr.G:
                            parametrValue = pixels[GetPixelIndex(i + k, width), GetPixelIndex(j + l, height)].Green;
                            break;
                        case Parametr.B:
                            parametrValue = pixels[GetPixelIndex(i + k, width), GetPixelIndex(j + l, height)].Blue;
                            break;
                    }
                    sum += kernel.Matrix[k + kernel.R, l + kernel.R] * parametrValue;
                }
            return (byte)Math.Round(sum);
        }

        private int GetPixelIndex(int i, int max)
        {
            if (i < 0)
                return Math.Abs(i);
            else if (i >= max)
                return max - Math.Abs(i - max + 1);
            return i;
        }

        #endregion

        #region public method

        public void Filter(int width, int height, RGB[,] pixels, double sigma)
        {
            Kernel kernel = new Kernel(sigma);
            RGB[,] filter = new RGB[width, height];
            for (int j = 0; j < height; j++)
                for (int i = 0; i < width; i++)
                {
                    filter[i, j].Red = Convolution(i, j, width, height, pixels, kernel, Parametr.R);
                    filter[i, j].Green = Convolution(i, j, width, height, pixels, kernel, Parametr.G);
                    filter[i, j].Blue = Convolution(i, j, width, height, pixels, kernel, Parametr.B);
                }
            for (int j = 0; j < height; j++)
                for (int i = 0; i < width; i++)
                    pixels[i, j] = filter[i, j];
        }

        #endregion
    }
}
