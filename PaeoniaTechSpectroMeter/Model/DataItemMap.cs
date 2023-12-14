using CsvHelper.Configuration;

namespace PaeoniaTechSpectroMeter.Model
{
    public class DataItemMap : ClassMap<DataItem>
    {
        public DataItemMap()
        {
            Map(m => m.Timestamp).Name("Time Stamp");
            Map(m => m.Name).Name("Name");
            Map(m => m.PassNo).Name("Pass No.");
            Map(m => m.Operator).Name("Operator");
            Map(m => m.AnalysisType).Name("Analysis Type");
            Map(m => m.SampleType).Name("Sample Type");
            Map(m => m.Ethanol).Name("Ethanol");
            Map(m => m.Denaturant).Name("Denaturant");
            Map(m => m.Methanol).Name("Methanol");
            Map(m => m.Water).Name("Water");
            Map(m => m.IsSelected).Ignore(); //Exclude IsSelected property
            Map(m => m.Batch).Name("Batch");
        }
    }
}
