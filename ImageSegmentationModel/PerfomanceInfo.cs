using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSegmentationModel
{
    public struct PerfomanceInfo
    {
        public int StructureCreatingPerfomance { get; set; }
        public int StructureFillingPerfomance { get; set; }
        public int SortingPerfomance { get; set; }
        public int AlgorithmPerfomance { get; set; }
        public int SmallSegmentMargingPerfomance { get; set; }
        public int SummaryPerfomance
        {
            get
            {
                return StructureCreatingPerfomance
                     + StructureFillingPerfomance
                     + SortingPerfomance
                     + AlgorithmPerfomance
                     + SmallSegmentMargingPerfomance;
            }
        }
    }
}
