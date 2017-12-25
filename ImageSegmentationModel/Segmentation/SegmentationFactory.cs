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

        public IFhSegmentation GetFhSegmentation(SortModification sortModification, MargeHeuristic margeHeuristic)
        {
            if (sortModification == SortModification.WithSorting)
            {
                if (margeHeuristic == MargeHeuristic.K)
                {
                    return new SLL.Fh.FhSegmentation();
                }
                else if (margeHeuristic == MargeHeuristic.Credit)
                {
                    return new SLL.FhCredit.FhSegmentation();
                }
            }
            else if (sortModification == SortModification.NoSorting)
            {
                if (margeHeuristic == MargeHeuristic.K)
                {
                    return new NoSortSLL.Fh.FhSegmentation();
                }
                else if (margeHeuristic == MargeHeuristic.Credit)
                {
                    return new NoSortSLL.FhCredit.FhSegmentation();
                }
            }
            return new NoSortSLL.FhCredit.FhSegmentation();
        }
    }
}
