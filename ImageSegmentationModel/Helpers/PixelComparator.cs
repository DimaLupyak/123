using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSegmentationModel
{
    public static class PixelComparator
    {

        const double K1 = 0.045;
        const double K2 = 0.015;
        const double d2r = Math.PI / 180;
        const double pow25_7 = 25.0 * 25 * 25 * 25 * 25 * 25 * 25;
        const double deg180 = 180 * d2r;
        const double deg360 = 360 * d2r;
        const double deg275 = 275 * d2r;
        const double deg30 = 30 * d2r;
        const double deg6 = 6 * d2r;
        const double deg63 = 63 * d2r;     

        public static double Difference(RGB rgb1, RGB rgb2, ColorDifference type)
        {
            switch (type)
            {
                case ColorDifference.RGB_std_deviation:
                    return RGB_std_deviation(rgb1, rgb2);

                 case ColorDifference.CIE76:
                     return CIE76(rgb1, rgb2);

                case ColorDifference.CIE94:
                    return CIE94(rgb1, rgb2);

                case ColorDifference.CIE2000:
                    return CIE2000(rgb1, rgb2);

                case ColorDifference.dRed:
                     return Math.Abs(rgb1.Red - rgb2.Red);
                 case ColorDifference.dGreen:
                     return Math.Abs(rgb1.Green - rgb2.Green);
                 case ColorDifference.dBlue:
                     return Math.Abs(rgb1.Blue - rgb2.Blue);
                 case ColorDifference.Grey:
                     return Math.Abs(ColorModelConverter.RGBtoGray(rgb1) - ColorModelConverter.RGBtoGray(rgb2));
                default:
                    return 0;
            }

        }

        private static double RGB_std_deviation(RGB rgb1, RGB rgb2)
        {
            return Math.Sqrt((
                Math.Pow(rgb1.Red - rgb2.Red, 2)
                + Math.Pow(rgb1.Green - rgb2.Green, 2)
                + Math.Pow(rgb1.Blue - rgb2.Blue, 2)) / 3
                );
        }

        private static double CIE76(RGB rgb1, RGB rgb2)
        {
            CIELab Lab1 = ColorModelConverter.RGBtoLab(rgb1.Red, rgb1.Green, rgb1.Blue);
            CIELab Lab2 = ColorModelConverter.RGBtoLab(rgb2.Red, rgb2.Green, rgb2.Blue);

            return Math.Sqrt((
                Math.Pow(Lab1.L - Lab2.L, 2)
                + Math.Pow(Lab1.A - Lab2.A, 2)
                + Math.Pow(Lab1.B - Lab2.B, 2))
            );
        }

        private static double CIE94(RGB rgb1, RGB rgb2)
        {
            CIELab Lab1 = ColorModelConverter.RGBtoLab(rgb1.Red, rgb1.Green, rgb1.Blue);
            CIELab Lab2 = ColorModelConverter.RGBtoLab(rgb2.Red, rgb2.Green, rgb2.Blue);

            double Da = Lab1.A - Lab2.A;
            double Db = Lab1.B - Lab2.B;
            double C1 = Math.Sqrt(Math.Pow(Lab1.A, 2) + Math.Pow(Lab1.B, 2));
            double C2 = Math.Sqrt(Math.Pow(Lab2.A, 2) + Math.Pow(Lab2.B, 2));
            double Sc = 1 + K1 * C1;
            double Sh = 1 + K2 * C1;
            double Dl = Lab1.L - Lab2.L;
            double Dc = C1 - C2;
            double Dh2 = Math.Pow(Da, 2) + Math.Pow(Db, 2) - Math.Pow(Dc, 2);

            return Math.Sqrt(Math.Pow(Dl, 2) + Math.Pow(Dc / Sc, 2) + Dh2 / Math.Pow(Sh, 2));
        }

        private static double CIE2000(RGB rgb1, RGB rgb2)
        {
            CIELab Lab1 = ColorModelConverter.RGBtoLab(rgb1.Red, rgb1.Green, rgb1.Blue);
            CIELab Lab2 = ColorModelConverter.RGBtoLab(rgb2.Red, rgb2.Green, rgb2.Blue);

            double L1 = Lab1.L;
            double L2 = Lab2.L;
            double a1 = Lab1.A;
            double a2 = Lab2.A;
            double b1 = Lab1.B;
            double b2 = Lab2.B;

            double Ltd = (L1 + L2) / 2;
            double C1 = Math.Sqrt(Math.Pow(a1, 2) + Math.Pow(b1, 2));
            double C2 = Math.Sqrt(Math.Pow(a2, 2) + Math.Pow(b2, 2));
            double Ct = (C1 + C2) / 2;

            double Ct7 = Math.Pow(Ct, 7);
            double G = (1 - Math.Sqrt(Ct7 / (Ct7 + pow25_7))) / 2;

            double ad1 = a1 * (1 + G);
            double ad2 = a2 * (1 + G);

            double Cd1 = Math.Sqrt(Math.Pow(ad1, 2) + Math.Pow(b1, 2));
            double Cd2 = Math.Sqrt(Math.Pow(ad2, 2) + Math.Pow(b2, 2));
            double Ctd = (Cd1 + Cd2) / 2;

            double atanhd1 = Math.Atan2(b1, ad1);
            double hd1 = atanhd1 + (atanhd1 >= 0 ? 0 : deg360);
            double atanhd2 = Math.Atan2(b2, ad2);
            double hd2 = atanhd2 + (atanhd2 >= 0 ? 0 : deg360);

            double sdh = hd1 + hd2;
            double dh1 = hd1 - hd2;
            double adh1 = Math.Abs(dh1);
            double Htd = (sdh + (adh1 > deg180 ? deg360 : 0)) / 2;

            double T = 1 - 0.17 * Math.Cos(Htd - deg30) + 0.24 * Math.Cos(2 * Htd) + 0.32 * Math.Cos(2 * Htd + deg6) - 0.20 * Math.Cos(4 * Htd - deg63);

            double dh2 = hd2 - hd1;
            double adh2 = Math.Abs(dh2);
            double dhd = adh2 <= deg180 ? dh2 : (hd2 <= hd1 ? (dh2 + deg360) : (dh2 - deg360));

            double dLd = L2 - L1;
            double dCd = Cd2 - Cd1;
            double dHd = 2 * Math.Sqrt(Cd1 * Cd2) * Math.Sin(dhd / 2);

            double Ltd2 = Math.Pow(Ltd - 50, 2);
            double Sl = 1 + (K2 * Ltd2) / Math.Sqrt(20 + Ltd2);
            double Sc = 1 + K1 * Ctd;
            double Sh = 1 + K2 * Ctd * T;

            double ctd7 = Math.Pow(Ctd, 7);
            double dO = 30 * Math.Exp(-Math.Pow((Htd - deg275) / 25, 2));

            double Rc = 2 * Math.Sqrt(ctd7 / (ctd7 + pow25_7));
            double Rt = -Rc * Math.Sin(2 * dO);

            double SDE = Math.Pow(dLd / Sl, 2) + Math.Pow(dCd / Sc, 2) + Math.Pow(dHd / Sh, 2) + Rt * (dCd / Sc) * (dHd / Sh);

            return Math.Sqrt(SDE);
        }
    }
}
