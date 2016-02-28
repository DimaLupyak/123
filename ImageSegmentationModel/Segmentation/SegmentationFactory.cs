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
    public enum ConnectingMethod
    {
        Connecred_4, Connecred_8
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
                    return new Fh.FhSegmentation();
                case SegmentationMethod.WithoutSort:
                    return new FhDSU.FhSegmentation();
                default:
                    return new Fh.FhSegmentation();
            }
            
        }
    }
}
