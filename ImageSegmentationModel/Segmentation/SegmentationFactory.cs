using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageSegmentationModel.Segmentation
{

    public enum SegmentationMethod
    {
        Original, WithoutSort
    }

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
                case SegmentationMethod.Original:
                    return new FhSegmentation1();
                case SegmentationMethod.WithoutSort:
                    return new FhSegmentation();
                default:
                    return new FhSegmentation1();
            }
            
        }
    }
}
