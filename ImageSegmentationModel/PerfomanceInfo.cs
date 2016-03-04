using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSegmentationModel
{
    public struct PerfomanceInfo
    {
        public long BuildingPerfomance { get; set; }
        public long SortingPerfomance { get; set; }
        public long AlgorithmPerfomance { get; set; }
        public long SmallSegmentMargingPerfomance { get; set; }
        public long SummaryPerfomance
        {
            get
            {
                return BuildingPerfomance
                     + SortingPerfomance
                     + AlgorithmPerfomance
                     + SmallSegmentMargingPerfomance;
            }
        }
    }
}
