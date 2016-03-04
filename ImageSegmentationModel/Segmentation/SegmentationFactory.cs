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

        public IFhSegmentation GetFhSegmentation(DataStructure dataStructure, SortModification sortModification, MargeHeuristic margeHeuristic)
        {
            if(dataStructure == DataStructure.SimpleGhaph)
            {
                if(sortModification == SortModification.WithSorting)
                {
                    if (margeHeuristic == MargeHeuristic.K)
                    {
                        return new Classic.Fh.FhSegmentation();
                    }
                    else if (margeHeuristic == MargeHeuristic.Credit)
                    {
                        return new Classic.FhCredit.FhSegmentation();
                    }
                }
                else if (sortModification == SortModification.NoSorting)
                {
                    if (margeHeuristic == MargeHeuristic.K)
                    {
                        return new NoSort.Fh.FhSegmentation();
                    }
                    else if (margeHeuristic == MargeHeuristic.Credit)
                    {
                        return new NoSort.FhCredit.FhSegmentation();
                    }
                }
            }
            else if (dataStructure == DataStructure.DisjointSetDataGhaph)
            {
                if (sortModification == SortModification.WithSorting)
                {
                    if (margeHeuristic == MargeHeuristic.K)
                    {
                        return new DSD.Fh.FhSegmentation();
                    }
                    else if (margeHeuristic == MargeHeuristic.Credit)
                    {
                        return new DSD.FhCredit.FhSegmentation();
                    }
                }
                else if (sortModification == SortModification.NoSorting)
                {
                    if (margeHeuristic == MargeHeuristic.K)
                    {
                        return new NoSortDSD.Fh.FhSegmentation();
                    }
                    else if (margeHeuristic == MargeHeuristic.Credit)
                    {
                        return new NoSortDSD.FhCredit.FhSegmentation();
                    }
                }
            }
            return new Classic.Fh.FhSegmentation();
        }
    }
}
