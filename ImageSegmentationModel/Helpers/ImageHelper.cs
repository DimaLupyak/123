﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ImageSegmentationModel
{



    public static class ImageHelper
    {


        private static RGB[,] GetPixelsMatrix(byte[] pixels, int width, int height, int stride, int bytesPerPixel)
        {
            RGB[,] matrix = new RGB[width, height];
            for (int j = 0; j < height; j++)
                for (int i = 0; i < width; i++)
                {
                    int idx = j * stride + i * bytesPerPixel;
                    if (bytesPerPixel >= 3)
                    {
                        matrix[i, j].Blue = pixels[idx + 0];
                        matrix[i, j].Green = pixels[idx + 1];
                        matrix[i, j].Red = pixels[idx + 2];
                    }
                }
            return matrix;
        }

        private static void FillFilterPixels(RGB[,] filter, byte[] pixels, int width, int height, int stride)
        {
            Random random = new Random();
            Dictionary<int, byte[]> colors = new Dictionary<int, byte[]>();
            for (int j = 0; j < height; j++)
                for (int i = 0; i < width; i++)
                {
                    int idx = j * stride + i * 3;
                    pixels[idx] = filter[i, j].Blue;
                    pixels[idx + 1] = filter[i, j].Green;
                    pixels[idx + 2] = filter[i, j].Red;
                }
        }

        public static T[,] ToMatrix<T>(T[] array, int width, int height, int stride)
        {
            var matrix = new T[width, height];
            for (var j = 0; j < height; j++)
                for (var i = 0; i < width; i++)
                {
                    var idx = j * height + i;
                    matrix[i, j] = array[idx];
                }
            return matrix;
        }

        public static T[] ToArray<T>(T[,] matrix, int width, int height, int stride)
        {
            var array = new T[width * height];
            for (var j = 0; j < height; j++)
                for (var i = 0; i < width; i++)
                {
                    var idx = j * height + i;
                    array[idx] = matrix[i, j];
                }
            return array;
        }

        public static RGB[,] GetPixels(Bitmap bitmap)
        {
            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
            int bytes = bitmapData.Stride * bitmap.Height;
            byte[] pixels = new byte[bytes];
            Marshal.Copy(bitmapData.Scan0, pixels, 0, bytes);
            RGB[,] pixelMatrix = null;
            if (bitmap.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                pixelMatrix = GetPixelsMatrix(pixels, bitmapData.Width, bitmapData.Height, bitmapData.Stride, 1);
            }
            else if (bitmap.PixelFormat == PixelFormat.Format24bppRgb)
            {
                pixelMatrix = GetPixelsMatrix(pixels, bitmapData.Width, bitmapData.Height, bitmapData.Stride, 3);
            }
            else if ((bitmap.PixelFormat == PixelFormat.Format32bppRgb) || (bitmap.PixelFormat == PixelFormat.Format32bppArgb))
            {
                pixelMatrix = GetPixelsMatrix(pixels, bitmapData.Width, bitmapData.Height, bitmapData.Stride, 4);
            }
            bitmap.UnlockBits(bitmapData);

            return pixelMatrix;
        }

        public static int[,] GetSegments(Bitmap bitmap)
        {
            if (bitmap.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
                int bytes = bitmapData.Stride * bitmap.Height;
                byte[] pixels = new byte[bytes];
                Marshal.Copy(bitmapData.Scan0, pixels, 0, bytes);

                int[,] segments = GetSegments(pixels, bitmapData.Width, bitmapData.Height, bitmapData.Stride, 1);

                bitmap.UnlockBits(bitmapData);

                return segments;
            }
            else if (bitmap.PixelFormat == PixelFormat.Format24bppRgb)
            {
                BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
                int bytes = bitmapData.Stride * bitmap.Height;
                byte[] pixels = new byte[bytes];
                Marshal.Copy(bitmapData.Scan0, pixels, 0, bytes);

                int[,] segments = GetSegments(pixels, bitmapData.Width, bitmapData.Height, bitmapData.Stride, 3);

                bitmap.UnlockBits(bitmapData);

                return segments;
            }
            else if ((bitmap.PixelFormat == PixelFormat.Format32bppRgb) || (bitmap.PixelFormat == PixelFormat.Format32bppArgb))
            {
                BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
                int bytes = bitmapData.Stride * bitmap.Height;
                byte[] pixels = new byte[bytes];
                Marshal.Copy(bitmapData.Scan0, pixels, 0, bytes);
                int[,] segments = GetSegments(pixels, bitmapData.Width, bitmapData.Height, bitmapData.Stride, 4);

                bitmap.UnlockBits(bitmapData);

                return segments;
            }
            return null;
        }

        public static Bitmap GetBitmap(int[,] segments)
        {
            Bitmap bitmap = new Bitmap(segments.GetLength(0), segments.GetLength(1), PixelFormat.Format24bppRgb);

            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            int bytes = Math.Abs(bitmapData.Stride) * bitmap.Height;
            byte[] pixels = new byte[bytes];
            Marshal.Copy(bitmapData.Scan0, pixels, 0, bytes);

            FillPixels(segments, pixels, bitmapData.Width, bitmapData.Height, bitmapData.Stride);

            Marshal.Copy(pixels, 0, bitmapData.Scan0, bytes);
            bitmap.UnlockBits(bitmapData);

            return bitmap;
        }

        private static void FillPixels(int[,] segments, byte[] pixels, int width, int height, int stride)
        {
            Random random = new Random();
            Dictionary<int, byte[]> colors = new Dictionary<int, byte[]>();
            for (int j = 0; j < height; j++)
                for (int i = 0; i < width; i++)
                {
                    int idx = j * stride + i * 3;
                    if (!colors.Keys.Contains(segments[i, j]))
                    {
                        byte[] rgb = new byte[3];
                        random.NextBytes(rgb);
                        colors.Add(segments[i, j], rgb);
                    }
                    byte[] color = colors[segments[i, j]];
                    pixels[idx] = color[0];
                    pixels[idx + 1] = color[1];
                    pixels[idx + 2] = color[2];
                }
        }

        public static Bitmap GetBitmap(int[,] segments, RGB[,] origin, bool makeBorders)
        {
            Bitmap bitmap = new Bitmap(segments.GetLength(0), segments.GetLength(1), PixelFormat.Format24bppRgb);

            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            int bytes = Math.Abs(bitmapData.Stride) * bitmap.Height;
            byte[] pixels = new byte[bytes];
            Marshal.Copy(bitmapData.Scan0, pixels, 0, bytes);

            FillPixels(segments, pixels, bitmapData.Width, bitmapData.Height, bitmapData.Stride, origin, makeBorders);

            Marshal.Copy(pixels, 0, bitmapData.Scan0, bytes);
            bitmap.UnlockBits(bitmapData);

            return bitmap;
        }

        private static void FillPixels(int[,] segments, byte[] pixels, int width, int height, int stride, RGB[,] origin, bool makeBorders)
        {
            Dictionary<int, byte[]> colors = new Dictionary<int, byte[]>();
            /*for (int j = 0; j < height; j++)
                for (int i = 0; i < width; i++)
                {
                    int n = 0;
                    int sum = 0;

                }*/
            for (int j = 0; j < height; j++)
                for (int i = 0; i < width; i++)
                {
                    int idx = j * stride + i * 3;
                    if (!colors.Keys.Contains(segments[i, j]))
                    {
                        byte[] rgb = new byte[3];
                        long red = 0;
                        long blue = 0;
                        long grean = 0;
                        var count = 0;
                        for (var x = 0; x < height; x++)
                            for (var y = 0; y < width; y++)
                            {
                                if (segments[i, j] != segments[y, x]) continue;
                                red += origin[y, x].Red;
                                blue += origin[y, x].Blue;
                                grean += origin[y, x].Green;
                                count++;
                            }
                        rgb[0] = (byte)(blue / count);
                        rgb[1] = (byte)(grean / count);
                        rgb[2] = (byte)(red / count);
                        colors.Add(segments[i, j], rgb);
                    }
                    if (makeBorders && i != 0 && j != 0 && i != width - 1 && j != height - 1)
                    {
                        if (segments[i, j] != segments[i - 1, j] ||
                            segments[i, j] != segments[i + 1, j] ||
                            segments[i, j] != segments[i, j - 1] ||
                            segments[i, j] != segments[i, j + 1])
                        {
                            pixels[idx] = 1;
                            pixels[idx + 1] = 1;
                            pixels[idx + 2] = 1;
                            continue;
                        }
                    }
                    byte[] color = colors[segments[i, j]];
                    pixels[idx] = color[0];
                    pixels[idx + 1] = color[1];
                    pixels[idx + 2] = color[2];
                }
        }

        private static int[,] GetSegments(byte[] pixels, int width, int height, int stride, int bytesPerPixel)
        {
            int[,] segments = new int[width, height];
            for (int j = 0; j < height; j++)
                for (int i = 0; i < width; i++)
                {
                    int idx = j * stride + i * bytesPerPixel;
                    int seg = 0;
                    for (int b = 0; b < bytesPerPixel; b++)
                        seg += (pixels[idx + b] << (b * 8));
                    segments[i, j] = seg;
                }
            return segments;
        }

        public static Bitmap GetFilterBitmap(RGB[,] filter)
        {
            Bitmap bitmap = new Bitmap(filter.GetLength(0), filter.GetLength(1), PixelFormat.Format24bppRgb);

            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            int bytes = Math.Abs(bitmapData.Stride) * bitmap.Height;
            byte[] pixels = new byte[bytes];
            Marshal.Copy(bitmapData.Scan0, pixels, 0, bytes);

            FillFilterPixels(filter, pixels, bitmapData.Width, bitmapData.Height, bitmapData.Stride);

            Marshal.Copy(pixels, 0, bitmapData.Scan0, bytes);
            bitmap.UnlockBits(bitmapData);

            return bitmap;
        }
    }
}
