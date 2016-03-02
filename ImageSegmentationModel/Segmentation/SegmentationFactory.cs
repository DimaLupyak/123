using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageSegmentationModel.Segmentation
{    
    public class SegmentationFactory
    {
        private static SegmentationFactory _instance;
        public static SegmentationFactory Instance
        {
            get { return _instance ?? (_instance = new SegmentationFactory()); }
        }

       

        public IFhSegmentation GetFhSegmentation(SegmentationMethod method)
        {
            switch (method)
            {
                case SegmentationMethod.FhOriginal:
                    return new Fh.FhSegmentation();
                case SegmentationMethod.FhWithoutSort:
                    return new FhWithoutSort.FhSegmentation();
                case SegmentationMethod.FhCreditWithoutSort:
                    return new FhCreditWithoutSort.FhSegmentation();
                case SegmentationMethod.FhDSD:
                    return new FhDSD.FhSegmentation();
                case SegmentationMethod.FhDSDWithoutSort:
                    return new FhDSDWithoutSort.FhSegmentation();
                default:
                    return new Fh.FhSegmentation();
            }
            
        }
    }
}
