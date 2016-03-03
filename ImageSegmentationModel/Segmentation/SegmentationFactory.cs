using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageSegmentationModel.Segmentation
{    
    public class SegmentationFactory
    {
        private static SegmentationFactory instance;
        public static SegmentationFactory Instance
        {
            get { return instance ?? (instance = new SegmentationFactory()); }
        }       

        public IFhSegmentation GetFhSegmentation(SegmentationMethod method)
        {
            switch (method)
            {
                case SegmentationMethod.OriginalFh:
                    return new Classic.Fh.FhSegmentation();

                case SegmentationMethod.OriginalCreditFh:
                    return new Classic.FhCredit.FhSegmentation();

                case SegmentationMethod.DSDFh:
                    return new DSD.Fh.FhSegmentation();

                case SegmentationMethod.DSDCreditFh:
                    return new DSD.FhCredit.FhSegmentation();

                case SegmentationMethod.NoSortFh:
                    return new NoSort.Fh.FhSegmentation();

                case SegmentationMethod.NoSortCreditFh:
                    return new NoSort.FhCredit.FhSegmentation();

                case SegmentationMethod.NoSortDSDFh:
                    return new NoSortDSD.Fh.FhSegmentation();

                case SegmentationMethod.NoSortCreditDSDFh:
                    return new NoSortDSD.FhCredit.FhSegmentation();

                default:
                    return new Classic.Fh.FhSegmentation();
            }

        }
    }
}
