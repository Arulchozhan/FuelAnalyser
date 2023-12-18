using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaeoniaTechSpectroMeter.Model
{
    public class DataItem
    {
        public DateTime Timestamp { get; set; }
        public string Name { get; set; }
        public string PassNo { get; set; }
        public string Operator { get; set; }
        public string AnalysisType { get; set; }
        public string SampleType { get; set; }
        public double? Ethanol { get; set; }
        public double? Denaturant { get; set; }
        public double? Methanol { get; set; }
        public double? Water { get; set; }

        public bool IsSelected { get; set; }

        public int? Batch { get; set; }
    }
}
